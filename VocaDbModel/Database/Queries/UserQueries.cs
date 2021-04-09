#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Exceptions;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources.Messages;
using VocaDb.Model.Service;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Service.Security.StopForumSpam;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries
{
	/// <summary>
	/// Database queries related to <see cref="User"/>.
	/// </summary>
	public class UserQueries : QueriesBase<IUserRepository, User>
	{
		/// <summary>
		/// Cached user stats, these might be slightly inaccurate.
		/// Most of the values are just "fun" statistical information, some of them aren't even displayed directly.
		/// </summary>
		class CachedUserStats
		{
			public int AlbumCollectionCount { get; set; }

			public int ArtistCount { get; set; }

			public int CommentCount { get; set; }

			public int EditCount { get; set; }

			public int FavoriteSongCount { get; set; }

			// Statistical information, not essential
			public int[] FavoriteTags { get; set; }

			// Only used for "power"
			public int OwnedAlbumCount { get; set; }

			// Only used for "power"
			public int RatedAlbumCount { get; set; }

			public int SubmitCount { get; set; }

			public int TagVotes { get; set; }
		}

		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly BrandableStringsManager _brandableStringsManager;
		private readonly ObjectCache _cache;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly IAggregatedEntryImageUrlFactory _entryImagePersister;
		private readonly IEnumTranslations _enumTranslations;
		private readonly IUserMessageMailer _mailer;
		private readonly IStopForumSpamClient _sfsClient;
		private readonly IUserIconFactory _userIconFactory;

		public IEntryLinkFactory EntryLinkFactory => _entryLinkFactory;

		private IQueryable<User> AddOrder(IQueryable<User> query, UserSortRule sortRule) => sortRule switch
		{
			UserSortRule.Name => query.OrderBy(u => u.Name),
			UserSortRule.RegisterDate => query.OrderBy(u => u.CreateDate),
			UserSortRule.Group => query.OrderBy(u => u.GroupId).ThenBy(u => u.Name),
			_ => query,
		};

		private UserReport CreateReport(IDatabaseContext ctx, User reportedUser, UserReportType reportType, string hostname, string notes)
		{
			var report = new UserReport(reportedUser, reportType, ctx.OfType<User>().GetLoggedUser(PermissionContext), hostname, notes);
			ctx.Save(report);
			return report;
		}

		private int[] GetFavoriteTagIds(IDatabaseContext<User> ctx, User user)
		{
			/* 
				Note: There have been some performance problems with this query.
				There's a DB index for both AllSongTagUsages (Tag-Song) and UserFavorites (Song-User).
				Attempting to do the sorting by count in memory.
			*/

			var tags = ctx
				.Query<Tag>()
				.Where(t => t.CategoryName != TagCommonCategoryNames.Lyrics && t.CategoryName != TagCommonCategoryNames.Distribution)
				.Select(t => new
				{
					Id = t.Id,
					Count = t.AllSongTagUsages.Count(u => u.Entry.UserFavorites.Any(f => f.User.Id == user.Id))
				})
				.ToArray()
				.Where(t => t.Count > 0)
				.OrderByDescending(t => t.Count)
				.Take(8)
				.Select(t => t.Id)
				.ToArray();

			return tags;
		}

		private CachedUserStats GetAlbumCounts(IDatabaseContext<User> ctx, User user)
		{
			return ctx
				.Query()
				.Where(u => u.Id == user.Id)
				.Select(u => new CachedUserStats
				{
					AlbumCollectionCount = u.AllAlbums.Count(a => !a.Album.Deleted),
					OwnedAlbumCount = u.AllAlbums.Count(a => !a.Album.Deleted && a.PurchaseStatus == PurchaseStatus.Owned),
					RatedAlbumCount = u.AllAlbums.Count(a => !a.Album.Deleted && a.Rating != 0),
				})
				.First();
		}

		private int GetArtistCount(IDatabaseContext<User> ctx, User user)
		{
			return ctx.Query<ArtistForUser>().Count(u => u.User.Id == user.Id && !u.Artist.Deleted);
		}

		private int GetSongCount(IDatabaseContext<User> ctx, User user)
		{
			return ctx.Query<FavoriteSongForUser>().Count(u => u.User.Id == user.Id && !u.Song.Deleted);
		}

		private CachedUserStats GetCachedUserStats(IDatabaseContext<User> ctx, User user)
		{
			var key = $"CachedUserStats.{user.Id}";
			return _cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(4), () =>
			{
				var stats = new CachedUserStats();

				try
				{
					var albumCounts = GetAlbumCounts(ctx, user);
					stats.AlbumCollectionCount = albumCounts.AlbumCollectionCount;
					stats.OwnedAlbumCount = albumCounts.OwnedAlbumCount;
					stats.RatedAlbumCount = albumCounts.RatedAlbumCount;

					stats.ArtistCount = GetArtistCount(ctx, user);
					stats.FavoriteSongCount = GetSongCount(ctx, user);

					stats.CommentCount = ctx.Query<Comment>().WhereNotDeleted().Count(c => c.Author.Id == user.Id);
					stats.EditCount = ctx.Query<ActivityEntry>().Count(c => c.Author.Id == user.Id);
					stats.SubmitCount = ctx.Query<ActivityEntry>().Count(c => c.Author.Id == user.Id && c.EditEvent == EntryEditEvent.Created);

					stats.TagVotes
						= ctx.Query<SongTagVote>().Count(t => t.User.Id == user.Id)
						+ ctx.Query<AlbumTagVote>().Count(t => t.User.Id == user.Id)
						+ ctx.Query<ArtistTagVote>().Count(t => t.User.Id == user.Id);

					stats.FavoriteTags = GetFavoriteTagIds(ctx, user);
				}
				catch (HibernateException x)
				{
					// TODO: Loading of stats timeouts sometimes. Since they're not essential we can accept returning only partial stats.
					// However, this should be fixed by tuning the queries further.
					s_log.Error(x, "Unable to load user stats");
				}

				return stats;
			});
		}

		private async Task SendPrivateMessageNotification(string mySettingsUrl, string messagesUrl, UserMessage message)
		{
			ParamIs.NotNull(() => message);

			var subject = $"New private message from {message.Sender.Name}";
			var body = string.Format(
				"You have received a message from {0}. " +
				"You can view your messages at {1}." +
				"\n\n" +
				"If you do not wish to receive more email notifications such as this, you can adjust your settings at {2}.",
				message.Sender.Name, messagesUrl, mySettingsUrl);

			await _mailer.SendEmailAsync(message.Receiver.Email, message.Receiver.Name, subject, body);
		}

		private ServerOnlyUserDetailsContract GetUserDetails(IDatabaseContext<User> session, User user)
		{
			var details = new ServerOnlyUserDetailsContract(user, PermissionContext);

			var cachedStats = GetCachedUserStats(session, user);
			details.AlbumCollectionCount = cachedStats.AlbumCollectionCount;
			details.ArtistCount = cachedStats.ArtistCount;
			details.FavoriteSongCount = cachedStats.FavoriteSongCount;
			details.FavoriteTags = cachedStats.FavoriteTags != null ? session.Query<Tag>().Where(t => cachedStats.FavoriteTags.Contains(t.Id)).ToArray()
				.Select(t => new TagBaseContract(t, LanguagePreference, true)).ToArray() : new TagBaseContract[0];
			details.CommentCount = cachedStats.CommentCount;
			details.EditCount = cachedStats.EditCount;
			details.SubmitCount = cachedStats.SubmitCount;
			details.TagVotes = cachedStats.TagVotes;

			details.FavoriteAlbums = session.Query<AlbumForUser>()
				.Where(c => c.User.Id == user.Id && !c.Album.Deleted && c.Rating > 3)
				.OrderByDescending(c => c.Rating)
				.ThenByDescending(c => c.Id)
				.Select(a => a.Album)
				.Take(7)
				.ToArray()
				.Select(c => new AlbumForApiContract(c, LanguagePreference, _entryImagePersister, AlbumOptionalFields.AdditionalNames | AlbumOptionalFields.MainPicture))
				.ToArray();

			details.FollowedArtists = session.Query<ArtistForUser>()
				.Where(c => c.User.Id == user.Id && !c.Artist.Deleted)
				.OrderByDescending(a => a.Id)
				.Select(c => c.Artist)
				.Take(6)
				.ToArray()
				.Select(c => new ArtistContract(c, LanguagePreference))
				.ToArray();

			details.LatestComments = session.Query<UserComment>()
				.WhereNotDeleted()
				.Where(c => c.EntryForComment == user).OrderByDescending(c => c.Created).Take(3)
				.ToArray()
				.Select(c => new CommentForApiContract(c, _userIconFactory)).ToArray();

			details.LatestRatedSongs = session.Query<FavoriteSongForUser>()
				.Where(c => c.User.Id == user.Id && !c.Song.Deleted)
				.OrderByDescending(c => c.Date)
				.Select(c => c.Song)
				.Take(6)
				.ToArray()
				.Select(c => new SongForApiContract(c, LanguagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl))
				.ToArray();

			// Correct cached stats if we can determine they're out of date
			details.AlbumCollectionCount = Math.Max(details.AlbumCollectionCount, details.FavoriteAlbums.Length);
			details.ArtistCount = Math.Max(details.ArtistCount, details.FollowedArtists.Length);
			details.FavoriteSongCount = Math.Max(details.FavoriteSongCount, details.LatestRatedSongs.Length);
			var songListCount = session.Query<SongList>().Count(l => l.Author.Id == user.Id && l.FeaturedCategory == SongListFeaturedCategory.Nothing);

			details.Power = UserHelper.GetPower(details, cachedStats.OwnedAlbumCount, cachedStats.RatedAlbumCount, songListCount);
			details.Level = UserHelper.GetLevel(details.Power);
			details.IsVeteran = UserHelper.IsVeteran(details);

			// If the user is viewing their own profile, check for possible producer account.
			// Skip users who are not active, limited or are already verified artists.
			if (user.Active && user.GroupId >= UserGroupId.Regular && user.GroupId <= UserGroupId.Trusted && user.Equals(PermissionContext.LoggedUser) && !user.VerifiedArtist)
			{
				// Scan by Twitter account name and entry name.
				var twitterUrl = !string.IsNullOrEmpty(user.Options.TwitterName) ? $"https://twitter.com/{user.Options.TwitterName}" : null;
				var producerTypes = new[] { ArtistType.Producer, ArtistType.Animator, ArtistType.Illustrator };

				details.PossibleProducerAccount = session.Query<Artist>().Any(a =>
					!a.Deleted
					&& producerTypes.Contains(a.ArtistType)
					&& (a.Names.Names.Any(n => n.Value == user.Name)
						|| (twitterUrl != null && a.WebLinks.Any(l => l.Url == twitterUrl)))
					&& !a.OwnerUsers.Any());

				if (details.PossibleProducerAccount)
				{
					session.AuditLogger.SysLog("possible producer account");
				}
			}

			return details;
		}

		private bool IsPoisoned(IDatabaseContext<User> ctx, string lcUserName)
		{
			return ctx.OfType<UserOptions>().Query().Any(o => o.Poisoned && o.User.NameLC == lcUserName);
		}

		private string MakeGeoIpToolLink(string hostname)
		{
			return $"<a href='http://www.geoiptool.com/?IP={hostname}'>{hostname}</a>";
		}

		private async Task SendEmailVerificationRequest(IDatabaseContext<User> ctx, User user, string resetUrl, string subject)
		{
			var request = new PasswordResetRequest(user);
			await ctx.SaveAsync(request);
			var body = string.Format(UserAccountStrings.VerifyEmailBody, _brandableStringsManager.Layout.SiteName, resetUrl, request.Id);

			await _mailer.SendEmailAsync(request.User.Email, request.User.Name, subject, body);
		}

		/// <summary>
		/// Validates email address.
		/// </summary>
		/// <param name="email">Email to be validated.</param>
		/// <exception cref="InvalidEmailFormatException">If <paramref name="email"/> is not valid email.</exception>
		private void ValidateEmail(string email)
		{
			try
			{
				new MailAddress(email);
			}
			catch (FormatException x)
			{
				throw new InvalidEmailFormatException("Email format is invalid", x);
			}
		}

		/// <summary>
		/// Validates username.
		/// </summary>
		/// <param name="session">DB context.</param>
		/// <param name="name">Desired username.</param>
		/// <param name="id">ID of the user requesting the name.</param>
		/// <exception cref="InvalidUserNameException">Username is not valid: either it's empty or contains invalid characters.</exception>
		/// <exception cref="UserNameAlreadyExistsException">Username is already in use by another user.</exception>
		private void ValidateUsername(IDatabaseContext<User> session, string name, int id)
		{
			if (!User.IsValidName(name))
			{
				throw new InvalidUserNameException(name);
			}

			var nameInUse = session.Query().Any(u => u.Name == name && u.Id != id);

			if (nameInUse)
			{
				throw new UserNameAlreadyExistsException();
			}
		}

		public UserQueries(IUserRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IStopForumSpamClient sfsClient,
			IUserMessageMailer mailer, IUserIconFactory userIconFactory, IAggregatedEntryImageUrlFactory entryImagePersister,
			ObjectCache cache,
			BrandableStringsManager brandableStringsManager, IEnumTranslations enumTranslations)
			: base(repository, permissionContext)
		{
			ParamIs.NotNull(() => repository);
			ParamIs.NotNull(() => permissionContext);
			ParamIs.NotNull(() => entryLinkFactory);
			ParamIs.NotNull(() => sfsClient);
			ParamIs.NotNull(() => mailer);

			_entryLinkFactory = entryLinkFactory;
			_sfsClient = sfsClient;
			_mailer = mailer;
			_userIconFactory = userIconFactory;
			_entryImagePersister = entryImagePersister;
			_cache = cache;
			_brandableStringsManager = brandableStringsManager;
			_enumTranslations = enumTranslations;
		}

		public void AddFollowedTag(int userId, int tagId)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(ctx =>
			{
				var exists = ctx.Query<TagForUser>().Any(u => u.User.Id == userId && u.Tag.Id == tagId);

				if (exists)
					return;

				var user = ctx.Load<User>(userId);
				var tag = ctx.Load<Tag>(tagId);

				ctx.Save(user.AddTag(tag));

				AuditLog($"followed {tag}", ctx, user);
			});
		}

		public CommentQueries<UserComment, User> Comments(IDatabaseContext<User> ctx)
		{
			return new CommentQueries<UserComment, User>(ctx.OfType<UserComment>(), PermissionContext, _userIconFactory, _entryLinkFactory);
		}

		/// <summary>
		/// Attempts to log in a user.
		/// </summary>
		/// <param name="name">Username. Cannot be null.</param>
		/// <param name="pass">Password. Cannot be null.</param>
		/// <param name="hostname">Host name where the user is logging in from. Cannot be null.</param>
		/// <param name="culture">User culture name. Can be empty.</param>
		/// <param name="delayFailedLogin">Whether failed login should cause artificial delay.</param>
		/// <returns>Login attempt result. Cannot be null.</returns>
		public LoginResult CheckAuthentication(string name, string pass, string hostname, string culture, bool delayFailedLogin)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(pass))
				return LoginResult.CreateError(LoginError.InvalidPassword);

			var lc = name.ToLowerInvariant();

			return _repository.HandleTransaction(ctx =>
			{
				if (IsPoisoned(ctx, lc))
				{
					ctx.AuditLogger.SysLog($"failed login from {MakeGeoIpToolLink(hostname)} - account is poisoned.", name);
					return LoginResult.CreateError(LoginError.AccountPoisoned);
				}

				// Attempt to find user by either lowercase username.
				var user = ctx.Query().FirstOrDefault(u => u.Active && (u.NameLC == lc || (u.Options.EmailVerified && u.Email == name)));

				if (user == null)
				{
					ctx.AuditLogger.AuditLog($"failed login from {MakeGeoIpToolLink(hostname)} - no user.", name);
					if (delayFailedLogin)
						Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.NotFound);
				}

				var algorithm = PasswordHashAlgorithms.Get(user.PasswordHashAlgorithm);

				// Attempt to verify password.				
				var hashed = algorithm.HashPassword(pass, user.Salt, user.NameLC);

				if (user.Password != hashed)
				{
					ctx.AuditLogger.AuditLog($"failed login from {MakeGeoIpToolLink(hostname)} - wrong password.", name);
					if (delayFailedLogin)
						Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.InvalidPassword);
				}

				// Login attempt successful.
				ctx.AuditLogger.AuditLog($"logged in from {MakeGeoIpToolLink(hostname)} with '{name}'.", user);

				user.UpdatePassword(pass, PasswordHashAlgorithms.Default);
				user.UpdateLastLogin(hostname, culture);
				ctx.Update(user);

				return LoginResult.CreateSuccess(new ServerOnlyUserContract(user));
			});
		}

		public bool CheckPasswordResetRequest(Guid requestId)
		{
			var cutoff = DateTime.Now - PasswordResetRequest.ExpirationTime;

			return _repository.HandleQuery(ctx => ctx.OfType<PasswordResetRequest>().Query().Any(r => r.Id == requestId && r.Created >= cutoff));
		}

		public CommentForApiContract CreateComment(int userId, string message)
		{
			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return _repository.HandleTransaction(ctx =>
			{
				var user = ctx.Load(userId);
				var agent = ctx.CreateAgentLoginData(PermissionContext);

				ctx.AuditLogger.AuditLog($"creating comment for {EntryLinkFactory.CreateEntryLink(user)}: '{HttpUtility.HtmlEncode(message)}'", agent.User);

				var comment = user.CreateComment(message, agent);
				ctx.OfType<UserComment>().Save(comment);

				var commentMsg = comment.Message.Truncate(200);
				var notificationMsg = $"{agent.Name} posted a comment on your profile.\n\n{commentMsg}";
				var notification = new UserMessage(user, "Comment posted on your profile", notificationMsg, false);
				ctx.OfType<UserMessage>().Save(notification);

				return new CommentForApiContract(comment, _userIconFactory);
			});
		}

		public (bool created, int reportId) CreateReport(int userId, UserReportType reportType, string hostname, string notes,
			int reportCountDisable = 10, int reportCountLimit = 5)
		{
			PermissionContext.VerifyPermission(PermissionToken.ReportUser);

			if (string.IsNullOrEmpty(notes))
			{
				s_log.Error("Notes are required");
				return (false, 0);
			}

			return _repository.HandleTransaction(ctx =>
			{
				var user = ctx.Load(userId);

				ctx.AuditLogger.SysLog($"reporting {user} as {reportType}");

				var existing = ctx.Query<UserReport>()
					.FirstOrDefault(ur => ur.Entry.Id == userId && ur.Status == ReportStatus.Open && ur.User.Id == PermissionContext.LoggedUserId);

				if (existing != null)
				{
					s_log.Info("Report already exists");
					return (false, existing.Id);
				}

				var report = CreateReport(ctx, user, reportType, hostname, notes);

				if (user.GroupId <= UserGroupId.Regular && reportType == UserReportType.Spamming)
				{
					var activeReportCount = ctx.Query<UserReport>()
						.Where(ur => ur.Entry.Id == userId
							&& ur.Status == ReportStatus.Open
							&& ur.ReportType == UserReportType.Spamming
							&& ur.User != null)
						.ToArray()
						.Distinct(ur => ur.User.Id)
						.Count();
					if (activeReportCount >= reportCountDisable)
					{
						s_log.Info("User disabled");
						user.Active = false;
						ctx.Update(user);
					}
					else if (activeReportCount >= reportCountLimit)
					{
						s_log.Info("User set to limited");
						user.GroupId = UserGroupId.Limited;
						ctx.Update(user);
					}
				}

				ctx.AuditLogger.AuditLog($"reported {user} as {reportType}");

				return (true, report.Id);
			});
		}

		/// <summary>
		/// Disconnects Twitter account for the currently logged in user.
		/// Twitter account can NOT be disconnected if the user has not set a VocaDB password.
		/// </summary>
		/// <exception cref="NoPasswordException">If the user has not set a password.</exception>
		public void DisconnectTwitter()
		{
			PermissionContext.VerifyLogin();

			_repository.HandleTransaction(ctx =>
			{
				var user = ctx.GetLoggedUser(PermissionContext);

				user.ClearTwitter();

				ctx.AuditLogger.AuditLog("disconnected twitter");
			});
		}

		private string GetSFSCheckStr(SFSResponseContract result)
		{
			if (result == null)
				return "error";

			return result.Conclusion switch
			{
				SFSCheckResultType.Malicious => $"Malicious ({result.Confidence} % confidence)",
				SFSCheckResultType.Uncertain => $"Uncertain ({result.Confidence} % confidence)",
				_ => "Ok",
			};
		}

		/// <summary>
		/// Clears all rated albums and songs by a user.
		/// Also updates rating totals.
		/// 
		/// Staff members cannot be cleared.
		/// </summary>
		/// <param name="id">User Id.</param>
		public void ClearRatings(int id, EntryTypes? entryTypes = null)
		{
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			var entryTypeFlags = entryTypes ?? EnumVal<EntryTypes>.All;

			_repository.HandleTransaction(ctx =>
			{
				var user = ctx.Load(id);

				if (!user.CanBeDisabled)
					throw new NotAllowedException("This user account cannot be cleared.");

				ctx.AuditLogger.AuditLog($"clearing ratings by {user}");

				while (entryTypeFlags.HasFlag(EntryTypes.Album) && user.AllAlbums.Any())
				{
					var albumLink = user.AllAlbums[0];
					albumLink.Delete();
					ctx.Delete(albumLink);
					ctx.Update(albumLink.Album); // Update album ratings
				}

				while (entryTypeFlags.HasFlag(EntryTypes.Song) && user.FavoriteSongs.Any())
				{
					var songLink = user.FavoriteSongs[0];
					songLink.Delete();
					ctx.Delete(songLink);
					ctx.Update(songLink.Song); // Update song ratings
				}

				while (entryTypeFlags.HasFlag(EntryTypes.Artist) && user.AllArtists.Any())
				{
					var artistLink = user.AllArtists[0];
					ctx.Delete(artistLink);
					artistLink.Delete();
				}

				ctx.Update(user);
			});
		}

		/// <param name="name">User name. Must be unique. Cannot be null or empty.</param>
		/// <param name="pass">Password. Cannot be null or empty.</param>
		/// <param name="email">Email address. Must be unique if specified. Cannot be null.</param>
		/// <param name="hostname">Host name (usually IP address) where the registration is from.</param>
		/// <param name="userAgent">User agent. Can be empty.</param>
		/// <param name="culture">User culture name. Can be empty.</param>
		/// <param name="timeSpan">Time in which the user filled the registration form.</param>
		/// <param name="softbannedIPs">List of application's soft-banned IPs. Soft-banned IPs are cleared when the application restarts.</param>
		/// <param name="verifyEmailUrl">Email verification URL. Cannot be null.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		/// <exception cref="TooFastRegistrationException">If the user registered too fast.</exception>
		/// <exception cref="RestrictedIPException">User's IP was banned, or determined to be malicious.</exception>
		public async Task<ServerOnlyUserContract> Create(string name, string pass, string email, string hostname,
			string userAgent,
			string culture,
			TimeSpan timeSpan,
			IPRuleManager ipRuleManager, string verifyEmailUrl)
		{
			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);
			ParamIs.NotNull(() => email);

			if (timeSpan < TimeSpan.FromSeconds(5))
			{
				s_log.Warn("Suspicious registration form fill time ({0}) from {1}.", timeSpan, hostname);

				if (timeSpan < TimeSpan.FromSeconds(2))
				{
					ipRuleManager.AddTempBannedIP(hostname, "Suspicious registration form fill time");
				}

				throw new TooFastRegistrationException();
			}

			return await _repository.HandleQueryAsync(async ctx =>
			{
				// Verification
				var lc = name.ToLowerInvariant();
				var existing = await ctx.Query().Where(u => u.NameLC == lc).VdbFirstOrDefaultAsync();

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email))
				{
					ValidateEmail(email);

					var normalizedEmail = MailAddressNormalizer.Normalize(email);
					existing = await ctx.Query().Where(u => u.Active && u.NormalizedEmail == normalizedEmail).VdbFirstOrDefaultAsync();

					if (existing != null)
						throw new UserEmailAlreadyExistsException();
				}

				var confidenceAutoban = 90;
				var sfsCheckResult = await _sfsClient.CallApiAsync(hostname) ?? new SFSResponseContract();
				var sfsStr = GetSFSCheckStr(sfsCheckResult);

				if (sfsCheckResult.Appears && sfsCheckResult.Confidence >= confidenceAutoban)
				{
					using (var tx = ctx.BeginTransaction())
					{
						ctx.AuditLogger.AuditLog($"flagged by SFS, conficence {sfsCheckResult.Confidence}%, user banned", name);
						ipRuleManager.AddPermBannedIP(ctx, hostname, $"SFS: {name}");
						await tx.CommitAsync();
					}
					throw new RestrictedIPException();
				}

				// All ok, create user
				User user;
				using (var tx = ctx.BeginTransaction())
				{
					user = await CreateUser(ctx, name, pass, email, hostname, culture, sfsCheckResult, verifyEmailUrl);
					ctx.AuditLogger.AuditLog($"registered from {MakeGeoIpToolLink(hostname)} in {timeSpan} (SFS check {sfsStr}, UA '{userAgent}').", user);
					await tx.CommitAsync();
				}

				return new ServerOnlyUserContract(user);
			});
		}

		private async Task<User> CreateUser(IDatabaseContext<User> ctx, string name, string pass, string email, string hostname, string culture,
			SFSResponseContract sfsCheckResult, string verifyEmailUrl)
		{
			var confidenceReport = 1;
			var confidenceLimited = 60;

			var user = new User(name, pass, email, PasswordHashAlgorithms.Default);
			user.UpdateLastLogin(hostname, culture);
			await ctx.SaveAsync(user);

			if (sfsCheckResult.Appears && sfsCheckResult.Confidence >= confidenceReport)
			{
				var report = new UserReport(user, UserReportType.MaliciousIP, null, hostname,
					$"Confidence {sfsCheckResult.Confidence} %, Frequency {sfsCheckResult.Frequency}, Last seen {sfsCheckResult.LastSeen.ToShortDateString()}. Conclusion {sfsCheckResult.Conclusion}.");

				await ctx.OfType<UserReport>().SaveAsync(report);

				if (sfsCheckResult.Confidence >= confidenceLimited)
				{
					user.GroupId = UserGroupId.Limited;
					await ctx.UpdateAsync(user);
				}
			}

			if (!string.IsNullOrEmpty(user.Email))
			{
				var subject = string.Format(UserAccountStrings.AccountCreatedSubject, _brandableStringsManager.Layout.SiteName);
				await SendEmailVerificationRequest(ctx, user, verifyEmailUrl, subject);
			}

			return user;
		}

		/// <summary>
		/// Creates a new user account using Twitter authentication token.
		/// </summary>
		/// <param name="authToken">Twitter OAuth token. Cannot be null or empty.</param>
		/// <param name="name">User name. Must be unique. Cannot be null or empty.</param>
		/// <param name="email">Email address. Must be unique. Cannot be null.</param>
		/// <param name="twitterId">Twitter user Id. Cannot be null or empty.</param>
		/// <param name="twitterName">Twitter user name. Cannot be null.</param>
		/// <param name="hostname">Host name where the registration is from.</param>
		/// <param name="culture">User culture name. Can be empty.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		public ServerOnlyUserContract CreateTwitter(string authToken, string name, string email, int twitterId, string twitterName, string hostname, string culture)
		{
			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => email);

			return _repository.HandleTransaction(ctx =>
			{
				var lc = name.ToLowerInvariant();
				var existing = ctx.Query().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email))
				{
					ValidateEmail(email);

					var normalizedEmail = MailAddressNormalizer.Normalize(email);
					existing = ctx.Query().FirstOrDefault(u => u.Active && u.NormalizedEmail == normalizedEmail);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();
				}

				var user = new User(name, string.Empty, email, PasswordHashAlgorithms.Default);
				user.Options.TwitterId = twitterId;
				user.Options.TwitterName = twitterName;
				user.Options.TwitterOAuthToken = authToken;
				user.UpdateLastLogin(hostname, culture);
				ctx.Save(user);

				ctx.AuditLogger.AuditLog($"registered from {MakeGeoIpToolLink(hostname)} using Twitter name '{twitterName}'.", user);

				return new ServerOnlyUserContract(user);
			});
		}

		public void DisableUser(int userId)
		{
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			_repository.HandleTransaction(ctx =>
			{
				var user = ctx.Load(userId);

				if (!user.CanBeDisabled)
					throw new NotAllowedException("This user account cannot be disabled.");

				user.Active = false;

				ctx.AuditLogger.AuditLog($"disabled {EntryLinkFactory.CreateEntryLink(user)}.");

				ctx.Update(user);
			});
		}

		public string[] FindNames(SearchTextQuery textQuery, int maxResults, bool allowDisabled)
		{
			if (textQuery.IsEmpty)
				return new string[] { };

			return HandleQuery(session =>
			{
				var query = session.Query<User>()
					.WhereHasName(textQuery);

				if (!allowDisabled)
				{
					query = query.Where(u => u.Active);
				}

				var names = query
					.Select(n => n.Name)
					.OrderBy(n => n)
					.Take(maxResults)
					.ToArray();

				return names;
			});
		}

		public PartialFindResult<T> GetAlbumCollection<T>(AlbumCollectionQueryParams queryParams, Func<AlbumForUser, bool, T> fac)
		{
			ParamIs.NotNull(() => queryParams);

			return HandleQuery(session =>
			{
				var paging = queryParams.Paging;
				var loggedUserId = PermissionContext.LoggedUserId;
				var user = session.Load(queryParams.UserId);
				var shouldShowCollectionStatus = user.Id == loggedUserId || user.Options.PublicAlbumCollection;

				var query = session.OfType<AlbumForUser>().Query()
					.Where(a => a.User.Id == user.Id
						&& !a.Album.Deleted
						&& (shouldShowCollectionStatus || a.Rating > 0))
					.WhereHasName(queryParams.TextQuery)
					.WhereHasCollectionStatus(queryParams.FilterByStatus)
					.WhereHasArtist(queryParams.ArtistId)
					.WhereHasReleaseEvent(queryParams.ReleaseEventId)
					.WhereAlbumHasTag(queryParams.TagId)
					.WhereAlbumHasTag(queryParams.Tag)
					.WhereAlbumMatchFilters(queryParams.AdvancedFilters)
					.WhereAlbumHasType(queryParams.AlbumType);

				var albums = query
					.OrderBy(queryParams.Sort, PermissionContext.LanguagePreference)
					.Paged(paging)
					.ToArray()
					.Select(afu => fac(afu, shouldShowCollectionStatus))
					.ToArray();

				var count = paging.GetTotalCount ? query.Count() : 0;

				return new PartialFindResult<T>(albums, count);
			});
		}

		public ArtistContract[] GetArtists(int userId)
		{
			return HandleQuery(session =>
				session.Load(userId)
					.Artists
					.Select(a => new ArtistContract(a.Artist, PermissionContext.LanguagePreference))
					.OrderBy(s => s.Name)
					.ToArray());
		}

		public PartialFindResult<T> GetArtists<T>(FollowedArtistQueryParams queryParams, Func<ArtistForUser, T> fac)
		{
			var paging = queryParams.Paging;

			return HandleQuery(ctx =>
			{
				var query = ctx.OfType<ArtistForUser>().Query()
					.Where(a => !a.Artist.Deleted && a.User.Id == queryParams.UserId)
					.WhereArtistHasName(queryParams.TextQuery)
					.WhereArtistHasType(queryParams.ArtistType)
					.WhereArtistHasTags(queryParams.TagIds);

				var artists = query
					.OrderBy(queryParams.SortRule, LanguagePreference)
					.Paged(paging)
					.ToArray()
					.Select(fac)
					.ToArray();

				var count = paging.GetTotalCount ? query.Count() : 0;

				return new PartialFindResult<T>(artists, count);
			});
		}

		public ReleaseEventForApiContract[] GetEvents(int userId, UserEventRelationshipType relationshipType, ReleaseEventOptionalFields fields)
		{
			return HandleQuery(ctx =>
			{
				var user = ctx.Load<User>(userId);
				return user.Events
					.Where(e => !e.ReleaseEvent.Deleted && e.RelationshipType == relationshipType)
					.OrderByDescending(e => e.ReleaseEvent.Date.DateTime)
					.Select(e => new ReleaseEventForApiContract(e.ReleaseEvent, LanguagePreference, fields, _entryImagePersister))
					.ToArray();
			});
		}

		public PartialFindResult<CommentForApiContract> GetProfileComments(int userId, PagingProperties paging)
		{
			return HandleQuery(ctx =>
			{
				var query = ctx.OfType<UserComment>().Query()
					.WhereNotDeleted()
					.Where(c => c.EntryForComment.Id == userId);

				var comments = query
					.OrderByDescending(c => c.Created)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray()
					.Select(c => new CommentForApiContract(c, _userIconFactory))
					.ToArray();

				var count = (paging.GetTotalCount ? query.Count() : 0);

				return new PartialFindResult<CommentForApiContract>(comments, count);
			});
		}

		public PartialFindResult<T> GetRatedSongs<T>(RatedSongQueryParams queryParams, Func<FavoriteSongForUser, T> fac)
		{
			ParamIs.NotNull(() => queryParams);

			return HandleQuery(session =>
			{
				// Apply initial filter
				var q = session.OfType<FavoriteSongForUser>().Query()
					.Where(a => !a.Song.Deleted && a.User.Id == queryParams.UserId)
					.WhereChildHasName(queryParams.TextQuery)
					.WhereSongHasArtists(queryParams.ArtistIds, queryParams.ChildVoicebanks, queryParams.ArtistGrouping)
					.WhereHasRating(queryParams.FilterByRating)
					.WhereSongIsInList(queryParams.SonglistId)
					.WhereSongHasTags(queryParams.TagIds)
					.WhereSongHasTag(queryParams.TagName)
					.WhereSongHasPVService(queryParams.PVServices)
					.WhereMatchFilters(queryParams.AdvancedFilters);

				var queryWithSort = q;

				// Group by rating if needed
				if (queryParams.GroupByRating && queryParams.FilterByRating == SongVoteRating.Nothing)
					queryWithSort = queryWithSort.OrderByDescending(r => r.Rating);

				// Add custom order
				queryWithSort = queryWithSort.OrderBy(queryParams.SortRule, PermissionContext.LanguagePreference);

				// Apply paging
				var resultQ = queryWithSort
					.Paged(queryParams.Paging);

				var contracts = resultQ.ToArray().Select(fac).ToArray();
				var totalCount = (queryParams.Paging.GetTotalCount ? q.Count() : 0);

				return new PartialFindResult<T>(contracts, totalCount);
			});
		}

		private void AddCount(Dictionary<int, int> genresDict, int? parentTag, int count)
		{
			if (parentTag == null || parentTag == 0)
				return;

			if (genresDict.ContainsKey(parentTag.Value))
				genresDict[parentTag.Value] += count;
			else
				genresDict.Add(parentTag.Value, count);
		}

		public Tuple<string, int>[] GetRatingsByGenre(int userId)
		{
			return _repository.HandleQuery(ctx =>
			{
				var genres = ctx
					.OfType<SongTagUsage>()
					.Query()
					.Where(u => u.Entry.UserFavorites.Any(f => f.User.Id == userId) && u.Tag.CategoryName == TagCommonCategoryNames.Genres)
					// NH doesn't support ? operator, instead casting ID to nullable works
					.GroupBy(s => new { TagId = s.Tag.Id, Parent = (int?)s.Tag.Parent.Id })
					.Select(g => new
					{
						TagId = g.Key.TagId,
						Parent = g.Key.Parent,
						Count = g.Count()
					})
					.ToArray();

				var genresDict = genres
					.Where(g => g.Parent == null || g.Parent == 0)
					.ToDictionary(t => t.TagId, t => t.Count);

				foreach (var tag in genres)
				{
					AddCount(genresDict, tag.Parent, tag.Count);
				}

				// Load names for top 10 genres.
				var mainGenreIds = genresDict.OrderByDescending(t => t.Value).Take(10).Select(t => t.Key).ToArray();
				var mainGenreTags = ctx.Query<Tag>().Where(t => mainGenreIds.Contains(t.Id)).SelectIdAndName(LanguagePreference).ToDictionary(t => t.Id);

				var sorted = genresDict.Select(t => new { TagName = mainGenreTags.ContainsKey(t.Key) ? mainGenreTags[t.Key].Name : null, Count = t.Value }).OrderByDescending(t => t.Count);
				var mainGenres = sorted.Take(10);
				var otherCount = sorted.Skip(10).Sum(g => g.Count);

				var allGenres = (otherCount > 0 ? mainGenres.Concat(new[] { new {
					TagName = "Other genres",
					Count = otherCount
				} }) : mainGenres);

				var points = allGenres.Select(g => Tuple.Create(g.TagName, g.Count)).ToArray();

				return points;
			});
		}

		public SongVoteRating GetSongRating(int userId, int songId)
		{
			if (userId == 0)
				return SongVoteRating.Nothing;

			return HandleQuery(ctx =>
			{
				var r = ctx.OfType<FavoriteSongForUser>().Query().FirstOrDefault(s => s.Song.Id == songId && s.User.Id == userId);

				return r != null ? r.Rating : SongVoteRating.Nothing;
			});
		}

		public PartialFindResult<SongListForApiContract> GetCustomSongLists(int userId, SongListQueryParams queryParams, SongListOptionalFields fields)
		{
			return HandleQuery(ctx =>
			{
				var query = ctx.Query<SongList>()
					.WhereNotDeleted()
					.Where(s => s.Author.Id == userId && s.FeaturedCategory == SongListFeaturedCategory.Nothing)
					.WhereHasName(queryParams.TextQuery)
					.WhereHasTags(queryParams.TagIds, queryParams.ChildTags);

				var items = query.OrderBy(queryParams.SortRule)
					.Paged(queryParams.Paging)
					.Select(s => new SongListForApiContract(s, LanguagePreference, _userIconFactory, _entryImagePersister, fields))
					.ToArray();

				var count = queryParams.Paging.GetTotalCount ? query.Count() : 0;

				return new PartialFindResult<SongListForApiContract>(items, count);
			});
		}

		private TagSelectionContract[] GetTagSelections<TEntry, TUsage, TVote>(int entryId, int userId) where TEntry : class, IEntryWithNames where TUsage : GenericTagUsage<TEntry, TVote> where TVote : GenericTagVote<TUsage>
		{
			return HandleQuery(session =>
			{
				var tagsInUse = session.Query<TUsage>().Where(a => a.Entry.Id == entryId && !a.Tag.Deleted).ToArray();
				var tagVotes = session.Query<TVote>().Where(a => a.User.Id == userId && a.Usage.Entry.Id == entryId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag, LanguagePreference, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();
			});
		}

		public TagSelectionContract[] GetAlbumTagSelections(int albumId, int userId)
		{
			return HandleQuery(session =>
			{
				var tagsInUse = session.Query<AlbumTagUsage>().Where(a => a.Entry.Id == albumId && !a.Tag.Deleted).ToArray();
				var tagVotes = session.Query<AlbumTagVote>().Where(a => a.User.Id == userId && a.Usage.Entry.Id == albumId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag, LanguagePreference, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();
			});
		}

		public TagSelectionContract[] GetArtistTagSelections(int artistId, int userId)
		{
			return HandleQuery(session =>
			{
				var tagsInUse = session.Query<ArtistTagUsage>().Where(a => a.Entry.Id == artistId && !a.Tag.Deleted).ToArray();
				var tagVotes = session.Query<ArtistTagVote>().Where(a => a.User.Id == userId && a.Usage.Entry.Id == artistId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag, LanguagePreference, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();
			});
		}

		public TagSelectionContract[] GetEventTagSelections(int eventId, int userId)
		{
			return GetTagSelections<ReleaseEvent, EventTagUsage, EventTagVote>(eventId, userId);
		}

		public TagSelectionContract[] GetEventSeriesTagSelections(int seriesId, int userId)
		{
			return GetTagSelections<ReleaseEventSeries, EventSeriesTagUsage, EventSeriesTagVote>(seriesId, userId);
		}

		public TagSelectionContract[] GetSongListTagSelections(int songListId, int userId)
		{
			return GetTagSelections<SongList, SongListTagUsage, SongListTagVote>(songListId, userId);
		}

		public TagSelectionContract[] GetSongTagSelections(int songId, int userId)
		{
			return HandleQuery(session =>
			{
				var tagsInUse = session.Query<SongTagUsage>().Where(a => a.Entry.Id == songId && !a.Tag.Deleted).ToArray();
				var tagVotes = session.Query<SongTagVote>().Where(a => a.User.Id == userId && a.Usage.Entry.Id == songId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag, LanguagePreference, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();
			});
		}

		public UserForApiContract GetUser(int id, UserOptionalFields fields)
		{
			return HandleQuery(ctx => new UserForApiContract(ctx.Load<User>(id), _userIconFactory, fields));
		}

		public ServerOnlyUserDetailsContract GetUserByNameNonSensitive(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;

			return HandleQuery(session =>
			{
				var user = session
					.Query()
					.FirstOrDefault(u => u.Name == name);

				if (user == null)
					return null;

				return GetUserDetails(session, user);
			});
		}

		public ServerOnlyUserDetailsContract GetUserDetails(int id)
		{
			return HandleQuery(ctx => GetUserDetails(ctx, ctx.Load(id)));
		}

		public PartialFindResult<T> GetUsers<T>(UserQueryParams queryParams,
			Func<User, T> fac)
		{
			return _repository.HandleQuery(ctx =>
			{
				var usersQuery = ctx.Query()
					.WhereHasName(queryParams.Common.TextQuery)
					.WhereKnowsLanguage(queryParams.KnowsLanguage);

				if (queryParams.Group != UserGroupId.Nothing)
				{
					usersQuery = usersQuery.Where(u => u.GroupId == queryParams.Group);
				}

				if (!queryParams.IncludeDisabled)
				{
					usersQuery = usersQuery.Where(u => u.Active && !u.Options.Standalone);
				}

				if (queryParams.OnlyVerifiedArtists)
				{
					usersQuery = usersQuery.Where(u => u.VerifiedArtist);
				}

				if (queryParams.JoinDateAfter.HasValue)
				{
					usersQuery = usersQuery.Where(u => u.CreateDate >= queryParams.JoinDateAfter);
				}

				if (queryParams.JoinDateBefore.HasValue)
				{
					usersQuery = usersQuery.Where(u => u.CreateDate < queryParams.JoinDateBefore);
				}

				var users = AddOrder(usersQuery, queryParams.Sort)
					.Paged(queryParams.Paging)
					.ToArray();

				var count = queryParams.Paging.GetTotalCount ? usersQuery.Count() : 0;
				var contracts = users.Select(fac).ToArray();

				return new PartialFindResult<T>(contracts, count);
			});
		}

		public void RemoveFollowedTag(int userId, int tagId)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session =>
			{
				var link = session.Query<TagForUser>()
					.FirstOrDefault(a => a.Tag.Id == tagId && a.User.Id == userId);

				AuditLog($"removing {link}", session);

				if (link != null)
				{
					session.Delete(link);
				}
			});
		}

		public async Task RequestEmailVerification(int userId, string resetUrl)
		{
			await _repository.HandleTransactionAsync(async ctx =>
			{
				var user = await ctx.LoadAsync(userId);
				ctx.AuditLogger.SysLog($"requesting email verification ({user.Email})", user.Name);

				var subject = string.Format(UserAccountStrings.VerifyEmailSubject, _brandableStringsManager.Layout.SiteName);

				await SendEmailVerificationRequest(ctx, user, resetUrl, subject);
			});
		}

		/// <summary>
		/// Create password reset request and send it by email.
		/// The user is identified by username and email, and the account must be active.
		/// </summary>
		/// <param name="username">Username of the user for whom the reset was requested.</param>
		/// <param name="email">User email. Must belong to that user. Cannot be null or empty.</param>
		/// <param name="resetUrl">Password reset URL. Cannot be null or empty.</param>
		/// <exception cref="UserNotFoundException">If no active user matching the email was found.</exception>
		public async Task RequestPasswordReset(string username, string email, string resetUrl)
		{
			ParamIs.NotNullOrEmpty(() => username);
			ParamIs.NotNullOrEmpty(() => email);

			var lc = username.ToLowerInvariant();

			await _repository.HandleTransactionAsync(async ctx =>
			{
				var user = await ctx.Query().Where(u => u.Active && u.NameLC.Equals(lc) && email.Equals(u.Email)).VdbFirstOrDefaultAsync();

				if (user == null)
				{
					s_log.Info("User not found or not active: {0}", username);
					throw new UserNotFoundException();
				}

				var request = new PasswordResetRequest(user);
				await ctx.SaveAsync(request);

				var resetFullUrl = $"{resetUrl}/{request.Id}";
				var subject = UserAccountStrings.PasswordResetSubject;
				var body = string.Format(UserAccountStrings.PasswordResetBody, resetFullUrl);

				await _mailer.SendEmailAsync(request.User.Email, request.User.Name, subject, body);

				ctx.AuditLogger.SysLog($"requested password reset with ID {CryptoHelper.HashSHA1(request.Id.ToString())}", username);
			});
		}

		public ServerOnlyUserContract ResetPassword(Guid requestId, string password)
		{
			ParamIs.NotNullOrEmpty(() => password);

			return _repository.HandleTransaction(ctx =>
			{
				ctx.AuditLogger.SysLog($"resetting password with ID {CryptoHelper.HashSHA1(requestId.ToString())}");

				var request = ctx.OfType<PasswordResetRequest>().Load(requestId);

				if (!request.IsValid)
				{
					ctx.AuditLogger.SysLog("request has expired");
					throw new RequestNotValidException("Request has expired");
				}

				var user = request.User;

				ctx.AuditLogger.AuditLog("resetting password", user);

				user.UpdatePassword(password, PasswordHashAlgorithms.Default);

				ctx.Update(user);

				ctx.Delete(request);

				return new ServerOnlyUserContract(user);
			});
		}

		public async Task<TagUsageForApiContract[]> SaveAlbumTags(int albumId, TagBaseContract[] tags, bool onlyAdd)
		{
			return await new TagUsageQueries(PermissionContext).AddTags<Album, AlbumTagUsage>(
				albumId, tags, onlyAdd, _repository, _entryLinkFactory, _enumTranslations,
				album => album.Tags,
				(album, ctx) => new AlbumTagUsageFactory(ctx, album));
		}

		public async Task<TagUsageForApiContract[]> SaveArtistTags(int artistId, TagBaseContract[] tags, bool onlyAdd)
		{
			return await new TagUsageQueries(PermissionContext).AddTags<Artist, ArtistTagUsage>(
				artistId, tags, onlyAdd, _repository, _entryLinkFactory, _enumTranslations,
				artist => artist.Tags,
				(artist, ctx) => new ArtistTagUsageFactory(ctx, artist));
		}

		public async Task<TagUsageForApiContract[]> SaveEventTags(int eventId, TagBaseContract[] tags, bool onlyAdd)
		{
			return await new TagUsageQueries(PermissionContext).AddTags<ReleaseEvent, EventTagUsage>(
				eventId, tags, onlyAdd, _repository, _entryLinkFactory, _enumTranslations,
				releaseEvent => releaseEvent.Tags,
				(releaseEvent, ctx) => new EventTagUsageFactory(ctx, releaseEvent));
		}

		public async Task<TagUsageForApiContract[]> SaveEventSeriesTags(int seriesId, TagBaseContract[] tags, bool onlyAdd)
		{
			return await new TagUsageQueries(PermissionContext).AddTags<ReleaseEventSeries, EventSeriesTagUsage>(
				seriesId, tags, onlyAdd, _repository, _entryLinkFactory, _enumTranslations,
				releaseEvent => releaseEvent.Tags,
				(releaseEvent, ctx) => new EventSeriesTagUsageFactory(ctx, releaseEvent));
		}

		public async Task<TagUsageForApiContract[]> SaveSongListTags(int songListId, TagBaseContract[] tags, bool onlyAdd)
		{
			return await new TagUsageQueries(_permissionContext).AddTags<SongList, SongListTagUsage>(
				songListId, tags, onlyAdd, _repository, _entryLinkFactory, _enumTranslations,
				songList => songList.Tags,
				(songList, ctx) => new SongListTagUsageFactory(ctx, songList));
		}

		public async Task<TagUsageForApiContract[]> SaveSongTags(int songId, TagBaseContract[] tags, bool onlyAdd)
		{
			return await new TagUsageQueries(PermissionContext).AddTags<Song, SongTagUsage>(
				songId, tags, onlyAdd, _repository, _entryLinkFactory, _enumTranslations,
				song => song.Tags,
				(song, ctx) => new SongTagUsageFactory(ctx, song));
		}

		public async Task<UserMessageContract> SendMessage(UserMessageContract contract, string mySettingsUrl, string messagesUrl)
		{
			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return await _repository.HandleTransactionAsync(async session =>
			{
				var receiver = await session.Query().Where(u => u.Name.Equals(contract.Receiver.Name)).VdbFirstOrDefaultAsync();

				if (receiver == null)
					throw new UserNotFoundException();

				if (receiver.Options.Standalone)
				{
					throw new UserNotFoundException();
				}

				var sender = await session.LoadAsync(contract.Sender.Id);

				VerifyResourceAccess(sender);

				session.AuditLogger.SysLog("sending message from " + sender + " to " + receiver);

				if (sender.CreateDate >= DateTime.Now.AddDays(-7))
				{
					var cutoffTime = DateTime.Now.AddHours(-1);
					var sentMessageCount = await session.Query<UserMessage>()
						.Where(msg => msg.Sender.Id == sender.Id && msg.Created >= cutoffTime)
						.VdbCountAsync();
					s_log.Debug($"Sent messages count for sender {sender} is {sentMessageCount}");
					if (sentMessageCount > 10)
					{
						throw new RateLimitException("Too many messages");
					}
				}

				var messages = sender.SendMessage(receiver, contract.Subject, contract.Body, contract.HighPriority);

				if (receiver.EmailOptions == UserEmailOptions.PrivateMessagesFromAll
					|| (receiver.EmailOptions == UserEmailOptions.PrivateMessagesFromAdmins
						&& sender.EffectivePermissions.Has(PermissionToken.DesignatedStaff)))
				{
					await SendPrivateMessageNotification(mySettingsUrl, messagesUrl, messages.Received);
				}

				await session.SaveAsync(messages.Received);
				await session.SaveAsync(messages.Sent);

				return new UserMessageContract(messages.Received, _userIconFactory);
			});
		}

		public void SetAlbumFormatString(string formatString)
		{
			if (!PermissionContext.IsLoggedIn)
				return;

			_repository.HandleTransaction(ctx =>
			{
				var user = ctx.GetLoggedUser(PermissionContext);

				user.Options.AlbumFormatString = formatString;
				ctx.Update(user);
			});
		}

		private void VerifyEditUser(IDatabaseContext<User> ctx, IUserPermissionContext permissionContext, User user)
		{
			if (!EntryPermissionManager.CanEditUser(PermissionContext, user.GroupId))
			{
				var loggedUser = ctx.GetLoggedUser(PermissionContext);
				var msg = $"{loggedUser} (level {loggedUser.GroupId}) not allowed to edit {user} (level {user.GroupId})";
				s_log.Error(msg);
				throw new NotAllowedException(msg);
			}
		}

		public void SetUserToLimited(int userId, string reason, string hostname, bool createReport)
		{
			_repository.UpdateEntity<User, IDatabaseContext<User>>(userId, (session, user) =>
			{
				VerifyEditUser(session, PermissionContext, user);

				user.GroupId = UserGroupId.Limited;

				if (createReport)
				{
					CreateReport(session, user, UserReportType.RemovePermissions, hostname, reason);
				}

				var reasonText = !string.IsNullOrEmpty(reason) ? ": " + reason : string.Empty;
				var message = $"updated user {EntryLinkFactory.CreateEntryLink(user)} by removing edit permissions{reasonText}";
				session.AuditLogger.AuditLog(message, entryId: user.GlobalId);
			}, PermissionToken.RemoveEditPermission, PermissionContext, skipLog: true);
		}

		/// <summary>
		/// Updates user, including permissions, by admin/moderator.
		/// </summary>
		/// <param name="contract">User data. Cannot be null.</param>
		/// <exception cref="InvalidUserNameException">If the new username contains invalid characters.</exception>
		/// <exception cref="UserNameAlreadyExistsException">If the username is already in use by another user.</exception>
		/// <remarks>
		/// Requires the <see cref="PermissionToken.ManageUserPermissions"/> right.
		/// </remarks>
		public void UpdateUser(ServerOnlyUserWithPermissionsContract contract)
		{
			ParamIs.NotNull(() => contract);

			_repository.UpdateEntity<User, IDatabaseContext<User>>(contract.Id, (session, user) =>
			{
				VerifyEditUser(session, PermissionContext, user);

				if (EntryPermissionManager.CanEditGroupTo(PermissionContext, contract.GroupId))
				{
					user.GroupId = contract.GroupId;
				}

				if (EntryPermissionManager.CanEditAdditionalPermissions(PermissionContext))
				{
					user.AdditionalPermissions = new PermissionCollection(contract.AdditionalPermissions.Select(p => PermissionToken.GetById(p.Id)));
				}

				if (user.Name != contract.Name)
				{
					ValidateUsername(session, contract.Name, contract.Id);
					SetUsername(session, user, contract.Name);
				}

				var diff = OwnedArtistForUser.Sync(user.AllOwnedArtists, contract.OwnedArtistEntries, a => user.AddOwnedArtist(session.Load<Artist>(a.Artist.Id)));
				session.Sync(diff);
				user.VerifiedArtist = user.OwnedArtists.Any();

				user.Active = contract.Active;
				user.Email = contract.Email;

				if (!string.IsNullOrEmpty(contract.Email))
				{
					ValidateEmail(contract.Email);

					user.NormalizedEmail = MailAddressNormalizer.Normalize(contract.Email);
				}
				else
				{
					user.NormalizedEmail = string.Empty;
				}

				user.Options.Poisoned = contract.Poisoned;
				user.Options.Supporter = contract.Supporter;

				session.AuditLogger.AuditLog($"updated user {EntryLinkFactory.CreateEntryLink(user)}", entryId: user.GlobalId);
			}, PermissionToken.ManageUserPermissions, PermissionContext, skipLog: true);
		}

		private void SetUsername(IDatabaseContext<User> session, User user, string newName)
		{
			session.AuditLogger.AuditLog($"changed username of {EntryLinkFactory.CreateEntryLink(user)} to '{newName}'");

			var usernameEntry = new OldUsername(user, user.Name);
			session.Save(usernameEntry);
			user.OldUsernames.Add(usernameEntry);

			user.Name = newName;
			user.NameLC = newName.ToLowerInvariant();
		}

		public void UpdateAlbumForUser(int userId, int albumId, PurchaseStatus status,
			MediaType mediaType, int rating)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			_repository.HandleTransaction(session =>
			{
				var albumForUser = session.OfType<AlbumForUser>().Query()
					.FirstOrDefault(a => a.Album.Id == albumId && a.User.Id == userId);

				// Delete
				if (albumForUser != null && status == PurchaseStatus.Nothing && rating == 0)
				{
					session.AuditLogger.AuditLog($"deleting {_entryLinkFactory.CreateEntryLink(albumForUser.Album)} from collection");

					NHibernateUtil.Initialize(albumForUser.Album.CoverPictureData);

					albumForUser.Delete();
					session.Delete(albumForUser);
					session.Update(albumForUser.Album);

					// Add
				}
				else if (albumForUser == null && (status != PurchaseStatus.Nothing || rating != 0))
				{
					var user = session.Load(userId);
					var album = session.OfType<Album>().Load(albumId);

					NHibernateUtil.Initialize(album.CoverPictureData);
					albumForUser = user.AddAlbum(album, status, mediaType, rating);
					session.Save(albumForUser);
					session.Update(album);

					session.AuditLogger.AuditLog($"added {_entryLinkFactory.CreateEntryLink(album)} to collection");

					// Update
				}
				else if (albumForUser != null)
				{
					albumForUser.MediaType = mediaType;
					albumForUser.PurchaseStatus = status;
					session.Update(albumForUser);

					if (albumForUser.Rating != rating)
					{
						albumForUser.Rating = rating;
						albumForUser.Album.UpdateRatingTotals();
						NHibernateUtil.Initialize(albumForUser.Album.CoverPictureData);
						session.Update(albumForUser.Album);
					}

					session.AuditLogger.AuditLog($"updated {_entryLinkFactory.CreateEntryLink(albumForUser.Album)} in collection");
				}
			});
		}

		public void UpdateArtistSubscriptionForCurrentUser(int artistId, bool? emailNotifications, bool? siteNotifications)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			var userId = PermissionContext.LoggedUserId;

			_repository.HandleTransaction(ctx =>
			{
				var subscription = ctx.OfType<ArtistForUser>().Query().FirstOrDefault(u => u.User.Id == userId && u.Artist.Id == artistId);

				// No subscription found. Shouldn't happen, but could also be handled so that a new subscription is added.
				if (subscription == null)
					return;

				if (emailNotifications.HasValue)
					subscription.EmailNotifications = emailNotifications.Value;

				if (siteNotifications.HasValue)
					subscription.SiteNotifications = siteNotifications.Value;

				ctx.Update(subscription);

				ctx.AuditLogger.SysLog($"updated artist subscription for {subscription.Artist}.");
			});
		}

		public void UpdateEventForUser(int userId, int eventId, UserEventRelationshipType? relationshipType)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			if (userId != _permissionContext.LoggedUserId)
			{
				throw new NotAllowedException("Only allowed for self");
			}

			_repository.HandleTransaction(ctx =>
			{
				var subscription = ctx.Query<EventForUser>().FirstOrDefault(e => e.User.Id == userId && e.ReleaseEvent.Id == eventId);

				if (subscription != null)
				{
					if (relationshipType == null)
					{
						subscription.OnDeleted();
						ctx.Delete(subscription);
						ctx.AuditLogger.AuditLog($"removed association to {subscription.ReleaseEvent}.");
					}
					else if (relationshipType != subscription.RelationshipType)
					{
						subscription.RelationshipType = relationshipType.Value;
						ctx.Update(subscription);
						ctx.AuditLogger.AuditLog($"updated association to {subscription.ReleaseEvent}.");
					}
				}
				else if (relationshipType.HasValue)
				{
					subscription = ctx.Load<User>(userId).AddEvent(ctx.Load<ReleaseEvent>(eventId), relationshipType.Value);
					ctx.Save(subscription);
					ctx.AuditLogger.AuditLog($"created association to {subscription.ReleaseEvent}.");
				}
			});
		}

		/// <summary>
		/// Updates user's settings (usually by the user themselves from my settings page).
		/// </summary>
		/// <param name="contract">New properties. Cannot be null.</param>
		/// <returns>Updated user data. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="InvalidPasswordException">If password change was attempted and the old password was incorrect.</exception>
		/// <exception cref="InvalidUserNameException">If the new username is invalid.</exception>
		/// <exception cref="UserNameAlreadyExistsException">If the username was already taken by another user.</exception>
		/// <exception cref="UserNameTooSoonException">If the cooldown for changing username has not expired.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken by another user.</exception>
		public ServerOnlyUserWithPermissionsContract UpdateUserSettings(ServerOnlyUpdateUserSettingsContract contract)
		{
			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return _repository.HandleTransaction(ctx =>
			{
				var user = ctx.Load(contract.Id);

				ctx.AuditLogger.SysLog($"Updating settings for {user}");

				PermissionContext.VerifyResourceAccess(user);

				if (!string.IsNullOrEmpty(contract.NewPass))
				{
					var oldAlgorithm = PasswordHashAlgorithms.Get(user.PasswordHashAlgorithm);
					var oldHashed = (!string.IsNullOrEmpty(user.Password) ? oldAlgorithm.HashPassword(contract.OldPass, user.Salt, user.NameLC) : string.Empty);

					if (user.Password != oldHashed)
						throw new InvalidPasswordException();

					user.UpdatePassword(contract.NewPass, PasswordHashAlgorithms.Default);
				}

				if (!string.IsNullOrEmpty(contract.Name) && !string.Equals(contract.Name, user.Name, StringComparison.InvariantCultureIgnoreCase))
				{
					ValidateUsername(ctx, contract.Name, contract.Id);

					if (!user.CanChangeName)
					{
						throw new UserNameTooSoonException();
					}

					SetUsername(ctx, user, contract.Name);
				}

				var email = contract.Email;

				if (!string.IsNullOrEmpty(email))
				{
					ValidateEmail(email);

					var normalizedEmail = MailAddressNormalizer.Normalize(email);
					var existing = ctx.Query().FirstOrDefault(u => u.Active && u.Id != user.Id && u.NormalizedEmail == normalizedEmail);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();
				}

				user.Options.AboutMe = contract.AboutMe;
				user.AnonymousActivity = contract.AnonymousActivity;
				user.Culture = contract.Culture;
				user.DefaultLanguageSelection = contract.DefaultLanguageSelection;
				user.EmailOptions = contract.EmailOptions;
				user.Language = new OptionalCultureCode(contract.Language);
				user.Options.Location = contract.Location;
				user.PreferredVideoService = contract.PreferredVideoService;
				user.Options.PublicAlbumCollection = contract.PublicAlbumCollection;
				user.Options.PublicRatings = contract.PublicRatings;
				user.Options.Stylesheet = contract.Stylesheet;
				user.Options.UnreadNotificationsToKeep = contract.UnreadNotificationsToKeep;
				user.SetEmail(email);

				var validWebLinks = contract.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(user.WebLinks, validWebLinks, user);
				ctx.OfType<UserWebLink>().Sync(webLinkDiff);

				var knownLanguagesDiff = CollectionHelper.Sync(user.KnownLanguages, contract.KnownLanguages.Distinct(l => l.CultureCode), (l, l2) => l.CultureCode.Equals(l2.CultureCode) && l.Proficiency == l2.Proficiency, l => user.AddKnownLanguage(l.CultureCode, l.Proficiency));
				ctx.Sync(knownLanguagesDiff);

				ctx.Update(user);

				ctx.AuditLogger.AuditLog("updated settings");

				return new ServerOnlyUserWithPermissionsContract(user, PermissionContext.LanguagePreference);
			});
		}

		public void UpdateUserSetting(IUserSetting setting)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			_repository.HandleTransaction(session =>
			{
				var user = session.GetLoggedUser(PermissionContext);

				setting.UpdateUser(user);

				session.Update(user);
			});
		}

		/// <summary>
		/// Verifies user email.
		/// Logged user must be the same as the user being verified.
		/// </summary>
		/// <param name="requestId">ID of the verification request.</param>
		/// <returns>True if the request was found and was processed. False if the request was not found (already used).</returns>
		public bool VerifyEmail(Guid requestId)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return _repository.HandleTransaction(ctx =>
			{
				var request = ctx.OfType<PasswordResetRequest>().Get(requestId);

				if (request == null || !request.IsValid)
					return false;

				var user = request.User;
				if (!user.Equals(PermissionContext.LoggedUser))
					throw new RequestNotValidException("Email verification request not valid for this user");

				if (!user.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase))
				{
					s_log.Info($"Email {request.Email} not valid for {user}");
					throw new RequestNotValidException("Email verification request not valid for this user");

					/*
					// Update email from request in case the user hasn't saved the new email yet.
					user.Email = request.Email;
					*/
				}

				user.Options.EmailVerified = true;
				ctx.Update(user);

				ctx.Delete(request);

				ctx.AuditLogger.AuditLog($"verified email ({user.Email})");

				return true;
			});
		}

		public UserForApiContract GetOne(int id, UserOptionalFields fields = UserOptionalFields.None) => HandleQuery(ctx => new UserForApiContract(ctx.Load(id), _userIconFactory, fields));

		public void PostEditComment(int commentId, CommentForApiContract contract) => HandleTransaction(ctx => Comments(ctx).Update(commentId, contract));

		public bool IsNotification(int messageId, ServerOnlyUserWithPermissionsContract user) => HandleQuery(ctx =>
		{
			return ctx.Query<UserMessage>()
				.Any(m => m.Id == messageId && m.User.Id == user.Id && m.Inbox == UserInboxType.Notifications);
		});

		public ArtistForUserForApiContract GetArtistForUser(int userId, int artistId) => HandleQuery(ctx =>
		{
			var artistForUser = ctx.OfType<ArtistForUser>().Query().FirstOrDefault(s => s.Artist.Id == artistId && s.User.Id == userId);
			return new ArtistForUserForApiContract(artistForUser, LanguagePreference, _entryImagePersister, ArtistOptionalFields.None);
		});

		public AlbumForUserForApiContract GetAlbumForUser(int userId, int albumId) => HandleQuery(ctx =>
		{
			var albumForUser = ctx.OfType<AlbumForUser>().Query().FirstOrDefault(s => s.Album.Id == albumId && s.User.Id == userId);
			return new AlbumForUserForApiContract(albumForUser, LanguagePreference, _entryImagePersister, AlbumOptionalFields.None, shouldShowCollectionStatus: true);
		});
	}

	public class AlbumTagUsageFactory : ITagUsageFactory<AlbumTagUsage>
	{
		private readonly Album _album;
		private readonly IDatabaseContext _session;

		public AlbumTagUsageFactory(IDatabaseContext session, Album album)
		{
			_session = session;
			_album = album;
		}

		public AlbumTagUsage CreateTagUsage(Tag tag)
		{
			var usage = new AlbumTagUsage(_album, tag);
			_session.Save(usage);

			return usage;
		}

		public AlbumTagUsage CreateTagUsage(Tag tag, AlbumTagUsage oldUsage)
		{
			var usage = new AlbumTagUsage(oldUsage.Entry, tag);
			_session.Save(usage);

			return usage;
		}
	}

	public class ArtistTagUsageFactory : ITagUsageFactory<ArtistTagUsage>
	{
		private readonly Artist _artist;
		private readonly IDatabaseContext _session;

		public ArtistTagUsageFactory(IDatabaseContext session, Artist artist)
		{
			_session = session;
			_artist = artist;
		}

		public ArtistTagUsage CreateTagUsage(Tag tag)
		{
			var usage = new ArtistTagUsage(_artist, tag);
			_session.Save(usage);

			return usage;
		}

		public ArtistTagUsage CreateTagUsage(Tag tag, ArtistTagUsage oldUsage)
		{
			var usage = new ArtistTagUsage(oldUsage.Entry, tag);
			_session.Save(usage);

			return usage;
		}
	}

	public class EventTagUsageFactory : ITagUsageFactory<EventTagUsage>
	{
		private readonly ReleaseEvent _releaseEvent;
		private readonly IDatabaseContext _ctx;

		public EventTagUsageFactory(IDatabaseContext ctx, ReleaseEvent releaseEvent)
		{
			_ctx = ctx;
			_releaseEvent = releaseEvent;
		}

		public EventTagUsage CreateTagUsage(Tag tag)
		{
			var usage = new EventTagUsage(_releaseEvent, tag);
			_ctx.Save(usage);

			return usage;
		}

		public EventTagUsage CreateTagUsage(Tag tag, EventTagUsage oldUsage)
		{
			var usage = new EventTagUsage(oldUsage.Entry, tag);
			_ctx.Save(usage);

			return usage;
		}
	}

	public class EventSeriesTagUsageFactory : ITagUsageFactory<EventSeriesTagUsage>
	{
		private readonly ReleaseEventSeries _releaseEvent;
		private readonly IDatabaseContext _ctx;

		public EventSeriesTagUsageFactory(IDatabaseContext ctx, ReleaseEventSeries releaseEvent)
		{
			_ctx = ctx;
			_releaseEvent = releaseEvent;
		}

		public EventSeriesTagUsage CreateTagUsage(Tag tag)
		{
			var usage = new EventSeriesTagUsage(_releaseEvent, tag);
			_ctx.Save(usage);

			return usage;
		}

		public EventSeriesTagUsage CreateTagUsage(Tag tag, EventSeriesTagUsage oldUsage)
		{
			var usage = new EventSeriesTagUsage(oldUsage.Entry, tag);
			_ctx.Save(usage);

			return usage;
		}
	}

	public class SongListTagUsageFactory : ITagUsageFactory<SongListTagUsage>
	{
		private readonly SongList _songList;
		private readonly IDatabaseContext _ctx;

		public SongListTagUsageFactory(IDatabaseContext ctx, SongList songList)
		{
			_ctx = ctx;
			_songList = songList;
		}

		public SongListTagUsage CreateTagUsage(Tag tag)
		{
			var usage = new SongListTagUsage(_songList, tag);
			_ctx.Save(usage);

			return usage;
		}

		public SongListTagUsage CreateTagUsage(Tag tag, SongListTagUsage oldUsage)
		{
			var usage = new SongListTagUsage(oldUsage.Entry, tag);
			_ctx.Save(usage);

			return usage;
		}
	}

	public class SongTagUsageFactory : ITagUsageFactory<SongTagUsage>
	{
		private readonly Song _song;
		private readonly IDatabaseContext _ctx;

		public SongTagUsageFactory(IDatabaseContext ctx, Song song)
		{
			_ctx = ctx;
			_song = song;
		}

		public SongTagUsage CreateTagUsage(Tag tag)
		{
			var usage = new SongTagUsage(_song, tag);
			_ctx.Save(usage);

			return usage;
		}

		public SongTagUsage CreateTagUsage(Tag tag, SongTagUsage oldUsage)
		{
			var usage = new SongTagUsage(oldUsage.Entry, tag);
			_ctx.Save(usage);

			return usage;
		}
	}
}