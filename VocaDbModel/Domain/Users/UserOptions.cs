#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Users
{
	/// <summary>
	/// Various additional properties for user that are not needed in most cases.
	/// For example, for authentication and user profile.
	/// </summary>
	public class UserOptions : IDatabaseObject
	{
		private string aboutMe;
		private string albumFormatString;
		private string lastLoginAddress;
		private string location;
		private string oauthToken;
		private string oauthTokenSecret;
		private string realname;
		private string twitterName;
		private User user;
		private OptionalCultureCode lastLoginCulture;

		public UserOptions()
		{
			LastLoginAddress
				= AboutMe
				= AlbumFormatString
				= Location
				= Realname
				= TwitterName = TwitterOAuthToken = TwitterOAuthTokenSecret
				= CustomTitle
				= string.Empty;

			LastLoginCulture = OptionalCultureCode.Empty;
			PublicAlbumCollection = true;
			PublicRatings = true;
			ShowChatbox = true;
			EmailVerified = false;
			UnreadNotificationsToKeep = 10;
		}

		public UserOptions(User user)
			: this()
		{
			User = user;
		}

		public virtual string AboutMe
		{
			get { return aboutMe; }
			set
			{
				ParamIs.NotNull(() => value);
				aboutMe = value;
			}
		}

		public virtual string AlbumFormatString
		{
			get { return albumFormatString; }
			set
			{
				ParamIs.NotNull(() => value);
				albumFormatString = value;
			}
		}

		public virtual string CustomTitle { get; set; }

		public virtual bool EmailVerified { get; set; }

		public virtual int Id { get; set; }

		public virtual OptionalCultureCode LastLoginCulture
		{
			get { return lastLoginCulture ?? (lastLoginCulture = new OptionalCultureCode()); }
			set
			{
				lastLoginCulture = value ?? OptionalCultureCode.Empty;
			}
		}

		public virtual string LastLoginAddress
		{
			get { return lastLoginAddress; }
			set
			{
				ParamIs.NotNull(() => value);
				lastLoginAddress = value;
			}
		}

		public virtual string Location
		{
			get { return location; }
			set
			{
				ParamIs.NotNull(() => value);
				location = value;
			}
		}

		/// <summary>
		/// Poisoned accounts cause the user logging in to be banned.
		/// </summary>
		public virtual bool Poisoned { get; set; }

		/// <summary>
		/// Allow others to see this user's album collection (ownership status and media type).
		/// </summary>
		public virtual bool PublicAlbumCollection { get; set; }

		/// <summary>
		/// Allow anyone to see user's album and song ratings. 
		/// Essentially this determines whether the user will appear in the "album in collections" and "song ratings" popups.
		/// </summary>
		public virtual bool PublicRatings { get; set; }

		public virtual string Realname
		{
			get { return realname; }
			set
			{
				ParamIs.NotNull(() => value);
				realname = value;
			}
		}

		public virtual bool ShowChatbox { get; set; }

		public virtual bool Standalone { get; set; }

		/// <summary>
		/// Custom stylesheet name. If null or empty, default stylesheet is used.
		/// </summary>
		public virtual string Stylesheet { get; set; }

		public virtual bool Supporter { get; set; }

		public virtual int TwitterId { get; set; }

		public virtual string TwitterName
		{
			get { return twitterName; }
			set
			{
				ParamIs.NotNull(() => value);
				twitterName = value;
			}
		}

		public virtual string TwitterOAuthToken
		{
			get { return oauthToken; }
			set
			{
				ParamIs.NotNull(() => value);
				oauthToken = value;
			}
		}

		public virtual string TwitterOAuthTokenSecret
		{
			get { return oauthTokenSecret; }
			set
			{
				oauthTokenSecret = value;
				ParamIs.NotNull(() => value);
			}
		}

		public virtual int UnreadNotificationsToKeep { get; set; }

		public virtual User User
		{
			get { return user; }
			set
			{
				ParamIs.NotNull(() => value);
				user = value;
			}
		}
	}
}
