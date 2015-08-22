using System;
using System.Linq;
using System.Net.Mail;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Exceptions;

namespace VocaDb.Model.Domain.Users {

	public class User : IEntryWithNames, IUserWithEmail, IEquatable<IUser>, IWebLinkFactory<UserWebLink> {

		INameManager IEntryWithNames.Names {
			get {
				return new SingleNameManager(Name);
			}
		}

		int IEntryBase.Version {
			get { return 0; }
		}

		private string accessKey;
		private PermissionCollection additionalPermissions = new PermissionCollection();
		private IList<AlbumForUser> albums = new List<AlbumForUser>();
		private IList<ArtistForUser> artists = new List<ArtistForUser>();
		private IList<UserComment> comments = new List<UserComment>();
		private string culture;
		private string email;
		private IList<FavoriteSongForUser> favoriteSongs = new List<FavoriteSongForUser>();
		private string language;
		private string name;
		private string nameLc;
		private UserOptions options;
		private IList<OwnedArtistForUser> ownedArtists = new List<OwnedArtistForUser>(); 
		private string password;
		private IList<UserMessage> receivedMessages = new List<UserMessage>();
		private IList<UserMessage> sentMessages = new List<UserMessage>();
		private IList<SongList> songLists = new List<SongList>();
		private IList<UserWebLink> webLinks = new List<UserWebLink>();

		public User() {

			Active = true;
			AnonymousActivity = false;
			CreateDate = DateTime.Now;
			Culture = string.Empty;
			DefaultLanguageSelection = ContentLanguagePreference.Default;
			Email = string.Empty;
			EmailOptions = UserEmailOptions.PrivateMessagesFromAll;
			GroupId = UserGroupId.Regular;
			Language = string.Empty;
			LastLogin = DateTime.Now;
			Options = new UserOptions(this);
			PreferredVideoService = PVService.Youtube;

		}

		public User(string name, string pass, string email, int salt)
			: this() {

			Name = name;
			NameLC = name.ToLowerInvariant();
			Password = pass;
			Email = email;
			Salt = salt;

			GenerateAccessKey();

		}

		public virtual string AccessKey {
			get { return accessKey; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				accessKey = value; 
			}
		}

		public virtual bool Active { get; set; }

		/// <summary>
		/// Additional user permissions. Base permissions are assigned through the user group.
		/// </summary>
		public virtual PermissionCollection AdditionalPermissions {
			get {
				return additionalPermissions;
			}
			set {
				additionalPermissions = value ?? new PermissionCollection();
			}
		}

		public virtual IEnumerable<AlbumForUser> Albums {
			get {
				return AllAlbums.Where(a => !a.Album.Deleted);
			}
		}

		public virtual IList<AlbumForUser> AllAlbums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		/// <summary>
		/// List of artists followed by this user. This list does not include deleted entries. Cannot be null.
		/// </summary>
		public virtual IEnumerable<ArtistForUser> Artists {
			get {
				return AllArtists.Where(a => !a.Artist.Deleted);
			}
		}

		/// <summary>
		/// List of artists followed by this user. Includes deleted artists. Cannot be null.
		/// </summary>
		public virtual IList<ArtistForUser> AllArtists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		/// <summary>
		/// List of artists entries for which this user is a verified owner. Includes deleted artists. Cannot be null.
		/// </summary>
		public virtual IList<OwnedArtistForUser> AllOwnedArtists {
			get { return ownedArtists; }
			set {
				ParamIs.NotNull(() => value);
				ownedArtists = value;
			}
		}

		public virtual bool AnonymousActivity { get; set; }

		public virtual bool CanBeDisabled {
			get {

				return !EffectivePermissions.Has(PermissionToken.DisableUsers);

			}
		}

		/// <summary>
		/// List of comments on this user's profile.
		/// This is not the list of comments this user had made himself!
		/// </summary>
		public virtual IList<UserComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		/// <summary>
		/// User's culture setting (date/time and number formatting).
		/// This is an ISO culture code, for example "en-US".
		/// Cannot be null.
		/// Can be empty, in which case the culture is set automatically.
		/// Also see <see cref="Language"/>.
		/// </summary>
		public virtual string Culture {
			get { return culture; }
			set { 
				ParamIs.NotNull(() => value);
				culture = value; 
			}
		}

		public virtual ContentLanguagePreference DefaultLanguageSelection { get; set; }

		public virtual string DefaultName {
			get { return Name; }
		}

		public virtual bool Deleted {
			get { return !Active; }
		}

