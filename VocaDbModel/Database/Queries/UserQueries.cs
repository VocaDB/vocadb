using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using NHibernate;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.SongLists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
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
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Service.Security.StopForumSpam;

namespace VocaDb.Model.Database.Queries {

	/// <summary>
	/// Database queries related to <see cref="User"/>.
	/// </summary>
	public class UserQueries : QueriesBase<IUserRepository, User> {

		/// <summary>
		/// Cached user stats, these might be slightly inaccurate.
		/// Most of the values are just "fun" statistical information, some of them aren't even displayed directly.
		/// </summary>
		class CachedUserStats {

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

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly BrandableStringsManager brandableStringsManager;
		private readonly ObjectCache cache;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryImagePersisterOld entryImagePersister;
		private readonly IUserMessageMailer mailer;
		private readonly IStopForumSpamClient sfsClient;
		private readonly IUserIconFactory userIconFactory;

		public IEntryLinkFactory EntryLinkFactory {
			get { return entryLinkFactory; }
		}

		private IQueryable<User> AddOrder(IQueryable<User> query, UserSortRule sortRule) {

			switch (sortRule) {
				case UserSortRule.Name:
					return query.OrderBy(u => u.Name);
				case UserSortRule.RegisterDate:
					return query.OrderBy(u => u.CreateDate);
				case UserSortRule.Group:
					return query
						.OrderBy(u => u.GroupId)
						.ThenBy(u => u.Name);
			}

			return query;

		}

		private int[] GetFavoriteTagIds(IDatabaseContext<User> ctx, User user) {

			/* 
				Note: There have been some performance problems with this query.
				There's a DB index for both AllSongTagUsages (Tag-Song) and UserFavorites (Song-User).
				Attempting to do the sorting by count in memory.
			*/

			var tags = ctx
				.Query<Tag>()
				.Where(t => t.CategoryName != TagCommonCategoryNames.Lyrics && t.CategoryName != TagCommonCategoryNames.Distribution)
				.Select(t => new {
					Id = t.Id,
					Count = t.AllSongTagUsages.Count(u => u.Song.UserFavorites.Any(f => f.User.Id == user.Id))
				})
				.ToArray()
				.Where(t => t.Count > 0)
				.OrderByDescending(t => t.Count)
				.Take(8)
				.Select(t => t.Id)
				.ToArray();

			return tags;

		}

		private CachedUserStats GetAlbumCounts(IDatabaseContext<User> ctx, User user) {

			return ctx
				.Query()
				.Where(u => u.Id == user.Id)
				.Select(u => new CachedUserStats {
					AlbumCollectionCount = u.AllAlbums.Count(a => !a.Album.Deleted),
					OwnedAlbumCount = u.AllAlbums.Count(a => !a.Album.Deleted && a.PurchaseStatus == PurchaseStatus.Owned),
					RatedAlbumCount = u.AllAlbums.Count(a => !a.Album.Deleted && a.Rating != 0),
				})
				.First();

		}

		private int GetArtistCount(IDatabaseContext<User> ctx, User user) {

			return ctx.Query<ArtistForUser>().Count(u => u.User.Id == user.Id && !u.Artist.Deleted);

		}

		private int GetSongCount(IDatabaseContext<User> ctx, User user) {

			return ctx.Query<FavoriteSongForUser>().Count(u => u.User.Id == user.Id && !u.Song.Deleted);

		}

		private CachedUserStats GetCachedUserStats(IDatabaseContext<User> ctx, User user) {
			
			var key = string.Format("CachedUserStats.{0}", user.Id);
			return cache.GetOrInsert(key, CachePolicy.AbsoluteExpiration(4), () => {

				var stats = new CachedUserStats();

				try {
					var albumCounts = GetAlbumCounts(ctx, user);
					stats.AlbumCollectionCount = albumCounts.AlbumCollectionCount;
					stats.OwnedAlbumCount = albumCounts.OwnedAlbumCount;
					stats.RatedAlbumCount = albumCounts.RatedAlbumCount;

					stats.ArtistCount = GetArtistCount(ctx, user);
					stats.FavoriteSongCount = GetSongCount(ctx, user);

					stats.CommentCount
						= ctx.Query<AlbumComment>().Count(c => c.Author.Id == user.Id)
						+ ctx.Query<ArtistComment>().Count(c => c.Author.Id == user.Id)
						+ ctx.Query<SongComment>().Count(c => c.Author.Id == user.Id);

					stats.EditCount = ctx.Query<ActivityEntry>().Count(c => c.Author.Id == user.Id);

					stats.SubmitCount = ctx.Query<ActivityEntry>().Count(c => c.Author.Id == user.Id && c.EditEvent == EntryEditEvent.Created);

					stats.TagVotes
						= ctx.Query<SongTagVote>().Count(t => t.User.Id == user.Id)
						+ ctx.Query<AlbumTagVote>().Count(t => t.User.Id == user.Id)
						+ ctx.Query<ArtistTagVote>().Count(t => t.User.Id == user.Id);

					stats.FavoriteTags = GetFavoriteTagIds(ctx, user);
				} catch (HibernateException x) {
					// TODO: Loading of stats timeouts sometimes. Since they're not essential we can accept returning only partial stats.
					// However, this should be fixed by tuning the queries further.
					log.Error(x, "Unable to load user stats");
				}

				return stats;

			});

		}

