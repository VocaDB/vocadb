using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using System.Xml.Linq;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Domain.Songs {

	public class Song : IEntryBase, IEntryWithNames<SongName>, IEntryWithArtistLinks<ArtistForSong>, 
		IEntryWithTags<SongTagUsage>,
		IEntryWithVersions, IEntryWithStatus, IDeletableEntry, INameFactory<SongName>, IWebLinkFactory<SongWebLink>, IEquatable<Song>, IEntryWithComments<SongComment>, 
		IEntryWithLinks<SongWebLink>, IEntryWithArtists {

		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		private IList<SongInAlbum> albums = new List<SongInAlbum>();
		private IList<Song> alternateVersions = new List<Song>();
		private ArchivedVersionManager<ArchivedSongVersion, SongEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedSongVersion, SongEditableFields>();
		private TranslatedStringWithDefault artistString;
		private IList<ArtistForSong> artists = new List<ArtistForSong>();
		private IList<SongComment> comments = new List<SongComment>();
		private IList<SongHit> hits = new List<SongHit>();
		private IList<SongInList> lists = new List<SongInList>();
		private IList<LyricsForSong> lyrics = new List<LyricsForSong>();
		private NameManager<SongName> names = new NameManager<SongName>();
		private EnglishTranslatedString notes;
		private PVManager<PVForSong> pvs = new PVManager<PVForSong>();
		private TagManager<SongTagUsage> tags = new TagManager<SongTagUsage>();
		private IList<FavoriteSongForUser> userFavorites = new List<FavoriteSongForUser>();
		private IList<SongWebLink> webLinks = new List<SongWebLink>();

		public virtual int GetLengthFromPV() {

			var pv = PVs.FirstOrDefault(p => p.Length > 0);
			return (pv != null ? pv.Length : 0);

		}

		public Song() {
			ArtistString = new TranslatedStringWithDefault(string.Empty, string.Empty, string.Empty, string.Empty);
			CreateDate = DateTime.Now;
			Deleted = false;
			Notes = new EnglishTranslatedString();
			PVServices = PVServices.Nothing;
			SongType = SongType.Unspecified;
			Status = EntryStatus.Draft;
		}

		public Song(LocalizedString name)
			: this() {

			ParamIs.NotNull(() => name);

			Names.Add(new SongName(this, name));

		}

		public Song(TranslatedString translatedName)
			: this() {

			ParamIs.NotNull(() => translatedName);

			foreach (var name in translatedName.AllLocalized)
				Names.Add(new SongName(this, name));

		}

		/// <summary>
		/// List of album links for this song. 
		/// The same album may appear multiple times, if the song was added more than once. 
		/// This list does not include deleted albums.
		/// Cannot be null.
		/// </summary>
		public virtual IEnumerable<SongInAlbum> Albums => AllAlbums.Where(a => !a.Album.Deleted);

		public virtual IList<SongInAlbum> AllAlbums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		public virtual IList<Song> AllAlternateVersions {
			get { return alternateVersions; }
			set {
				ParamIs.NotNull(() => value);
				alternateVersions = value;
			}
		}

		public virtual IEnumerable<string> AllNames => Names.AllValues;

		public virtual IList<ArtistForSong> AllArtists {
			get => artists;
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		/// <summary>
		/// List of alternate (derived) song versions. Does not include deleted songs.
		/// </summary>
		public virtual IEnumerable<Song> AlternateVersions => AllAlternateVersions.Where(a => !a.Deleted);

		public virtual ArchivedVersionManager<ArchivedSongVersion, SongEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		/// <summary>
		/// List of artists for this song. Does not include deleted artists.
		/// </summary>
		public virtual IEnumerable<Artist> ArtistList {
			get {
				return Artists
					.Where(a => a.Artist != null)
					.Select(a => a.Artist);
			}
		}

		/// <summary>
		/// List of artist links. Does not include deleted artists.
		/// </summary>
		public virtual IEnumerable<ArtistForSong> Artists => AllArtists.Where(a => a.Artist == null || !a.Artist.Deleted);

		public virtual TranslatedStringWithDefault ArtistString {
			get => artistString;
			set {
				ParamIs.NotNull(() => value);
				artistString = value;
			}
		}

		public virtual IList<SongComment> Comments {
			get => comments;
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		/// <summary>
		/// Date when this entry was created.
		/// </summary>
		public virtual DateTime CreateDate { get; set; }

		public virtual string DefaultName => TranslatedName.Default;

		public virtual bool Deleted { get; set; }

		public virtual EntryType EntryType => EntryType.Song;

		public virtual int FavoritedTimes { get; set; }

		/// <summary>
		/// Release date of the earliest album.
		/// </summary>
		public virtual DateTime? FirstAlbumDate {
			get {

				// Sanity check
				var minDateLimit = new DateTime(AppConfig.SiteSettings.MinAlbumYear, 1, 1);

				return Albums
					.Where(a => a.Album != null && a.Album.OriginalReleaseDate.IsFullDate)
					.Select(a => a.Album.OriginalReleaseDate.ToDateTime())
					.Where(d => d > minDateLimit)
					.MinOrNull();

			}
		}

		/// <summary>
		/// Tests whether this song has an original version specified.
		/// This method also tests that the song type isn't "Original", because original songs aren't supposed to have parent songs.
		/// </summary>
		public virtual bool HasOriginalVersion => SongType != SongType.Original && OriginalVersion != null;

		public virtual IList<SongHit> Hits {
			get { return hits; }
			set { hits = value; }
		}

		public virtual int Id { get; set; }

		/// <summary>
		/// Song length in seconds. If 0, that means no length is saved.
		/// </summary>
		public virtual int LengthSeconds { get; set; }

		public virtual IList<SongInList> ListLinks {
			get { return lists; }
			set {
				ParamIs.NotNull(() => value);
				lists = value;
			}
		}

		public virtual IList<LyricsForSong> Lyrics {
			get { return lyrics; }
			set {
				ParamIs.NotNull(() => value);
				lyrics = value;
			}
		}

		/// <summary>
		/// Lyrics for this song, either from the song entry itself, or its original version.
		/// </summary>
		/// <param name="specialTags">Special tags. Can be null, which will cause no lyrics to be inherited.</param>
		/// <param name="allowInstrumental">
		/// Whether to allow inheriting lyrics for instrumental songs.
		/// This is mostly the case when the instrumental version is in the middle, for example original -> instrumental -> cover (with lyrics)
		/// </param>
		/// <param name="levels">Current level of traversing the parent chain.</param>
		private IList<LyricsForSong> GetLyricsFromParents(ISpecialTags specialTags, bool allowInstrumental, int levels) {

			int maxLevels = 10;

			if (specialTags != null 
				&& (allowInstrumental || SongType != SongType.Instrumental)
				&& HasOriginalVersion 
				&& !OriginalVersion.Deleted
				&& !Lyrics.Any()
				&& !Tags.HasTag(specialTags.ChangedLyrics)
				&& (allowInstrumental || !Tags.HasTag(specialTags.Instrumental))
				&& levels < maxLevels) {

				return OriginalVersion.GetLyricsFromParents(specialTags, true, levels + 1);

			}

			return Lyrics;

		}

		/// <summary>
		/// Lyrics for this song, either from the song entry itself, or its original version.
		/// </summary>
		/// <param name="specialTags">Special tags. Can be null, which will cause no lyrics to be inherited.</param>
		public virtual IList<LyricsForSong> GetLyricsFromParents(ISpecialTags specialTags) {

			return GetLyricsFromParents(specialTags, false, 0);

		}

		public virtual NameManager<SongName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		INameManager<SongName> IEntryWithNames<SongName>.Names => Names;

		INameManager IEntryWithNames.Names => Names;

		public virtual EnglishTranslatedString Notes {
			get { return notes; }
			set {
				ParamIs.NotNull(() => value);
				notes = value; 
			}
		}

		/// <summary>
		/// List of albums this song appears on. 
		/// Duplicates are removed. Does not include deleted albums.
		/// Cannot be null.
		/// </summary>
		public virtual IEnumerable<Album> OnAlbums => Albums.Select(a => a.Album).Distinct();

		/// <summary>
		/// Id of the original (parent) version of this song.
		/// Can be null.
		/// Use <see cref="HasOriginalVersion"/> as a shortcut to check both song type and existence of original version.
		/// </summary>
		public virtual Song OriginalVersion { get; set; }

		/// <summary>
		/// Date this song was first published.
		/// Only includes the date component, no time for now.
		/// Should always be in UTC.
		/// </summary>
		public virtual Date PublishDate { get; set; }

		public virtual PVManager<PVForSong> PVs {
			get => pvs;
			set {
				ParamIs.NotNull(() => value);
				pvs = value;
			}
		}

		/// <summary>
		/// Bitarray of PV services for this song. Persisted as a bitfield.
		/// This list does not include disabled PVs.
		/// </summary>
		public virtual PVServices PVServices { get; set; }

		public virtual int RatingScore { get; set; }

		public virtual ReleaseEvent ReleaseEvent { get; set; }

		public virtual string PersonalDescriptionText { get; set; }

		public virtual Artist PersonalDescriptionAuthor => PersonalDescriptionAuthorId != null ? ArtistList.FirstOrDefault(a => a.Id == PersonalDescriptionAuthorId) : null;

		public virtual int? PersonalDescriptionAuthorId { get; set; }

		public virtual SongType SongType { get; set; }

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<SongTagUsage> Tags {
			get => tags;
			set {
				ParamIs.NotNull(() => value);
				tags = value;
			}
		}

		ITagManager IEntryWithTags.Tags => Tags;

		/// <summary>
		/// Absolute URL to primary thumbnail for this song, based on the PVs list.
		/// For example, http://tn-skr1.smilevideo.jp/smile?i=12849032.
		/// Can be null or empty if no thumbnail is available.
		/// Also see <see cref="GetThumbUrl"/>.
		/// </summary>
		public virtual string ThumbUrl { get; set; }

		public virtual TranslatedString TranslatedName => Names.SortNames;

		/// <summary>
		/// NicoNicoDouga Id for the PV (for example sm12850213). Is unique, but can be null.
		/// </summary>
		public virtual string NicoId { get; set; }

		public virtual IList<FavoriteSongForUser> UserFavorites {
			get { return userFavorites; }
			set {
				ParamIs.NotNull(() => value);
				userFavorites = value;
			}
		}

		public virtual int Version { get; set; }

		public virtual IList<SongWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual ArtistForSong AddArtist(Artist artist) {
			return AddArtist(artist, false, ArtistRoles.Default);
		}

		public virtual ArtistForSong AddArtist(Artist artist, bool support, ArtistRoles roles) {
			ParamIs.NotNull(() => artist);
			return artist.AddSong(this, support, roles);
		}

		public virtual ArtistForSong AddArtist(string name, bool isSupport, ArtistRoles roles) {

			ParamIs.NotNullOrEmpty(() => name);

			var link = new ArtistForSong(this, name, isSupport, roles);

			AllArtists.Add(link);

			return link;

		}
		public virtual void AddAlternateVersion(Song song) {

			ParamIs.NotNull(() => song);

			if (song.OriginalVersion != null)
				song.OriginalVersion.AllAlternateVersions.Remove(song);

			AllAlternateVersions.Add(song);
			song.OriginalVersion = this;

		}

		/// <summary>
		/// Adds a tag to the song.
		/// First checks if the tag has already been added.
		/// No votes will be added, just the usage.
		/// </summary>
		/// <param name="tag">Tag to be added. Cannot be null.</param>
		/// <returns>
		/// Result of usage addition. Cannot be null.
		/// If the usage was added, it doesn't any have votes.
		/// </returns>
		public virtual CollectionAddResult<SongTagUsage> AddTag(Tag tag) {
			
			ParamIs.NotNull(() => tag);

			if (Tags.HasTag(tag))
				return CollectionAddResult.Create(Tags.GetTagUsage(tag), false);

			var usage = new SongTagUsage(this, tag);
			Tags.Usages.Add(usage);
			tag.UsageCount++;

			return CollectionAddResult.Create(usage, true);

		}

		public virtual ArchivedSongVersion CreateArchivedVersion(XDocument data, SongDiff diff, AgentLoginData author, SongArchiveReason reason, string notes) {

			var archived = new ArchivedSongVersion(this, data, diff, author, Version, Status, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual Comment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new SongComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual LyricsForSong CreateLyrics(LyricsForSongContract lyrics) {
			ParamIs.NotNull(() => lyrics);
			return CreateLyrics(lyrics.Value, lyrics.Source, lyrics.URL, lyrics.TranslationType, lyrics.CultureCode);
		}

		public virtual LyricsForSong CreateLyrics(string val, string source, string url, TranslationType translationType, string cultureCode) {
			
			ParamIs.NotNullOrEmpty(() => val);
			ParamIs.NotNull(() => source);
			ParamIs.NotNull(() => url);

			var entry = new LyricsForSong(this, val, source, url, translationType, cultureCode);
			Lyrics.Add(entry);

			return entry;

		}

		public virtual SongName CreateName(string val, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => val);

			return CreateName(new LocalizedString(val, language));

		}

		public virtual SongName CreateName(LocalizedString localizedString) {

			ParamIs.NotNull(() => localizedString);

			var name = new SongName(this, localizedString);
			Names.Add(name);

			return name;

		}

		public virtual PVForSong CreatePV(PVContract contract) {

			ParamIs.NotNull(() => contract);

			var pv = new PVForSong(this, contract);
			PVs.Add(pv);

			UpdateNicoId();
			UpdatePVServices();

			if (LengthSeconds <= 0)
				LengthSeconds = GetLengthFromPV();

			return pv;

		}

		public virtual SongWebLink CreateWebLink(string description, string url, WebLinkCategory category) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new SongWebLink(this, description, url, category);
			WebLinks.Add(link);

			return link;

		}

		public virtual void Delete() {

			Deleted = true;

		}

		public virtual void DeleteArtistForSong(ArtistForSong artistForSong) {

			if (!artistForSong.Song.Equals(this))
				throw new ArgumentException("Artist is not attached to song", "artistForSong");

			artistForSong.Delete();

		}

		public virtual bool Equals(Song another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as Song);
		}

		public virtual ArtistForSong GetArtistLink(Artist artist) {
			return Artists.FirstOrDefault(a => a.Artist != null && a.Artist.Equals(artist));
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual ArchivedSongVersion GetLatestVersion() {
			return ArchivedVersionsManager.GetLatestVersion();
		}

		/// <summary>
		/// Gets primary thumbnail URL for this song.
		/// For new songs this is saved directly to the song entry, but for older entries the PVs list needs to be loaded.
		/// Therefore, there might be some performance penalty.
		/// See <see cref="ThumbUrl"/>.
		/// </summary>
		/// <returns>
		/// Absolute URL to song thumbnail. For example, http://tn-skr1.smilevideo.jp/smile?i=12849032. 
		/// Can be null or empty if no thumbnail is available.
		/// </returns>
		public virtual string GetThumbUrl() {
			return !string.IsNullOrEmpty(ThumbUrl) ? ThumbUrl : VideoServiceHelper.GetThumbUrl(PVs.PVs);
		}

		public virtual bool HasArtist(Artist artist) {

			return ArtistList.Contains(artist);

		}

		/// <summary>
		/// Checks whether this song has a specific artist.
		/// </summary>
		/// <param name="artistLink">Artist to be checked. Cannot be null.</param>
		/// <returns>True if the artist has this album. Otherwise false.</returns>
		public virtual bool HasArtistLink(ArtistForSong artistLink) {

			ParamIs.NotNull(() => artistLink);

			return Artists.Any(a => a.ArtistLinkEquals(artistLink));

		}

		public virtual bool HasName(LocalizedString name) {

			ParamIs.NotNull(() => name);

			return Names.HasName(name);

		}

		public virtual bool HasPV(PVService service, string pvId) {

			ParamIs.NotNullOrEmpty(() => pvId);

			return PVs.Any(p => p.Service == service && p.PVId == pvId);

		}

		public virtual bool HasWebLink(string url) {

			ParamIs.NotNull(() => url);

			return WebLinks.Any(w => w.Url == url);

		}

		public virtual bool IsOnAlbum(Album album) {

			ParamIs.NotNull(() => album);

			return Albums.Any(a => a.Album.Equals(album));

		}

		public virtual bool IsFavoritedBy(User user) {

			ParamIs.NotNull(() => user);

			return UserFavorites.Any(a => a.User.Equals(user));

		}

		public virtual ArtistForSong RemoveArtist(Artist artist) {

			var link = Artists.First(a => a.Artist.Equals(artist));

			if (link == null)
				return null;

			DeleteArtistForSong(link);

			return link;

		}

		public virtual void SetOriginalVersion(Song original) {

			OriginalVersion?.AllAlternateVersions.Remove(this);
			OriginalVersion = original;
			original?.AllAlternateVersions.Add(this);

		}

		public virtual void SetReleaseEvent(ReleaseEvent releaseEvent) {
			
			if (Equals(ReleaseEvent, releaseEvent)) {
				return;
			}

			ReleaseEvent?.AllSongs.Remove(this);
			ReleaseEvent = releaseEvent;
			ReleaseEvent?.AllSongs.Add(this);

		}

		public virtual CollectionDiff<ArtistForSong, ArtistForSong> SyncArtists(IEnumerable<ArtistContract> newArtists, Func<ArtistContract[], Artist[]> artistGetter) {

			var realArtists = Artists.Where(a => a.Artist != null).ToArray();
			var artistDiff = CollectionHelper.Diff(realArtists, newArtists, (a, a2) => a.Artist.Id == a2.Id);
			var created = new List<ArtistForSong>();

			if (artistDiff.Added.Any()) {

				var addedArtists = artistGetter(artistDiff.Added);

				foreach (var artist in addedArtists) {
					if (!HasArtist(artist)) {
						created.Add(AddArtist(artist));
					}
				}
				
			}

			foreach (var removed in artistDiff.Removed) {
				removed.Delete();
			}

			UpdateArtistString();

			return new CollectionDiff<ArtistForSong, ArtistForSong>(created, artistDiff.Removed, artistDiff.Unchanged);

		}

		public virtual CollectionDiffWithValue<ArtistForSong, ArtistForSong> SyncArtists(
			IEnumerable<ArtistForSongContract> newArtists, Func<ArtistForSongContract, Artist> artistGetter) {

			ParamIs.NotNull(() => newArtists);

			var diff = CollectionHelper.Diff(AllArtists, newArtists, (n1, n2) => n1.Id == n2.Id); // Crawl AllArtists to remove deleted artists
			var created = new List<ArtistForSong>();
			var edited = new List<ArtistForSong>();

			foreach (var n in diff.Removed) {
				n.Delete();
			}

			foreach (var newEntry in diff.Added) {

				ArtistForSong l;

				if (newEntry.Artist != null) {

					var artist = artistGetter(newEntry);

					if (!HasArtist(artist)) {
						l = artist.AddSong(this, newEntry.IsSupport, newEntry.Roles);
						l.Name = newEntry.IsCustomName ? newEntry.Name : null;
						created.Add(l);
					}

				} else {
					l = AddArtist(newEntry.Name, newEntry.IsSupport, newEntry.Roles);
					created.Add(l);
				}

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newArtists.First(e => e.Id == entry.Id);

				if (!linkEntry.ContentEquals(newEntry)) {
					linkEntry.IsSupport = newEntry.IsSupport;
					linkEntry.Roles = newEntry.Roles;
					linkEntry.Name = newEntry.IsCustomName ? newEntry.Name : null;
					edited.Add(linkEntry);
				}

			}

			UpdateArtistString();

			return new CollectionDiffWithValue<ArtistForSong, ArtistForSong>(created, diff.Removed, diff.Unchanged, edited);

		}

		public virtual CollectionDiffWithValue<LyricsForSong, LyricsForSong> SyncLyrics(IEnumerable<LyricsForSongContract> newLyrics) {

			ParamIs.NotNull(() => newLyrics);

			var diff = CollectionHelper.Diff(Lyrics, newLyrics, (n1, n2) => n1.Id == n2.Id);
			var created = new List<LyricsForSong>();
			var edited = new List<LyricsForSong>();

			foreach (var n in diff.Removed) {
				Lyrics.Remove(n);
			}

			foreach (var newEntry in diff.Added) {

				var l = CreateLyrics(newEntry.Value, newEntry.Source, newEntry.URL, newEntry.TranslationType, newEntry.CultureCode);
				created.Add(l);

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newLyrics.First(e => e.Id == entry.Id);

				if (!entry.ContentEquals(newEntry)) {
					linkEntry.CultureCode = new OptionalCultureCode(newEntry.CultureCode);
					linkEntry.Source = newEntry.Source;
					linkEntry.TranslationType = newEntry.TranslationType;
					linkEntry.URL = newEntry.URL;
					linkEntry.Value = newEntry.Value;
					edited.Add(linkEntry);
				}

			}

			return new CollectionDiffWithValue<LyricsForSong, LyricsForSong>(created, diff.Removed, diff.Unchanged, edited);

		}

		/// <summary>
		/// Adds new PVs and removes deleted PVs.
		/// </summary>
		/// <param name="newPVs">Updated list of PVs. Cannot be null.</param>
		/// <returns>PVs list diff. Cannot be null.</returns>
		public virtual CollectionDiffWithValue<PVForSong, PVForSong> SyncPVs(IList<PVContract> newPVs) {

			var result = PVs.Sync(newPVs, CreatePV);

			if (result.Changed || string.IsNullOrEmpty(ThumbUrl)) {
				UpdateThumbUrl();				
			}

			if (result.Changed && !PublishDate.DateTime.HasValue) {
				UpdatePublishDateFromPVs();
			}

			new LocalFileManager().SyncLocalFilePVs(result, Id);

			return result;

		}

		public override string ToString() {
			return string.Format("song '{0}' [{1}]", DefaultName, Id);
		}

		public virtual void UpdateArtistString() {

			ArtistString = ArtistHelper.GetArtistString(Artists, SongHelper.GetContentFocus(SongType));

		}

		public virtual void UpdateFavoritedTimes() {

			FavoritedTimes = UserFavorites.Count;

		}

		public virtual void UpdateNicoId() {

			var originalPv = PVs.FirstOrDefault(p => p.Service == PVService.NicoNicoDouga && p.PVType == PVType.Original);

			NicoId = (originalPv != null ? originalPv.PVId : null);

		}

		public virtual void UpdatePublishDateFromPVs() {

			if (!PVs.Any())
				return;

			// Sanity check
			var minDateLimit = new DateTime(AppConfig.SiteSettings.MinAlbumYear, 1, 1); 

			// Original PVs that have a publish date
			var pvsWithDate = PVs.Where(p => p.PVType == PVType.Original && p.PublishDate.HasValue && p.PublishDate > minDateLimit).ToArray();

			// Lowest published (original) PV
			var minPvDate = pvsWithDate.Any() ? pvsWithDate.Min(p => p.PublishDate) : null;

			var minAlbumDate = FirstAlbumDate;

			var minDate = minAlbumDate.HasValue && minAlbumDate > minDateLimit && minAlbumDate < minPvDate ? minAlbumDate : minPvDate;

			PublishDate = minDate;

		}

		public virtual void UpdatePVServices() {

			var services = PVServices.Nothing;

			foreach (var service in EnumVal<PVService>.Values) {
				if (PVs.Any(p => !p.Disabled && p.Service == service))
					services |= (PVServices)service;
			}

			PVServices = services;

		}

		public virtual void UpdateThumbUrl() {
			ThumbUrl = VideoServiceHelper.GetThumbUrl(PVs.PVs);			
		}

	}

}