		public virtual PermissionCollection EffectivePermissions {
			get {

				if (!Active)
					return new PermissionCollection();

				return UserGroup.GetPermissions(GroupId).Merge(AdditionalPermissions);

			}
		}

		/// <summary>
		/// Email address. Can be empty. Cannot be null. Must be unique.
		/// </summary>
		/// <remarks>
		/// Email address, like username, can be used for logging in.
		/// Thus they must be unique.
		/// </remarks>
		public virtual string Email {
			get { return email; }
			set {
				ParamIs.NotNull(() => value);
				email = value;
			}
		}

		public virtual UserEmailOptions EmailOptions { get; set; }

		public virtual EntryType EntryType {
			get { return EntryType.User; }
		}

		public virtual IList<FavoriteSongForUser> FavoriteSongs {
			get { return favoriteSongs; }
			set {
				ParamIs.NotNull(() => value);
				favoriteSongs = value;
			}
		}

		public virtual bool HasPassword {
			get {
				return !string.IsNullOrEmpty(Password);
			}
		}

		public virtual int Id { get; set; }

		public virtual UserGroupId GroupId { get; set; }

		/// <summary>
		/// User's group. Cannot be null.
		/// </summary>
		public virtual UserGroup Group {
			get {
				return UserGroup.GetGroup(GroupId);
			}
		}

		/// <summary>
		/// User's language setting. This is an ISO culture code, for example "en-US".
		/// Determine's user interface language.
		/// Cannot be null.
		/// Can be empty, in which case the language is determined automatically.
		/// </summary>
		public virtual string Language {
			get { return language; }
			set {
				ParamIs.NotNull(() => value);
				language = value;
			}
		}

		/// <summary>
		/// User's language setting or last login culture if language is not set (automatic).
		/// Can be empty if neither is set.
		/// </summary>
		public virtual string LanguageOrLastLoginCulture {
			get {
				
				return !string.IsNullOrEmpty(Language) ? Language : Options.LastLoginCulture;

			}
		}

		public virtual DateTime LastLogin { get; set; }