		private void SendPrivateMessageNotification(string mySettingsUrl, string messagesUrl, UserMessage message) {

			ParamIs.NotNull(() => message);

			var subject = string.Format("New private message from {0}", message.Sender.Name);
			var body = string.Format(
				"You have received a message from {0}. " +
				"You can view your messages at {1}." +
				"\n\n" +
				"If you do not wish to receive more email notifications such as this, you can adjust your settings at {2}.",
				message.Sender.Name, messagesUrl, mySettingsUrl);

			mailer.SendEmail(message.Receiver.Email, message.Receiver.Name, subject, body);

		}

		private UserDetailsContract GetUserDetails(IDatabaseContext<User> session, User user) {

			var details = new UserDetailsContract(user, PermissionContext);

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
				.Select(c => new AlbumContract(c, LanguagePreference))
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
				.Where(c => c.EntryForComment == user).OrderByDescending(c => c.Created).Take(3)
				.ToArray()
				.Select(c => new CommentForApiContract(c, userIconFactory)).ToArray();

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

			// If the user is viewing their own profile, check for possible producer account.
			// Skip users who are not active, limited or are already verified artists.
			if (user.Active && user.GroupId >= UserGroupId.Regular && user.GroupId <= UserGroupId.Trusted && user.Equals(PermissionContext.LoggedUser) && !user.VerifiedArtist) {
				
				// Scan by Twitter account name and entry name.
				var twitterUrl = !string.IsNullOrEmpty(user.Options.TwitterName) ? string.Format("https://twitter.com/{0}", user.Options.TwitterName) : null;
				var producerTypes = new[] { ArtistType.Producer, ArtistType.Animator, ArtistType.Illustrator };

				details.PossibleProducerAccount = session.Query<Artist>().Any(a => 
					!a.Deleted
					&& producerTypes.Contains(a.ArtistType) 
					&& (a.Names.Names.Any(n => n.Value == user.Name) 
						|| (twitterUrl != null && a.WebLinks.Any(l => l.Url == twitterUrl)))
					&& !a.OwnerUsers.Any());

			}

			return details;

		}

		private bool IsPoisoned(IDatabaseContext<User> ctx, string lcUserName) {

			return ctx.OfType<UserOptions>().Query().Any(o => o.Poisoned && o.User.NameLC == lcUserName);

		}

		private string MakeGeoIpToolLink(string hostname) {

			return string.Format("<a href='http://www.geoiptool.com/?IP={0}'>{0}</a>", hostname);

		}

		private void SendEmailVerificationRequest(IDatabaseContext<User> ctx, User user, string resetUrl, string subject) {
			
			var request = new PasswordResetRequest(user);
			ctx.Save(request);

			var body = string.Format(UserAccountStrings.VerifyEmailBody, brandableStringsManager.Layout.SiteName, resetUrl, request.Id);

			mailer.SendEmail(request.User.Email, request.User.Name, subject, body);

		}

		private void ValidateEmail(string email) {
			
			try {
				new MailAddress(email);
			} catch (FormatException x) {
				throw new InvalidEmailFormatException("Email format is invalid", x);
			}

		}

		public UserQueries(IUserRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IStopForumSpamClient sfsClient,
			IUserMessageMailer mailer, IUserIconFactory userIconFactory, IEntryImagePersisterOld entryImagePersister, ObjectCache cache, 
			BrandableStringsManager brandableStringsManager)
			: base(repository, permissionContext) {

			ParamIs.NotNull(() => repository);
			ParamIs.NotNull(() => permissionContext);
			ParamIs.NotNull(() => entryLinkFactory);
			ParamIs.NotNull(() => sfsClient);
			ParamIs.NotNull(() => mailer);

			this.entryLinkFactory = entryLinkFactory;
			this.sfsClient = sfsClient;
			this.mailer = mailer;
			this.userIconFactory = userIconFactory;
			this.entryImagePersister = entryImagePersister;
			this.cache = cache;
			this.brandableStringsManager = brandableStringsManager;

		}

		public CommentQueries<UserComment, User> Comments(IDatabaseContext<User> ctx) {
			return new CommentQueries<UserComment, User>(ctx.OfType<UserComment>(), PermissionContext, userIconFactory, entryLinkFactory);
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
		public LoginResult CheckAuthentication(string name, string pass, string hostname, string culture, bool delayFailedLogin) {

			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(pass))
				return LoginResult.CreateError(LoginError.InvalidPassword);

			var lc = name.ToLowerInvariant();

			return repository.HandleTransaction(ctx => {

				if (IsPoisoned(ctx, lc)) {
					ctx.AuditLogger.SysLog(string.Format("failed login from {0} - account is poisoned.", MakeGeoIpToolLink(hostname)), name);
					return LoginResult.CreateError(LoginError.AccountPoisoned);
				}

				// Attempt to find user by either lowercase username.
				var user = ctx.Query().FirstOrDefault(u => u.Active && (u.NameLC == lc || (u.Options.EmailVerified && u.Email == name)));

				if (user == null) {
					ctx.AuditLogger.AuditLog(string.Format("failed login from {0} - no user.", MakeGeoIpToolLink(hostname)), name);
					if (delayFailedLogin)
						Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.NotFound);
				}

				var algorithm = PasswordHashAlgorithms.Get(user.PasswordHashAlgorithm);

				// Attempt to verify password.				
				var hashed = algorithm.HashPassword(pass, user.Salt, user.NameLC);

				if (user.Password != hashed) {
					ctx.AuditLogger.AuditLog(string.Format("failed login from {0} - wrong password.", MakeGeoIpToolLink(hostname)), name);
					if (delayFailedLogin)
						Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.InvalidPassword);
				}
				
				// Login attempt successful.
				ctx.AuditLogger.AuditLog(string.Format("logged in from {0} with '{1}'.", MakeGeoIpToolLink(hostname), name), user);

				user.UpdatePassword(pass, PasswordHashAlgorithms.Default);
				user.UpdateLastLogin(hostname, culture);
				ctx.Update(user);

				return LoginResult.CreateSuccess(new UserContract(user));

			});

		}

