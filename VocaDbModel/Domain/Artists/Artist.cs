using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NLog;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using System;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Artists {

	public class Artist : IEntryBase, IEntryWithNames, IEntryWithStatus, IDeletableEntry, IEquatable<Artist>, INameFactory<ArtistName>, IWebLinkFactory<ArtistWebLink> {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private IList<ArtistForAlbum> albums = new List<ArtistForAlbum>();
		private ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields>();
		private IList<Artist> childVoicebanks = new List<Artist>();
		private IList<ArtistComment> comments = new List<ArtistComment>();
		private User createdBy;
		private string description;
		private IList<GroupForArtist> groups = new List<GroupForArtist>();
		//private IList<AlbumHit> hits = new List<AlbumHit>();
		private IList<GroupForArtist> members = new List<GroupForArtist>();
		private NameManager<ArtistName> names = new NameManager<ArtistName>();
		private IList<OwnedArtistForUser> ownerUsers = new List<OwnedArtistForUser>();
		private EntryPictureFileManager<ArtistPictureFile> pictureManager = new EntryPictureFileManager<ArtistPictureFile>();
		private IList<ArtistForSong> songs = new List<ArtistForSong>();
		private TagManager<ArtistTagUsage> tags = new TagManager<ArtistTagUsage>();
		private IList<ArtistForUser> users = new List<ArtistForUser>();
		private IList<ArtistWebLink> webLinks = new List<ArtistWebLink>();

		public Artist() {
			ArtistType = ArtistType.Unknown;
			CreateDate = DateTime.Now;
			Deleted = false;
			Description = string.Empty;
			Status = EntryStatus.Draft;
			Version = 0;
		}

		public Artist(TranslatedString translatedName)
			: this() {

			ParamIs.NotNull(() => translatedName);

			//TranslatedName = translatedName;
			
			foreach (var name in translatedName.AllLocalized)
				Names.Add(new ArtistName(this, name));

		}

		/// <summary>
		/// List of albums for this artist (not including deleted). 
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IEnumerable<ArtistForAlbum> Albums {
			get {
				return AllAlbums.Where(a => !a.Album.Deleted);
			}
		}

		/// <summary>
		/// List of all albums for this artist (including deleted). 
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IList<ArtistForAlbum> AllAlbums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		public virtual IList<GroupForArtist> AllGroups {
			get { return groups; }
			set {
				ParamIs.NotNull(() => value);
				groups = value;
			}
		}

		public virtual IList<GroupForArtist> AllMembers {
			get { return members; }
			set {
				ParamIs.NotNull(() => value);
				members = value;
			}
		}

		public virtual IList<ArtistForSong> AllSongs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual ArtistType ArtistType { get; set; }

		public virtual Artist BaseVoicebank { get; set; }

		/// <summary>
		/// Whether it makes sense for this artist to have child voicebanks. 
		/// If false, the ChildVoicebanks collection should be ignored.
		/// In practice, only vocalists can have child voicebanks.
		/// This is mostly a performance optimization thing, so that the child voicebanks list doesn't need to be loaded unless needed.
		/// </summary>
		public virtual bool CanHaveChildVoicebanks {
			get {
				return (ArtistHelper.VocalistTypes.Contains(ArtistType) || ArtistType == ArtistType.Unknown) && BaseVoicebank == null;
			}
		}

		public virtual IList<Artist> ChildVoicebanks {
			get { return childVoicebanks; }
			set {
				ParamIs.NotNull(() => value);
				childVoicebanks = value;
			}
		}

		public virtual IList<ArtistComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual User CreatedBy {
			get { return createdBy; }
			set {
				ParamIs.NotNull(() => value);
				createdBy = value; 
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual string DefaultName {
			get {
				return TranslatedName.Default;
			}
		}

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual EntryType EntryType {
			get {
				return EntryType.Artist;
			}
		}

		public virtual IEnumerable<GroupForArtist> Groups {
			get {
				return AllGroups.Where(g => !g.Group.Deleted);
			}
		}

		/*public virtual IList<Artist> Hits {
			get { return hits; }
			set { hits = value; }
		}*/

		public virtual int Id { get; set; }

		/// <summary>
		/// Tests whether an artist is a valid base voicebank for this artist.
		/// 
		/// Base voicebank can be null (not set). 
		/// If set, the base voicebank must not have a base voicebank set, otherwise circular dependencies might occur.
		/// An artist cannot be the base voicebank of itself obviously.
		/// </summary>
		/// <param name="artist"></param>
		/// <returns></returns>
		public virtual bool IsValidBaseVoicebank(Artist artist) {
			return artist == null || (artist.BaseVoicebank == null && !this.Equals(artist));
		}

		public virtual TranslatedString TranslatedName {
			get { return Names.SortNames; }
		}

		public virtual IEnumerable<GroupForArtist> Members {
			get { return AllMembers.Where(m => !m.Member.Deleted); }
		}

		[Obsolete]
		public virtual string Name {
			get {
				return TranslatedName.Default;
			}
		}

		public virtual NameManager<ArtistName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		INameManager IEntryWithNames.Names {
			get { return Names; }
		}

		/// <summary>
		/// List of users who are verifid owners of this artist. Cannot be null.
		/// </summary>
		public virtual IList<OwnedArtistForUser> OwnerUsers {
			get { return ownerUsers; }
			set {
				ParamIs.NotNull(() => value);
				ownerUsers = value;
			}
		}

		public virtual PictureData Picture { get; set; }

		public virtual string PictureMime { get; set; }

		public virtual EntryPictureFileManager<ArtistPictureFile> Pictures {
			get { return pictureManager; }
			set {
				ParamIs.NotNull(() => value);
				pictureManager = value;
			}
		}

		public virtual IEnumerable<ArtistForSong> Songs {
			get {
				return AllSongs.Where(s => !s.Song.Deleted);
			}
		}

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<ArtistTagUsage> Tags {
			get { return tags; }
			set {
				ParamIs.NotNull(() => value);
				tags = value;
			}
		}

		/// <summary>
		/// List of users who follow this artist. Cannot be null.
		/// </summary>
		public virtual IList<ArtistForUser> Users {
			get { return users; }
			set {
				ParamIs.NotNull(() => value);
				users = value; 
			}
		}

		public virtual int Version { get; set; }

		public virtual IList<ArtistWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual IEnumerable<string> AllNames {
			get { return Names.AllValues; }
		}

		public virtual ArtistForAlbum AddAlbum(Album album) {
			return AddAlbum(album, false, ArtistRoles.Default);
		}

		public virtual ArtistForAlbum AddAlbum(Album album, bool support, ArtistRoles roles) {

			ParamIs.NotNull(() => album);

			// Check is too slow for now
			//if (HasAlbum(album))
			//	throw new InvalidOperationException(string.Format("{0} has already been added for {1}", album, this));

			var link = new ArtistForAlbum(album, this, support, roles);
			AllAlbums.Add(link);
			album.AllArtists.Add(link);

			return link;

		}

		public virtual GroupForArtist AddGroup(Artist grp) {

			ParamIs.NotNull(() => grp);

			var link = new GroupForArtist(grp, this);
			AllGroups.Add(link);
			grp.AllMembers.Add(link);

			return link;

		}

		public virtual GroupForArtist AddMember(Artist member) {

			ParamIs.NotNull(() => member);

			return member.AddGroup(this);

		}

		public virtual ArtistForSong AddSong(Song song) {
			return AddSong(song, false, ArtistRoles.Default);
		}

		public virtual ArtistForSong AddSong(Song song, bool support, ArtistRoles roles) {

			ParamIs.NotNull(() => song);

			var link = new ArtistForSong(song, this, support, roles);
			AllSongs.Add(link);
			song.AllArtists.Add(link);

			return link;

		}

		public virtual ArchivedArtistVersion CreateArchivedVersion(XDocument data, ArtistDiff diff, AgentLoginData author, ArtistArchiveReason reason, string notes) {

			var archived = new ArchivedArtistVersion(this, data, diff, author, Version, Status, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual ArtistComment CreateComment(string message, User author) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => author);

			var comment = new ArtistComment(this, message, author);
			Comments.Add(comment);

			return comment;

		}

		public virtual ArtistName CreateName(string val, ContentLanguageSelection language) {
			
			ParamIs.NotNullOrEmpty(() => val);

			var name = new ArtistName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

		}

		public virtual ArtistPictureFile CreatePicture(string name, string mime, User author) {

			var f = new ArtistPictureFile(name, mime, author, this);
			Pictures.Add(f);

			log.Info(string.Format("{0} created {1}", author, f));

			return f;

		}

		public virtual ArtistWebLink CreateWebLink(string description, string url, WebLinkCategory category) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrWhiteSpace(() => url);

			var link = new ArtistWebLink(this, description, url, category);
			WebLinks.Add(link);

			return link;

		}

		public virtual void Delete() {

			Deleted = true;

		}

		public virtual bool Equals(Artist another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as Artist);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual ArchivedArtistVersion GetLatestVersion() {
			return ArchivedVersionsManager.GetLatestVersion();
		}

		/// <summary>
		/// Checks whether this artist has a specific album.
		/// </summary>
		/// <param name="album">Album to be checked. Cannot be null.</param>
		/// <returns>True if the artist has the album. Otherwise false.</returns>
		/// <remarks>
		/// Warning: This check can be slow if the artist has too many albums and the collection needs to be loaded.
		/// Most of the time the check should be done from the album side, since usually albums have fewer artist links than artist have linked albums.
		/// </remarks>
		public virtual bool HasAlbum(Album album) {

			ParamIs.NotNull(() => album);

			return Albums.Any(a => a.Album.Equals(album));

		}

		public virtual bool HasGroup(Artist grp) {

			ParamIs.NotNull(() => grp);

			return Groups.Any(a => a.Group.Equals(grp));

		}

		public virtual bool HasMember(Artist member) {

			ParamIs.NotNull(() => member);

			return Members.Any(a => a.Member.Equals(member));

		}

		public virtual bool HasName(LocalizedString name) {

			ParamIs.NotNull(() => name);

			return Names.HasName(name);

		}

		public virtual bool HasOwnerUser(User user) {

			ParamIs.NotNull(() => user);

			return OwnerUsers.Any(a => a.User.Equals(user));

		}

		public virtual bool HasSong(Song song) {

			ParamIs.NotNull(() => song);

			return Songs.Any(a => a.Song.Equals(song));

		}

		public virtual bool HasWebLink(string url) {

			ParamIs.NotNull(() => url);

			return WebLinks.Any(w => w.Url == url);

		}

		public virtual void SetBaseVoicebank(Artist baseVoicebank) {
			
			if (Equals(BaseVoicebank, baseVoicebank))
				return;

			if (BaseVoicebank != null) {
				BaseVoicebank.ChildVoicebanks.Remove(this);
			}

			if (baseVoicebank != null) {
				baseVoicebank.ChildVoicebanks.Add(this);
			}

			BaseVoicebank = baseVoicebank;

		}

		[Obsolete]
		public virtual CollectionDiff<ArtistForAlbum, ArtistForAlbum> SyncAlbums(
			IEnumerable<AlbumForArtistEditContract> newAlbums, Func<AlbumForArtistEditContract, Album> albumGetter) {

			var albumDiff = CollectionHelper.Diff(Albums, newAlbums, (a1, a2) => (a1.Id == a2.ArtistForAlbumId));
			var created = new List<ArtistForAlbum>();

			foreach (var removed in albumDiff.Removed) {
				removed.Delete();
				AllAlbums.Remove(removed);
			}

			foreach (var added in albumDiff.Added) {

				var album = albumGetter(added);
				var link = AddAlbum(album);
				created.Add(link);

			}

			return new CollectionDiff<ArtistForAlbum, ArtistForAlbum>(created, albumDiff.Removed, albumDiff.Unchanged);

		}

		public override string ToString() {
			return string.Format("artist '{0}' [{1}]", DefaultName, Id);
		}

	}

}