		/// <summary>
		/// Username.
		/// Must be unique, case-insensitive.
		/// </summary>
		/// <remarks>
		/// Currently the allowed characters in username are English alphabet, numbers and underscores.
		/// Characters are limited because of the URL to the user profile page, which includes the full username.
		/// 
		/// Notes about allowing characters:
		/// - Spaces and dots would be problematic because of URLs. Some legacy accounts still have these chars.
		/// - At '@' symbols would be problematic because username must be separate from the email.
		///   Email, like username, can be used for logging in. Thus one user could "steal" another user's email has his username.
		/// - Hyphens could probably be allowed.
		/// </remarks>
		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value;
			}
		}

		/// <summary>
		/// Username in lowercase.
		/// Password has is always based on the lowercase username.
		/// </summary>
		public virtual string NameLC {
			get { return nameLc; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				nameLc = value;
			}
		}

		public virtual UserOptions Options {
			get { return options; }
			set {
				options = value ?? new UserOptions(this);
			}
		}

		/*
		public virtual UserOptions Options {
			get {

				if (!OptionsList.Any())
					OptionsList.Add(new UserOptions(this));

				return OptionsList.First();

			}
		}

		public virtual IList<UserOptions> OptionsList {
			get { return optionsList; }
			set { optionsList = value; }
		}
		 */

		/// <summary>
		/// List of artists entries for which this user is a verified owner. Does not include deleted artists. Cannot be null.
		/// </summary>
		public virtual IEnumerable<OwnedArtistForUser> OwnedArtists {
			get {
				return AllOwnedArtists.Where(a => !a.Artist.Deleted);
			}
		}

		/// <summary>
		/// Password SHA-1 hash.
		/// </summary>
		public virtual string Password {
			get { return password; }
			set {
				ParamIs.NotNull(() => value);
				password = value;
			}
		}

		public virtual PVService PreferredVideoService { get; set; }

		public virtual IList<UserMessage> ReceivedMessages {
			get { return receivedMessages; }
			set {
				ParamIs.NotNull(() => value);
				receivedMessages = value;
			}
		}

		public virtual RoleTypes Roles { get; set; }

		/// <summary>
		/// Per-user password salt. Applied to password hash.
		/// </summary>
		public virtual int Salt { get; set; }

		public virtual IList<UserMessage> SentMessages {
			get { return sentMessages; }
			set {
				ParamIs.NotNull(() => value);
				sentMessages = value;
			}
		}

		public virtual IList<SongList> SongLists {
			get { return songLists; }
			set {
				ParamIs.NotNull(() => value);
				songLists = value; 
			}
		}

		public virtual IList<UserWebLink> WebLinks {
			get { return webLinks; }
			set { 
				ParamIs.NotNull(() => value);
				webLinks = value; 
			}
		}

		public virtual AlbumForUser AddAlbum(Album album, PurchaseStatus status, MediaType mediaType, int rating) {

			ParamIs.NotNull(() => album);

			var link = new AlbumForUser(this, album, status, mediaType, rating);
			AllAlbums.Add(link);
			album.UserCollections.Add(link);
			album.UpdateRatingTotals();

			return link;

		}

		/// <summary>
		/// Adds a artist subscription (followed artist) for this user.
		/// </summary>
		/// <param name="artist">Artist to be subscribed to. Cannot be null.</param>
		/// <returns>The link object. Cannot be null.</returns>
		public virtual ArtistForUser AddArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			var link = new ArtistForUser(this, artist);
			AllArtists.Add(link);
			artist.Users.Add(link);

			return link;

		}

		public virtual OwnedArtistForUser AddOwnedArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			var old = ownedArtists.FirstOrDefault(a => a.Artist.Equals(artist));

			if (old != null)
				throw new ArgumentException("Unable to add the same artist again", "artist");

			var link = new OwnedArtistForUser(this, artist);
			AllOwnedArtists.Add(link);
			artist.OwnerUsers.Add(link);

			return link;

		}

		public virtual FavoriteSongForUser AddSongToFavorites(Song song, SongVoteRating rating) {
			
			ParamIs.NotNull(() => song);

			var link = new FavoriteSongForUser(this, song, rating);
			FavoriteSongs.Add(link);
			song.UserFavorites.Add(link);

			if (rating == SongVoteRating.Favorite)
				song.FavoritedTimes++;

			song.RatingScore += FavoriteSongForUser.GetRatingScore(rating);

			return link;

		}

		public virtual void ClearTwitter() {

			if (!HasPassword) {
				throw new NoPasswordException("Cannot disconnect Twitter if there is no password set.");
			}

			Options.TwitterName = Options.TwitterOAuthToken =Options.TwitterOAuthTokenSecret = string.Empty;
			Options.TwitterId = 0;

		}

		public virtual UserComment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new UserComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual UserWebLink CreateWebLink(WebLinkContract contract) {

			ParamIs.NotNull(() => contract);

			var link = new UserWebLink(this, contract);
			WebLinks.Add(link);

			return link;

		}

		public virtual UserWebLink CreateWebLink(string description, string url, WebLinkCategory category) {
			return CreateWebLink(new WebLinkContract(url, description, category));
		}

		public virtual bool Equals(IUser another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return string.Equals(this.Name, another.Name, StringComparison.InvariantCultureIgnoreCase);

		}

		public override bool Equals(object obj) {
			return Equals(obj as IUser);
		}

		public virtual void GenerateAccessKey() {

			AccessKey = new AlphaPassGenerator(true, true, true).Generate(20);

		}

		public override int GetHashCode() {
			return NameLC.GetHashCode();
		}

		public virtual bool IsTheSameUser(UserContract contract) {

			if (contract == null)
				return false;

			return (Id == contract.Id);

		}

		public virtual Tuple<UserMessage, UserMessage> SendMessage(User to, string subject, string body, bool highPriority) {

			ParamIs.NotNull(() => to);

			var received = UserMessage.CreateReceived(this, to, subject, body, highPriority);
			to.ReceivedMessages.Add(received);

			var sent = UserMessage.CreateSent(this, to, subject, body, highPriority);
			SentMessages.Add(sent);

			return Tuple.Create(received, sent);

		}

		public virtual void SetEmail(string newEmail) {
			
			ParamIs.NotNull(() => newEmail);

			if (newEmail != string.Empty)
				new MailAddress(newEmail);

			if (!string.Equals(Email, newEmail, StringComparison.InvariantCultureIgnoreCase)) {
				Email = newEmail;
				Options.EmailVerified = false;				
			}

		}

		public override string ToString() {
			return string.Format("user '{0}' [{1}]", Name, Id);
		}

		public virtual void UpdateLastLogin(string host, string culture) {
			LastLogin = DateTime.Now;
			Options.LastLoginAddress = host;
			Options.LastLoginCulture = culture;
		}

	}

}