		public bool CheckPasswordResetRequest(Guid requestId) {

			var cutoff = DateTime.Now - PasswordResetRequest.ExpirationTime;

			return repository.HandleQuery(ctx => ctx.OfType<PasswordResetRequest>().Query().Any(r => r.Id == requestId && r.Created >= cutoff));

		}

		public CommentForApiContract CreateComment(int userId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return repository.HandleTransaction(ctx => {

				var user = ctx.Load(userId);
				var agent = ctx.CreateAgentLoginData(PermissionContext);

				ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'",
					EntryLinkFactory.CreateEntryLink(user),
					HttpUtility.HtmlEncode(message)), agent.User);

				var comment = user.CreateComment(message, agent);
				ctx.OfType<UserComment>().Save(comment);

				var commentMsg = comment.Message.Truncate(200);
				var notificationMsg = string.Format("{0} posted a comment on your profile.\n\n{1}", agent.Name, commentMsg);
				var notification = new UserMessage(user, "Comment posted on your profile", notificationMsg, false);
				ctx.OfType<UserMessage>().Save(notification);

				return new CommentForApiContract(comment, userIconFactory);

			});

		}

		/// <summary>
		/// Disconnects Twitter account for the currently logged in user.
		/// Twitter account can NOT be disconnected if the user has not set a VocaDB password.
		/// </summary>
		/// <exception cref="NoPasswordException">If the user has not set a password.</exception>
		public void DisconnectTwitter() {
		
			PermissionContext.VerifyLogin();

			repository.HandleTransaction(ctx => {
				
				var user = ctx.GetLoggedUser(PermissionContext);

				user.ClearTwitter();

				ctx.AuditLogger.AuditLog("disconnected twitter");

			});
	
		}

		private string GetSFSCheckStr(SFSResponseContract result) {

			if (result == null)
				return "error";

			switch (result.Conclusion) {
				case SFSCheckResultType.Malicious:
					return string.Format("Malicious ({0} % confidence)", result.Confidence);
				case SFSCheckResultType.Uncertain:
					return string.Format("Uncertain ({0} % confidence)", result.Confidence);
				default:
					return "Ok";
			}

		}

		/// <summary>
		/// Clears all rated albums and songs by a user.
		/// Also updates rating totals.
		/// 
		/// Staff members cannot be cleared.
		/// </summary>
		/// <param name="id">User Id.</param>
		public void ClearRatings(int id) {
			
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			repository.HandleTransaction(ctx => {
				
				var user = ctx.Load(id);

				if (!user.CanBeDisabled)
					throw new NotAllowedException("This user account cannot be cleared.");

				ctx.AuditLogger.AuditLog(string.Format("clearing ratings by {0}", user));

				while (user.AllAlbums.Any()) {
					var albumLink = user.AllAlbums[0];
					albumLink.Delete();		
					ctx.Delete(albumLink);
					ctx.Update(albumLink.Album); // Update album ratings
				}

				while (user.FavoriteSongs.Any()) {
					var songLink = user.FavoriteSongs[0];
					songLink.Delete();
					ctx.Delete(songLink);
					ctx.Update(songLink.Song); // Update song ratings
				}

				while (user.AllArtists.Any()) {
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
		/// <param name="hostname">Host name where the registration is from.</param>
		/// <param name="culture">User culture name. Can be empty.</param>
		/// <param name="timeSpan">Time in which the user filled the registration form.</param>
		/// <param name="softbannedIPs">List of application's soft-banned IPs. Soft-banned IPs are cleared when the application restarts.</param>
		/// <param name="verifyEmailUrl">Email verification URL. Cannot be null.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		/// <exception cref="TooFastRegistrationException">If the user registered too fast.</exception>
		public UserContract Create(string name, string pass, string email, string hostname, 
			string culture,
			TimeSpan timeSpan,
			HostCollection softbannedIPs, string verifyEmailUrl) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);
			ParamIs.NotNull(() => email);

			if (timeSpan < TimeSpan.FromSeconds(5)) {

				log.Warn("Suspicious registration form fill time ({0}) from {1}.", timeSpan, hostname);

				if (timeSpan < TimeSpan.FromSeconds(2)) {
					softbannedIPs.Add(hostname);
				}

				throw new TooFastRegistrationException();

			}

			return repository.HandleTransaction(ctx => {

				// Verification
				var lc = name.ToLowerInvariant();
				var existing = ctx.Query().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email)) {

					ValidateEmail(email);

					existing = ctx.Query().FirstOrDefault(u => u.Active && u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();

				}

				// All ok, create user
				var sfsCheckResult = sfsClient.CallApi(hostname);
				var sfsStr = GetSFSCheckStr(sfsCheckResult);

				var user = new User(name, pass, email, PasswordHashAlgorithms.Default);
				user.UpdateLastLogin(hostname, culture);
				ctx.Save(user);

				if (sfsCheckResult != null && sfsCheckResult.Conclusion == SFSCheckResultType.Malicious) {

					var report = new UserReport(user, UserReportType.MaliciousIP, null, hostname, 
						string.Format("Confidence {0} %, Frequency {1}, Last seen {2}.", 
						sfsCheckResult.Confidence, sfsCheckResult.Frequency, sfsCheckResult.LastSeen.ToShortDateString()));

					ctx.OfType<UserReport>().Save(report);

					user.GroupId = UserGroupId.Limited;
					ctx.Update(user);

				}

				if (!string.IsNullOrEmpty(user.Email)) {
					var subject = string.Format(UserAccountStrings.AccountCreatedSubject, brandableStringsManager.Layout.SiteName);
					SendEmailVerificationRequest(ctx, user, verifyEmailUrl, subject);					
				}

				ctx.AuditLogger.AuditLog(string.Format("registered from {0} in {1} (SFS check {2}).", MakeGeoIpToolLink(hostname), timeSpan, sfsStr), user);

				return new UserContract(user);

			});

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
		public UserContract CreateTwitter(string authToken, string name, string email, int twitterId, string twitterName, string hostname, string culture) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => email);

			return repository.HandleTransaction(ctx => {

				var lc = name.ToLowerInvariant();
				var existing = ctx.Query().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email)) {

					ValidateEmail(email);

					existing = ctx.Query().FirstOrDefault(u => u.Active && u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();

				}

				var user = new User(name, string.Empty, email, PasswordHashAlgorithms.Default);
				user.Options.TwitterId = twitterId;
				user.Options.TwitterName = twitterName;
				user.Options.TwitterOAuthToken = authToken;
				user.UpdateLastLogin(hostname, culture);
				ctx.Save(user);

				ctx.AuditLogger.AuditLog(string.Format("registered from {0} using Twitter name '{1}'.", MakeGeoIpToolLink(hostname), twitterName), user);

				return new UserContract(user);

			});

		}

		public void DisableUser(int userId) {
			
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			repository.HandleTransaction(ctx => {

				var user = ctx.Load(userId);

				if (!user.CanBeDisabled)
					throw new NotAllowedException("This user account cannot be disabled.");

				user.Active = false;

				ctx.AuditLogger.AuditLog(string.Format("disabled {0}.", EntryLinkFactory.CreateEntryLink(user)));

				ctx.Update(user);

			});

		}

		public string[] FindNames(SearchTextQuery textQuery, int maxResults) {

			if (textQuery.IsEmpty)
				return new string[] { };

			return HandleQuery(session => {

				var names = session.Query<User>()
					.WhereHasName(textQuery)
					.Select(n => n.Name)
					.OrderBy(n => n)
					.Take(maxResults)
					.ToArray();

				return names;

			});

		}

		public PartialFindResult<T> GetAlbumCollection<T>(AlbumCollectionQueryParams queryParams, Func<AlbumForUser, bool, T> fac) {

			ParamIs.NotNull(() => queryParams);

			return HandleQuery(session => {

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
					.WhereAlbumMatchFilters(queryParams.AdvancedFilters);

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

		public ArtistContract[] GetArtists(int userId) {

			return HandleQuery(session =>
				session.Load(userId)
					.Artists
					.Select(a => new ArtistContract(a.Artist, PermissionContext.LanguagePreference))
					.OrderBy(s => s.Name)
					.ToArray());

		}

		public PartialFindResult<T> GetArtists<T>(FollowedArtistQueryParams queryParams, Func<ArtistForUser, T> fac) {
		
			var paging = queryParams.Paging;
	
			return HandleQuery(ctx => {
				
				var query = ctx.OfType<ArtistForUser>().Query()
					.Where(a => !a.Artist.Deleted && a.User.Id == queryParams.UserId)
					.WhereArtistHasName(queryParams.TextQuery)
					.WhereArtistHasType(queryParams.ArtistType);

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

		public PartialFindResult<CommentForApiContract> GetProfileComments(int userId, PagingProperties paging) {
			
			return HandleQuery(ctx => {
				
				var query = ctx.OfType<UserComment>().Query()
					.Where(c => c.EntryForComment.Id == userId);

				var comments = query
					.OrderByDescending(c => c.Created)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray()
					.Select(c => new CommentForApiContract(c, userIconFactory))
					.ToArray();

				var count = (paging.GetTotalCount ? query.Count() : 0);

				return new PartialFindResult<CommentForApiContract>(comments, count);

			});

		}

		public PartialFindResult<T> GetRatedSongs<T>(RatedSongQueryParams queryParams, Func<FavoriteSongForUser, T> fac) {

			ParamIs.NotNull(() => queryParams);

			return HandleQuery(session => {

				// Apply initial filter
				var q = session.OfType<FavoriteSongForUser>().Query()
					.Where(a => !a.Song.Deleted && a.User.Id == queryParams.UserId)
					.WhereChildHasName(queryParams.TextQuery)
					.WhereSongHasArtists(queryParams.ArtistIds, queryParams.ChildVoicebanks)
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

		private void AddCount(Dictionary<int, int> genresDict, int? parentTag, int count) {
			
			if (parentTag == null || parentTag == 0)
				return;

			if (genresDict.ContainsKey(parentTag.Value))
				genresDict[parentTag.Value] += count;
			else
				genresDict.Add(parentTag.Value, count);

		}

		public Tuple<string, int>[] GetRatingsByGenre(int userId) {
			
			return repository.HandleQuery(ctx => {
				
				var genres = ctx
					.OfType<SongTagUsage>()
					.Query()
					.Where(u => u.Song.UserFavorites.Any(f => f.User.Id == userId) && u.Tag.CategoryName == TagCommonCategoryNames.Genres)
					// NH doesn't support ? operator, instead casting ID to nullable works
					.GroupBy(s => new { TagId = s.Tag.Id, Parent = (int?)s.Tag.Parent.Id })
					.Select(g => new {
						TagId = g.Key.TagId,
						Parent = g.Key.Parent,
						Count = g.Count()
					})
					.ToArray();

				var genresDict = genres
					.Where(g => g.Parent == null || g.Parent == 0)
					.ToDictionary(t => t.TagId, t => t.Count);
					
				foreach (var tag in genres) {

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

		public SongVoteRating GetSongRating(int userId, int songId) {
			
			if (userId == 0)
				return SongVoteRating.Nothing;

			return HandleQuery(ctx => {
				
				var r = ctx.OfType<FavoriteSongForUser>().Query().FirstOrDefault(s => s.Song.Id == songId && s.User.Id == userId);

				return r != null ? r.Rating : SongVoteRating.Nothing;

			});

		}

		public PartialFindResult<SongListForApiContract> GetCustomSongLists(int userId, SearchTextQuery textQuery, bool ssl, SongListSortRule sort, PagingProperties paging, SongListOptionalFields fields) {
			
			return HandleQuery(ctx => { 
				
				var query = ctx.Query<SongList>()
					.Where(s => s.Author.Id == userId && s.FeaturedCategory == SongListFeaturedCategory.Nothing)
					.WhereHasName(textQuery);

				var items = query.OrderBy(sort)
					.Paged(paging)
					.Select(s => new SongListForApiContract(s, userIconFactory, entryImagePersister, ssl, fields))
					.ToArray();

				var count = paging.GetTotalCount ? query.Count() : 0;

				return new PartialFindResult<SongListForApiContract>(items, count);

			});

		}

		public TagSelectionContract[] GetAlbumTagSelections(int albumId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<AlbumTagUsage>().Where(a => a.Album.Id == albumId && !a.Tag.Deleted).ToArray();
				var tagVotes = session.Query<AlbumTagVote>().Where(a => a.User.Id == userId && a.Usage.Album.Id == albumId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag, LanguagePreference, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		public TagSelectionContract[] GetArtistTagSelections(int artistId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<ArtistTagUsage>().Where(a => a.Artist.Id == artistId && !a.Tag.Deleted).ToArray();
				var tagVotes = session.Query<ArtistTagVote>().Where(a => a.User.Id == userId && a.Usage.Artist.Id == artistId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag, LanguagePreference, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		public TagSelectionContract[] GetSongTagSelections(int songId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<SongTagUsage>().Where(a => a.Song.Id == songId && !a.Tag.Deleted).ToArray();
				var tagVotes = session.Query<SongTagVote>().Where(a => a.User.Id == userId && a.Usage.Song.Id == songId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag, LanguagePreference, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		public UserForApiContract GetUser(int id, UserOptionalFields fields) {
			return HandleQuery(ctx => new UserForApiContract(ctx.Load<User>(id), userIconFactory, fields));
		}

		public UserDetailsContract GetUserByNameNonSensitive(string name) {

			if (string.IsNullOrEmpty(name))
				return null;

			return HandleQuery(session => {

				var user = session
					.Query()
					.FirstOrDefault(u => u.Name == name);

				if (user == null)
					return null;

				return GetUserDetails(session, user);
				
			});

		}

		public UserDetailsContract GetUserDetails(int id) {

			return HandleQuery(ctx => GetUserDetails(ctx, ctx.Load(id)));

		}

		public PartialFindResult<T> GetUsers<T>(SearchTextQuery textQuery, UserGroupId groupId, bool disabled, bool verifiedArtists, string knowsLanguage, 
			UserSortRule sortRule, PagingProperties paging,
			Func<User, T> fac) {

			return repository.HandleQuery(ctx => {

				var usersQuery = ctx.Query()
					.WhereHasName(textQuery)
					.WhereKnowsLanguage(knowsLanguage);

				if (groupId != UserGroupId.Nothing) {
					usersQuery = usersQuery.Where(u => u.GroupId == groupId);
				}

				if (!disabled) {
					usersQuery = usersQuery.Where(u => u.Active);
				}

				if (verifiedArtists) {
					usersQuery = usersQuery.Where(u => u.VerifiedArtist);
				}

				var users = AddOrder(usersQuery, sortRule)
					.Paged(paging)
					.ToArray();

				var count = paging.GetTotalCount ? usersQuery.Count() : 0;
				var contracts = users.Select(fac).ToArray();

				return new PartialFindResult<T>(contracts, count);

			});

		}

		public void RequestEmailVerification(int userId, string resetUrl) {

			repository.HandleTransaction(ctx => {

				var user = ctx.Load(userId);
				ctx.AuditLogger.SysLog(string.Format("requesting email verification ({0})", user.Email), user.Name);

				var subject = "Verify your email at VocaDB.";

				SendEmailVerificationRequest(ctx, user, resetUrl, subject);

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
		public void RequestPasswordReset(string username, string email, string resetUrl) {

			ParamIs.NotNullOrEmpty(() => username);
			ParamIs.NotNullOrEmpty(() => email);

			var lc = username.ToLowerInvariant();

			repository.HandleTransaction(ctx => {

				var user = ctx.Query().FirstOrDefault(u => u.Active && u.NameLC.Equals(lc) && email.Equals(u.Email));

				if (user == null) {
					log.Info("User not found or not active: {0}", username);
					throw new UserNotFoundException();
				}

				var request = new PasswordResetRequest(user);
				ctx.Save(request);

				var resetFullUrl = string.Format("{0}/{1}", resetUrl, request.Id);
				var subject = UserAccountStrings.PasswordResetSubject;
				var body = string.Format(UserAccountStrings.PasswordResetBody, resetFullUrl);

				mailer.SendEmail(request.User.Email, request.User.Name, subject, body);

				ctx.AuditLogger.SysLog("requested password reset", username);

			});

		}

		public UserContract ResetPassword(Guid requestId, string password) {

			ParamIs.NotNullOrEmpty(() => password);

			return repository.HandleTransaction(ctx => {

				var request = ctx.OfType<PasswordResetRequest>().Load(requestId);

				if (!request.IsValid)
					throw new RequestNotValidException("Request has expired");

				var user = request.User;

				ctx.AuditLogger.AuditLog("resetting password", user);

				user.UpdatePassword(password, PasswordHashAlgorithms.Default);

				ctx.Update(user);

				ctx.Delete(request);

				return new UserContract(user);

			});

		}

		public TagUsageForApiContract[] SaveAlbumTags(int albumId, TagBaseContract[] tags, bool onlyAdd) {
			
			return new TagUsageQueries(PermissionContext).AddTags<Album, AlbumTagUsage>(
				albumId, tags, onlyAdd, repository, entryLinkFactory,
				album => album.Tags, 
				(album, ctx) => new AlbumTagUsageFactory(ctx, album));

		}

		public TagUsageForApiContract[] SaveArtistTags(int artistId, TagBaseContract[] tags, bool onlyAdd) {
			
			return new TagUsageQueries(PermissionContext).AddTags<Artist, ArtistTagUsage>(
				artistId, tags, onlyAdd, repository, entryLinkFactory,
				artist => artist.Tags, 
				(artist, ctx) => new ArtistTagUsageFactory(ctx, artist));

		}

		public TagUsageForApiContract[] SaveSongTags(int songId, TagBaseContract[] tags, bool onlyAdd) {
			
			return new TagUsageQueries(PermissionContext).AddTags<Song, SongTagUsage>(
				songId, tags, onlyAdd, repository, entryLinkFactory,
				song => song.Tags, 
				(song, ctx) => new SongTagUsageFactory(ctx, song));

		}

		public void SendMessage(UserMessageContract contract, string mySettingsUrl, string messagesUrl) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var receiver = session.Query().FirstOrDefault(u => u.Name.Equals(contract.Receiver.Name));

				if (receiver == null)
					throw new UserNotFoundException();

				var sender = session.Load(contract.Sender.Id);

				VerifyResourceAccess(sender);

				session.AuditLogger.SysLog("sending message from " + sender + " to " + receiver);

				var messages = sender.SendMessage(receiver, contract.Subject, contract.Body, contract.HighPriority);

				if (receiver.EmailOptions == UserEmailOptions.PrivateMessagesFromAll
					|| (receiver.EmailOptions == UserEmailOptions.PrivateMessagesFromAdmins
						&& sender.EffectivePermissions.Has(PermissionToken.DesignatedStaff))) {

					SendPrivateMessageNotification(mySettingsUrl, messagesUrl, messages.Item1);

				}

				session.Save(messages.Item1);
				session.Save(messages.Item2);

			});

		}

		public void SetAlbumFormatString(string formatString) {

			if (!PermissionContext.IsLoggedIn)
				return;

			repository.HandleTransaction(ctx => {

				var user = ctx.GetLoggedUser(PermissionContext);

				user.Options.AlbumFormatString = formatString;
				ctx.Update(user);

			});

		}

		private void VerifyEditUser(IDatabaseContext<User> ctx, IUserPermissionContext permissionContext, User user) {

			if (!EntryPermissionManager.CanEditUser(PermissionContext, user.GroupId)) {
				var loggedUser = ctx.GetLoggedUser(PermissionContext);
				var msg = string.Format("{0} (level {1}) not allowed to edit {2} (level {3})", loggedUser, loggedUser.GroupId, user, user.GroupId);
				log.Error(msg);
				throw new NotAllowedException(msg);
			}

		}

		public void SetUserToLimited(int userId) {

			repository.UpdateEntity<User, IDatabaseContext<User>>(userId, (session, user) => {

				VerifyEditUser(session, PermissionContext, user);

				user.GroupId = UserGroupId.Limited;

			}, PermissionToken.RemoveEditPermission, PermissionContext);

		}

		public void UpdateUser(UserWithPermissionsContract contract) {

			ParamIs.NotNull(() => contract);

			repository.UpdateEntity<User, IDatabaseContext<User>>(contract.Id, (session, user) => {

				VerifyEditUser(session, PermissionContext, user);

				if (EntryPermissionManager.CanEditGroupTo(PermissionContext, contract.GroupId)) {
					user.GroupId = contract.GroupId;
				}

				if (EntryPermissionManager.CanEditAdditionalPermissions(PermissionContext)) {
					user.AdditionalPermissions = new PermissionCollection(contract.AdditionalPermissions.Select(p => PermissionToken.GetById(p.Id)));
				}

				if (user.Name != contract.Name) {

					if (!Regex.IsMatch(contract.Name, "^" + User.NameRegex + "$")) {
						throw new InvalidUserNameException();
					}

					var nameInUse = session.Query().Any(u => u.Name == contract.Name && u.Id != contract.Id);

					if (nameInUse) {
						throw new UserNameAlreadyExistsException();
					}

					session.AuditLogger.AuditLog(string.Format("changed username of {0} to '{1}'", EntryLinkFactory.CreateEntryLink(user), contract.Name));

					var usernameEntry = new OldUsername(user, user.Name);
					session.Save(usernameEntry);

					user.Name = contract.Name;
					user.NameLC = contract.Name.ToLowerInvariant();

				}

				var diff = OwnedArtistForUser.Sync(user.AllOwnedArtists, contract.OwnedArtistEntries, a => user.AddOwnedArtist(session.Load<Artist>(a.Artist.Id)));
				session.Sync(diff);
				user.VerifiedArtist = user.OwnedArtists.Any();

				user.Active = contract.Active;
				user.Email = contract.Email;
				user.Options.Poisoned = contract.Poisoned;
				user.Options.Supporter = contract.Supporter;

				session.AuditLogger.AuditLog(string.Format("updated user {0}", EntryLinkFactory.CreateEntryLink(user)));

			}, PermissionToken.ManageUserPermissions, PermissionContext, skipLog: true);

		}

		public void UpdateAlbumForUser(int userId, int albumId, PurchaseStatus status, 
			MediaType mediaType, int rating) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			repository.HandleTransaction(session => {

				var albumForUser = session.OfType<AlbumForUser>().Query()
					.FirstOrDefault(a => a.Album.Id == albumId && a.User.Id == userId);

				// Delete
				if (albumForUser != null && status == PurchaseStatus.Nothing && rating == 0) {

					session.AuditLogger.AuditLog(string.Format("deleting {0} from collection", 
						entryLinkFactory.CreateEntryLink(albumForUser.Album)));

					NHibernateUtil.Initialize(albumForUser.Album.CoverPictureData);

					albumForUser.Delete();
					session.Delete(albumForUser);
					session.Update(albumForUser.Album);

				// Add
				} else if (albumForUser == null && (status != PurchaseStatus.Nothing || rating != 0)) {

					var user = session.Load(userId);
					var album = session.OfType<Album>().Load(albumId);

					NHibernateUtil.Initialize(album.CoverPictureData);
					albumForUser = user.AddAlbum(album, status, mediaType, rating);
					session.Save(albumForUser);
					session.Update(album);

					session.AuditLogger.AuditLog(string.Format("added {0} to collection", entryLinkFactory.CreateEntryLink(album)));

				// Update
				} else if (albumForUser != null) {

					albumForUser.MediaType = mediaType;
					albumForUser.PurchaseStatus = status;
					session.Update(albumForUser);

					if (albumForUser.Rating != rating) {
						albumForUser.Rating = rating;
						albumForUser.Album.UpdateRatingTotals();
						NHibernateUtil.Initialize(albumForUser.Album.CoverPictureData);
						session.Update(albumForUser.Album);
					}

					session.AuditLogger.AuditLog(string.Format("updated {0} in collection", 
						entryLinkFactory.CreateEntryLink(albumForUser.Album)));

				}

			});

		}

		public void UpdateArtistSubscriptionForCurrentUser(int artistId, bool? emailNotifications, bool? siteNotifications) {
			
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			var userId = PermissionContext.LoggedUserId;

			repository.HandleTransaction(ctx => {
				
				var subscription = ctx.OfType<ArtistForUser>().Query().FirstOrDefault(u => u.User.Id == userId && u.Artist.Id == artistId);

				// No subscription found. Shouldn't happen, but could also be handled so that a new subscription is added.
				if (subscription == null)
					return;

				if (emailNotifications.HasValue)
					subscription.EmailNotifications = emailNotifications.Value;

				if (siteNotifications.HasValue)
					subscription.SiteNotifications = siteNotifications.Value;

				ctx.Update(subscription);

				ctx.AuditLogger.SysLog(string.Format("updated artist subscription for {0}.", subscription.Artist));

			});

		}

		/// <summary>
		/// Updates user's settings (from my settings page).
		/// </summary>
		/// <param name="contract">New properties. Cannot be null.</param>
		/// <returns>Updated user data. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="InvalidPasswordException">If password change was attempted and the old password was incorrect.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken by another user.</exception>
		public UserWithPermissionsContract UpdateUserSettings(UpdateUserSettingsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {

				var user = ctx.Load(contract.Id);

				ctx.AuditLogger.SysLog(string.Format("Updating settings for {0}", user));

				PermissionContext.VerifyResourceAccess(user);

				if (!string.IsNullOrEmpty(contract.NewPass)) {

					var oldAlgorithm = PasswordHashAlgorithms.Get(user.PasswordHashAlgorithm);
					var oldHashed = (!string.IsNullOrEmpty(user.Password) ? oldAlgorithm.HashPassword(contract.OldPass, user.Salt, user.NameLC) : string.Empty);

					if (user.Password != oldHashed)
						throw new InvalidPasswordException();

					user.UpdatePassword(contract.NewPass, PasswordHashAlgorithms.Default);

				}

				var email = contract.Email;

				if (!string.IsNullOrEmpty(email)) {

					ValidateEmail(email);

					var existing = ctx.Query().FirstOrDefault(u => u.Active && u.Id != user.Id && u.Email == email);

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
				//user.Options.ShowChatbox = contract.ShowChatbox;
				user.Options.UnreadNotificationsToKeep = contract.UnreadNotificationsToKeep;
				user.SetEmail(email);

				var validWebLinks = contract.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(user.WebLinks, validWebLinks, user);
				ctx.OfType<UserWebLink>().Sync(webLinkDiff);

				var knownLanguagesDiff = CollectionHelper.Sync(user.KnownLanguages, contract.KnownLanguages.Distinct(l => l.CultureCode), (l, l2) => l.CultureCode.Equals(l2.CultureCode) && l.Proficiency == l2.Proficiency, l => user.AddKnownLanguage(l.CultureCode, l.Proficiency));
				ctx.Sync(knownLanguagesDiff);

				ctx.Update(user);

				ctx.AuditLogger.AuditLog("updated settings");

				return new UserWithPermissionsContract(user, PermissionContext.LanguagePreference);

			});

		}

		public void UpdateUserSetting(IUserSetting setting) {
			
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			repository.HandleTransaction(session => {

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
		public bool VerifyEmail(Guid requestId) {
			
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {
				
				var request = ctx.OfType<PasswordResetRequest>().Get(requestId);

				if (request == null || !request.IsValid)
					return false;

				var user = request.User;
				if (!user.Equals(PermissionContext.LoggedUser))
					throw new RequestNotValidException("Email verification request not valid for this user");

				if (!user.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase)) {

					log.Info(string.Format("Email {0} not valid for {1}", request.Email, user));
					throw new RequestNotValidException("Email verification request not valid for this user");					

					/*
					// Update email from request in case the user hasn't saved the new email yet.
					user.Email = request.Email;
					*/
				}

				user.Options.EmailVerified = true;
				ctx.Update(user);

				ctx.Delete(request);

				ctx.AuditLogger.AuditLog(string.Format("verified email ({0})", user.Email));

				return true;

			});

		}

	}

	public class AlbumTagUsageFactory : ITagUsageFactory<AlbumTagUsage> {

		private readonly Album album;
		private readonly IDatabaseContext<AlbumTagUsage> session;

		public AlbumTagUsageFactory(IDatabaseContext<AlbumTagUsage> session, Album album) {
			this.session = session;
			this.album = album;
		}

		public AlbumTagUsage CreateTagUsage(Tag tag) {

			var usage = new AlbumTagUsage(album, tag);
			session.Save(usage);

			return usage;

		}

		public AlbumTagUsage CreateTagUsage(Tag tag, AlbumTagUsage oldUsage) {

			var usage = new AlbumTagUsage(oldUsage.Album, tag);
			session.Save(usage);

			return usage;

		}

	}

	public class ArtistTagUsageFactory : ITagUsageFactory<ArtistTagUsage> {

		private readonly Artist artist;
		private readonly IDatabaseContext<ArtistTagUsage> session;

		public ArtistTagUsageFactory(IDatabaseContext<ArtistTagUsage> session, Artist artist) {
			this.session = session;
			this.artist = artist;
		}

		public ArtistTagUsage CreateTagUsage(Tag tag) {

			var usage = new ArtistTagUsage(artist, tag);
			session.Save(usage);

			return usage;

		}

		public ArtistTagUsage CreateTagUsage(Tag tag, ArtistTagUsage oldUsage) {

			var usage = new ArtistTagUsage(oldUsage.Artist, tag);
			session.Save(usage);

			return usage;

		}

	}

	public class SongTagUsageFactory : ITagUsageFactory<SongTagUsage> {

		private readonly Song song;
		private readonly IDatabaseContext<SongTagUsage> ctx;

		public SongTagUsageFactory(IDatabaseContext<SongTagUsage> ctx, Song song) {
			this.ctx = ctx;
			this.song = song;
		}

		public SongTagUsage CreateTagUsage(Tag tag) {

			var usage = new SongTagUsage(song, tag);
			ctx.Save(usage);

			return usage;

		}

		public SongTagUsage CreateTagUsage(Tag tag, SongTagUsage oldUsage) {

			var usage = new SongTagUsage(oldUsage.Song, tag);
			ctx.Save(usage);

			return usage;

		}

	}

}