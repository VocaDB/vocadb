#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Security;
using IUserRepository = VocaDb.Model.Database.Repositories.IUserRepository;

namespace VocaDb.Model.Service
{
	public class UserService : QueriesBase<IUserRepository, User>
	{
		// ReSharper disable UnusedMember.Local
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		// ReSharper restore UnusedMember.Local

		private IEntryLinkFactory EntryLinkFactory { get; }
		private readonly IUserIconFactory _userIconFactory;

		private string MakeGeoIpToolLink(string hostname)
		{
			return $"<a href='http://www.geoiptool.com/?IP={hostname}'>{hostname}</a>";
		}

		public UserService(
			IUserRepository sessionFactory,
			IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			IUserMessageMailer userMessageMailer,
			IUserIconFactory userIconFactory)
			: base(sessionFactory, permissionContext)
		{
			EntryLinkFactory = entryLinkFactory;
			_userIconFactory = userIconFactory;
		}

		public void AddArtist(int userId, int artistId)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session =>
			{
				var exists = session.Query<ArtistForUser>().Any(u => u.User.Id == userId && u.Artist.Id == artistId);

				if (exists)
					return;

				var user = session.Load<User>(userId);
				var artist = session.Load<Artist>(artistId);

				user.AddArtist(artist);

				session.Update(user);

				AuditLog($"followed {artist}", session, user);
			});
		}

		public ServerOnlyUserContract CheckAccessWithKey(string name, string accessKey, string hostname, bool delayFailedLogin)
		{
			return HandleQuery(session =>
			{
				var lc = name.ToLowerInvariant();
				var user = session.Query<User>().FirstOrDefault(u => u.Active && u.NameLC == lc);

				if (user == null)
				{
					AuditLog($"failed login from {MakeGeoIpToolLink(hostname)} - no user.", session, name);

					if (delayFailedLogin)
						Thread.Sleep(2000);

					return null;
				}

				var hashed = LoginManager.GetHashedAccessKey(user.AccessKey);

				if (accessKey != hashed)
				{
					AuditLog($"failed login from {MakeGeoIpToolLink(hostname)} - wrong password.", session, name);

					if (delayFailedLogin)
						Thread.Sleep(2000);

					return null;
				}

				AuditLog($"logged in from {MakeGeoIpToolLink(hostname)} with access key.", session, user);

				return new ServerOnlyUserContract(user);
			});
		}

		public ServerOnlyUserContract CheckTwitterAuthentication(string accessToken, string hostname, string culture)
		{
			return HandleTransaction(session =>
			{
				var user = session.Query<User>().FirstOrDefault(u => u.Active && u.Options.TwitterOAuthToken == accessToken);

				if (user == null)
					return null;

				AuditLog($"logged in from {MakeGeoIpToolLink(hostname)} with twitter.", session, user);

				user.UpdateLastLogin(hostname, culture);
				session.Update(user);

				return new ServerOnlyUserContract(user);
			});
		}

		public bool ConnectTwitter(string authToken, int twitterId, string twitterName, string hostname)
		{
			ParamIs.NotNullOrEmpty(() => authToken);
			ParamIs.NotNullOrEmpty(() => hostname);

			return HandleTransaction(session =>
			{
				var user = session.Query<UserOptions>().Where(u => u.TwitterOAuthToken == authToken)
					.Select(a => a.User).FirstOrDefault();

				if (user != null)
					return false;

				user = GetLoggedUser(session);

				user.Options.TwitterId = twitterId;
				user.Options.TwitterName = twitterName;
				user.Options.TwitterOAuthToken = authToken;
				session.Update(user);

				AuditLog($"connected to Twitter account '{twitterName}' from {MakeGeoIpToolLink(hostname)}.", session, user);

				return true;
			});
		}

		public void DeleteComment(int commentId)
		{
			HandleTransaction(session =>
			{
				var comment = session.Load<UserComment>(commentId);
				var user = GetLoggedUser(session);

				AuditLog("deleting " + comment, session, user);

				if (!user.Equals(comment.Author) && !user.Equals(comment.EntryForComment))
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);

				comment.Delete();
				session.Update(comment);
			});
		}

		public CommentForApiContract[] GetComments(int userId)
		{
			return HandleQuery(session =>
			{
				var user = session.Load<User>(userId);

				var comments = session.Query<AlbumComment>()
					.WhereNotDeleted()
					.Where(c => c.Author == user && !c.EntryForComment.Deleted).OrderByDescending(c => c.Created).ToArray().Cast<Comment>()
					.Concat(session.Query<ArtistComment>()
						.WhereNotDeleted()
						.Where(c => c.Author == user && !c.EntryForComment.Deleted)).OrderByDescending(c => c.Created).ToArray();

				return comments.Select(c => new CommentForApiContract(c, _userIconFactory)).ToArray();
			});
		}

		public ServerOnlyUserContract GetUser(int id, bool getPublicCollection = false)
		{
			return HandleQuery(session => new ServerOnlyUserContract(session.Load<User>(id), getPublicCollection));
		}

		public ServerOnlyUserForMySettingsContract GetUserForMySettings(int id)
		{
			return HandleQuery(session => new ServerOnlyUserForMySettingsContract(session.Load<User>(id)));
		}

		public ServerOnlyUserWithPermissionsContract GetUserWithPermissions(int id)
		{
			return HandleQuery(session => new ServerOnlyUserWithPermissionsContract(session.Load<User>(id), LanguagePreference));
		}

		public ServerOnlyUserWithPermissionsContract GetUserByName(string name, bool skipMessages)
		{
			return HandleQuery(session =>
			{
				var user = session.Query<User>().FirstOrDefault(u => u.Name.Equals(name));

				if (user == null)
					return null;

				var contract = new ServerOnlyUserWithPermissionsContract(user, LanguagePreference);

				if (!skipMessages)
					contract.UnreadMessagesCount = session.Query<UserMessage>()
						.Where(m => m.User.Id == user.Id)
						.WhereIsUnread(true)
						.WhereInboxIs(UserInboxType.Nothing, true)
						.Count();

				return contract;
			});
		}

		public PartialFindResult<UserMessageContract> GetReceivedMessages(int userId, PagingProperties paging)
		{
			return HandleQuery(session =>
			{
				var query = session.Query<UserMessage>()
					.Where(m => m.Receiver.Id == userId);

				var messages = query
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray();

				var count = (paging.GetTotalCount ? query.Count() : 0);

				return new PartialFindResult<UserMessageContract>(messages.Select(m => new UserMessageContract(m, null)).ToArray(), count);
			});
		}

		public PartialFindResult<UserMessageContract> GetSentMessages(int userId, PagingProperties paging)
		{
			return HandleQuery(session =>
			{
				var query = session.Query<UserMessage>()
					.Where(m => m.Sender.Id == userId);

				var messages = query
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray();

				var count = (paging.GetTotalCount ? query.Count() : 0);

				return new PartialFindResult<UserMessageContract>(messages.Select(m => new UserMessageContract(m, null)).ToArray(), count);
			});
		}

		public void RemoveArtistFromUser(int userId, int artistId)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session =>
			{
				var link = session.Query<ArtistForUser>()
					.FirstOrDefault(a => a.Artist.Id == artistId && a.User.Id == userId);

				AuditLog($"removing {link}", session);

				if (link != null)
				{
					link.Delete();
					session.Delete(link);
				}
			});
		}

		public void ResetAccessKey()
		{
			PermissionContext.VerifyLogin();

			HandleTransaction(session =>
			{
				var user = GetLoggedUser(session);
				user.GenerateAccessKey();

				session.Update(user);

				AuditLog("reset access key", session);
			});
		}

		public void UpdateSongRating(int userId, int songId, SongVoteRating rating)
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session =>
			{
				var existing = session.Query<FavoriteSongForUser>().FirstOrDefault(f => f.User.Id == userId && f.Song.Id == songId);
				var user = session.Load<User>(userId);
				var song = session.Load<Song>(songId);
				var agent = new AgentLoginData(user);

				if (existing != null)
				{
					if (rating != SongVoteRating.Nothing)
					{
						existing.SetRating(rating);
						session.Update(existing);
					}
					else
					{
						existing.Delete();
						session.Delete(existing);
					}
				}
				else if (rating != SongVoteRating.Nothing)
				{
					var link = user.AddSongToFavorites(song, rating);
					session.Save(link);
				}

				session.Update(song);

				AuditLog($"rating {EntryLinkFactory.CreateEntryLink(song)} as '{rating}'.",
					session, agent);
			}, $"Unable to rate song with ID '{songId}'.");
		}
	}

	public class InvalidPasswordException : Exception
	{
		public InvalidPasswordException()
			: base("Invalid password") { }

		protected InvalidPasswordException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}

	public class UserNotFoundException : EntityNotFoundException
	{
		public UserNotFoundException()
			: base("User not found") { }

		protected UserNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}

	public class UserNameAlreadyExistsException : Exception
	{
		public UserNameAlreadyExistsException()
			: base("Username is already taken") { }
	}

	public class InvalidUserNameException : Exception
	{
		public InvalidUserNameException()
			: base("Specified username is invalid") { }

		public InvalidUserNameException(string name)
			: base($"Specified username is invalid: '{name}'") { }
	}

	public class UserNameTooSoonException : Exception
	{
		public UserNameTooSoonException()
			: base("Username cannot be changed yet") { }
	}

	public class UserEmailAlreadyExistsException : Exception
	{
		public UserEmailAlreadyExistsException()
			: base("Email address is already taken") { }
	}

	public enum UserSortRule
	{
		RegisterDate,

		Name,

		Group
	}

	public enum LoginError
	{
		Nothing,

		NotFound,

		InvalidPassword,

		AccountPoisoned,
	}

	public class LoginResult
	{
		public static LoginResult CreateError(LoginError error)
		{
			return new LoginResult { Error = error };
		}

		public static LoginResult CreateSuccess(ServerOnlyUserContract user)
		{
			return new LoginResult { User = user, Error = LoginError.Nothing };
		}

		public LoginError Error { get; set; }

		public bool IsOk => Error == LoginError.Nothing;

		public ServerOnlyUserContract User { get; set; }
	}
}
