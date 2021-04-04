#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.ReleaseEvents;
using System.Threading.Tasks;

namespace VocaDb.Model.Domain.Albums
{
	public class Album : IEntryBase, IEntryWithNames<AlbumName>, IEntryWithVersions, IEntryWithStatus,
		IDeletableEntry, IEquatable<Album>, INameFactory<AlbumName>, IWebLinkFactory<AlbumWebLink>, IEntryWithArtistLinks<ArtistForAlbum>, IEntryWithTags<AlbumTagUsage>,
		IEntryWithLinks<AlbumWebLink>, IEntryWithComments<AlbumComment>, IEntryWithArtists
	{
		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;

		public static string ParseBarcode(string barcode)
		{
			return !string.IsNullOrEmpty(barcode) ? barcode.Replace(" ", string.Empty).Replace("-", string.Empty) : barcode;
		}

		public static bool TrackPropertiesEqual(SongInAlbum first, SongInAlbumEditContract second)
		{
			return first.DiscNumber == second.DiscNumber && first.TrackNumber == second.TrackNumber;
		}

		public static bool TrackArtistsEqual(Song first, SongInAlbumEditContract second)
		{
			if (first == null || second.IsCustomTrack)
				return true; // Cannot edit artists for custom tracks.

			return first.ArtistList.All(a => second.Artists.Any(a2 => a.Id == a2.Id))
				   && second.Artists.All(a => first.ArtistList.Any(a2 => a.Id == a2.Id));
		}

		private ArchivedVersionManager<ArchivedAlbumVersion, AlbumEditableFields> _archivedVersions = new();
		private TranslatedStringWithDefault _artistString;
		private IList<ArtistForAlbum> _artists = new List<ArtistForAlbum>();
		private IList<AlbumComment> _comments = new List<AlbumComment>();
		private EnglishTranslatedString _description;
		private IList<AlbumDiscProperties> _discs = new List<AlbumDiscProperties>();
		private IList<AlbumHit> _hits = new List<AlbumHit>();
		private IList<AlbumIdentifier> _identifiers = new List<AlbumIdentifier>();
		private NameManager<AlbumName> _names = new();
		private AlbumRelease _originalRelease = new();
		private IList<OtherArtistForAlbum> _otherArtists = new List<OtherArtistForAlbum>();
		private EntryPictureFileManager<AlbumPictureFile> _pictureManager = new();
		private IList<PVForAlbum> _pvs = new List<PVForAlbum>();
		private IList<AlbumReview> _reviews = new List<AlbumReview>();
		private IList<SongInAlbum> _songs = new List<SongInAlbum>();
		private TagManager<AlbumTagUsage> _tags = new();
		private IList<AlbumForUser> _userCollections = new List<AlbumForUser>();
		private IList<AlbumWebLink> _webLinks = new List<AlbumWebLink>();

		public Album()
		{
			ArtistString = new TranslatedStringWithDefault(string.Empty, string.Empty, string.Empty, string.Empty);
			CreateDate = DateTime.Now;
			Deleted = false;
			Description = new EnglishTranslatedString();
			DiscType = DiscType.Album;
			OriginalRelease = new AlbumRelease();
			Status = EntryStatus.Draft;
		}

#nullable enable
		public Album(LocalizedString name)
			: this()
		{
			ParamIs.NotNull(() => name);

			Names.Add(new AlbumName(this, name));
		}

		public Album(TranslatedString translatedName)
			: this()
		{
			ParamIs.NotNull(() => translatedName);

			foreach (var name in translatedName.AllLocalized)
				Names.Add(new AlbumName(this, name));
		}
#nullable disable

		public virtual IList<ArtistForAlbum> AllArtists
		{
			get => _artists;
			set
			{
				ParamIs.NotNull(() => value);
				_artists = value;
			}
		}

		public virtual IEnumerable<string> AllNames => Names.AllValues;

		public virtual IList<SongInAlbum> AllSongs
		{
			get => _songs;
			set
			{
				ParamIs.NotNull(() => value);
				_songs = value;
			}
		}

		public virtual bool AllowNotifications => true;

		public virtual ArchivedVersionManager<ArchivedAlbumVersion, AlbumEditableFields> ArchivedVersionsManager
		{
			get => _archivedVersions;
			set
			{
				ParamIs.NotNull(() => value);
				_archivedVersions = value;
			}
		}

		public virtual IEnumerable<ArtistForAlbum> Artists => AllArtists.Where(a => a.Artist == null || !a.Artist.Deleted);

		public virtual IEnumerable<Artist> ArtistList
		{
			get
			{
				return Artists
					.Where(a => a.Artist != null)
					.Select(a => a.Artist);
			}
		}

		public virtual TranslatedStringWithDefault ArtistString
		{
			get => _artistString;
			set
			{
				ParamIs.NotNull(() => value);
				_artistString = value;
			}
		}

		public virtual IList<AlbumComment> AllComments
		{
			get => _comments;
			set
			{
				ParamIs.NotNull(() => value);
				_comments = value;
			}
		}

		public virtual IEnumerable<AlbumComment> Comments => AllComments.Where(c => !c.Deleted);

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		/// <summary>
		/// Album cover picture (main picture) as database BLOB.
		/// Also contains optional thumbnail.
		/// This field is lazy-loaded.
		/// Can be null if there is no picture.
		/// Try to avoid accessing this field directly to confirm whether picture exists. Instead, check the MIME.
		/// </summary>
		public virtual PictureData CoverPictureData { get; set; }

		/// <summary>
		/// Album cover picture (main picture) MIME.
		/// Can be null if there is no picture.
		/// </summary>
		public virtual string CoverPictureMime { get; set; }

		public virtual DateTime CreateDate { get; set; }

		public virtual string DefaultName => TranslatedName.Default;

		public virtual bool Deleted { get; set; }

		public virtual EnglishTranslatedString Description
		{
			get => _description;
			set
			{
				ParamIs.NotNull(() => value);
				_description = value;
			}
		}

		public virtual IList<AlbumDiscProperties> Discs
		{
			get => _discs;
			set
			{
				ParamIs.NotNull(() => value);
				_discs = value;
			}
		}

		public virtual DiscType DiscType { get; set; }

		public virtual EntryType EntryType => EntryType.Album;

		public virtual IList<AlbumHit> Hits
		{
			get => _hits;
			set => _hits = value;
		}

		public virtual int Id { get; set; }

		public virtual IList<AlbumIdentifier> Identifiers
		{
			get => _identifiers;
			set
			{
				ParamIs.NotNull(() => value);
				_identifiers = value;
			}
		}

		/// <summary>
		/// Album's artist string is "various artists".
		/// </summary>
		public virtual bool IsVariousArtists => ArtistString.Default == ArtistHelper.VariousArtists;

		/// <summary>
		/// Gets the ordinal number of the last disc for this album, starting from 1.
		/// </summary>
		public virtual int LastDiscNumber => (Songs.Any() ? Songs.Max(s => s.DiscNumber) : 1);

		public virtual AlbumReview LastReview => Reviews.OrderByDescending(r => r.Created).FirstOrDefault();

		public virtual TranslatedString TranslatedName => Names.SortNames;

		public virtual NameManager<AlbumName> Names
		{
			get => _names;
			set
			{
				ParamIs.NotNull(() => value);
				_names = value;
			}
		}

		INameManager<AlbumName> IEntryWithNames<AlbumName>.Names => Names;

		INameManager IEntryWithNames.Names => Names;

		public virtual AlbumRelease OriginalRelease
		{
			get => _originalRelease;
			set
			{
				_originalRelease = value;
			}
		}

		/// <summary>
		/// Original release date. Cannot be null.
		/// </summary>
		public virtual OptionalDateTime OriginalReleaseDate
		{
			get
			{
				if (OriginalRelease == null)
					OriginalRelease = new AlbumRelease();

				if (OriginalRelease.ReleaseDate == null)
					OriginalRelease.ReleaseDate = new OptionalDateTime();

				return OriginalRelease.ReleaseDate;
			}
		}

		public virtual ReleaseEvent OriginalReleaseEvent
		{
			get
			{
				if (OriginalRelease == null)
					return null;

				return OriginalRelease.ReleaseEvent;
			}
			set
			{
				if (OriginalRelease == null)
					OriginalRelease = new AlbumRelease();

				OriginalRelease.ReleaseEvent = value;
			}
		}

		public virtual IList<OtherArtistForAlbum> OtherArtists
		{
			get => _otherArtists;
			set
			{
				ParamIs.NotNull(() => value);
				_otherArtists = value;
			}
		}

		public virtual string PersonalDescriptionText { get; set; }
		public virtual Artist PersonalDescriptionAuthor => PersonalDescriptionAuthorId != null ? ArtistList.FirstOrDefault(a => a.Id == PersonalDescriptionAuthorId) : null;
		public virtual int? PersonalDescriptionAuthorId { get; set; }

		public virtual EntryPictureFileManager<AlbumPictureFile> Pictures
		{
			get => _pictureManager;
			set
			{
				ParamIs.NotNull(() => value);
				_pictureManager = value;
			}
		}

		public virtual IList<PVForAlbum> PVs
		{
			get => _pvs;
			set
			{
				ParamIs.NotNull(() => value);
				_pvs = value;
			}
		}

		/// <summary>
		/// Rating average as integer, basically the decimal number multiplied by 100.
		/// Thus, the scale is from 100 to 500 for rated albums, while 0 means there are no ratings.
		/// This field is mapped to the DB.
		/// </summary>
		public virtual int RatingAverageInt { get; set; }

		/// <summary>
		/// Rating average. Scale is from 1.00 to 5.00 with rated albums, with precision of 2 decimals. 
		/// 0.00 means there are no ratings.
		/// This field is not mapped to the DB and thus cannot be used in queries.
		/// </summary>
		public virtual double RatingAverage
		{
			get => Math.Round(RatingAverageInt / 100.0f, 2);
			set
			{
				RatingAverageInt = (int)(value * 100);
			}
		}

		public virtual int RatingCount { get; set; }

		public virtual int RatingTotal { get; set; }

		public virtual IList<AlbumReview> AllReviews
		{
			get => _reviews;
			set
			{
				ParamIs.NotNull(() => value);
				_reviews = value;
			}
		}

		public virtual IEnumerable<AlbumReview> Reviews => AllReviews.Where(r => !r.Deleted);

		public virtual IEnumerable<SongInAlbum> Songs => AllSongs.Where(s => s.Song == null || !s.Song.Deleted);

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<AlbumTagUsage> Tags
		{
			get => _tags;
			set
			{
				ParamIs.NotNull(() => value);
				_tags = value;
			}
		}

		ITagManager IEntryWithTags.Tags => Tags;

		public virtual EntryThumbMain Thumb => !string.IsNullOrEmpty(CoverPictureMime) ? new EntryThumbMain(this, CoverPictureMime) : null;

		public virtual IList<AlbumForUser> UserCollections
		{
			get => _userCollections;
			set
			{
				ParamIs.NotNull(() => value);
				_userCollections = value;
			}
		}

		public virtual int Version { get; set; }

		public virtual IList<AlbumWebLink> WebLinks
		{
			get => _webLinks;
			set
			{
				ParamIs.NotNull(() => value);
				_webLinks = value;
			}
		}

#nullable enable
		public virtual ArtistForAlbum AddArtist(Artist artist)
		{
			ParamIs.NotNull(() => artist);

			return artist.AddAlbum(this);
		}

		public virtual ArtistForAlbum AddArtist(Artist artist, bool isSupport, ArtistRoles roles)
		{
			ParamIs.NotNull(() => artist);

			return artist.AddAlbum(this, isSupport, roles);
		}

		public virtual ArtistForAlbum AddArtist(string name, bool isSupport, ArtistRoles roles)
		{
			ParamIs.NotNullOrEmpty(() => name);

			var link = new ArtistForAlbum(this, name, isSupport, roles);

			AllArtists.Add(link);

			return link;
		}

		public virtual SongInAlbum AddSong(Song song, int trackNum, int discNum)
		{
			ParamIs.NotNull(() => song);

			var track = new SongInAlbum(song, this, trackNum, discNum);
			AllSongs.Add(track);
			song.AllAlbums.Add(track);

			return track;
		}

		public virtual SongInAlbum AddSong(string songName, int trackNum, int discNum)
		{
			ParamIs.NotNullOrEmpty(() => songName);

			var track = new SongInAlbum(songName, this, trackNum, discNum);
			AllSongs.Add(track);

			return track;
		}
#nullable disable

		public virtual ArchivedAlbumVersion CreateArchivedVersion(XDocument data, AlbumDiff diff, AgentLoginData author, AlbumArchiveReason reason, string notes)
		{
			var archived = new ArchivedAlbumVersion(this, data, diff, author, Version, Status, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;
		}

#nullable enable
		public virtual Comment CreateComment(string message, AgentLoginData loginData)
		{
			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new AlbumComment(this, message, loginData);
			AllComments.Add(comment);

			return comment;
		}

		public virtual AlbumName CreateName(string val, ContentLanguageSelection language)
		{
			ParamIs.NotNullOrEmpty(() => val);

			var name = new AlbumName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;
		}
#nullable disable

		public virtual AlbumPictureFile CreatePicture(string name, string mime, User author)
		{
			var f = new AlbumPictureFile(name, mime, author, this);
			Pictures.Add(f);

			return f;
		}

#nullable enable
		public virtual PVForAlbum CreatePV(PVContract contract)
		{
			ParamIs.NotNull(() => contract);

			var pv = new PVForAlbum(this, contract);
			PVs.Add(pv);

			return pv;
		}

		public virtual AlbumWebLink CreateWebLink(string description, string url, WebLinkCategory category, bool disabled)
		{
			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new AlbumWebLink(this, description, url, category, disabled);
			WebLinks.Add(link);

			return link;
		}
#nullable disable

		public virtual void Delete()
		{
			Deleted = true;
		}

		public virtual void DeleteArtistForAlbum(ArtistForAlbum artistForAlbum)
		{
			if (!artistForAlbum.Album.Equals(this))
				throw new ArgumentException("Artist is not attached to album", "artistForAlbum");

			AllArtists.Remove(artistForAlbum);

			if (artistForAlbum.Artist != null)
				artistForAlbum.Artist.AllAlbums.Remove(artistForAlbum);

			UpdateArtistString();
		}

		/// <summary>
		/// Cleans up all links to other entries so that this entry can be deleted.
		/// </summary>
		public virtual void DeleteLinks()
		{
			var artistLinks = AllArtists.ToArray();
			foreach (var artist in artistLinks)
				artist.Delete();

			var songLinks = AllSongs.ToArray();
			foreach (var song in songLinks)
				song.Delete();

			var users = UserCollections.ToArray();
			foreach (var user in users)
				user.Delete();

			Tags.DeleteUsages();

			// Archived versions and comments are cascaded
		}

		public virtual bool Equals(Album another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Album);
		}

		public virtual ArtistForAlbum GetArtistLink(Artist artist)
		{
			return Artists.FirstOrDefault(a => a.Artist != null && a.Artist.Equals(artist));
		}

		public virtual AlbumDiscProperties GetDisc(int discNumber)
		{
			return Discs.FirstOrDefault(d => d.DiscNumber == discNumber);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		/// <summary>
		/// Gets the next free track number for a particular disc on this album.
		/// </summary>
		/// <param name="discNum">Disc number, starting from 1.</param>
		/// <returns>Next free track number on the specified disc, starting from 1.</returns>
		public virtual int GetNextTrackNumber(int discNum)
		{
			return (Songs.Any(s => s.DiscNumber == discNum)
				? Songs.Where(s => s.DiscNumber == discNum).Max(s => s.TrackNumber) + 1 : 1);
		}

		public virtual SongInAlbum GetSongByTrackNum(int discNum, int trackNum)
		{
			return Songs.FirstOrDefault(s => s.DiscNumber == discNum && s.TrackNumber == trackNum);
		}

		/// <summary>
		/// Checks whether this album has a specific artist.
		/// </summary>
		/// <param name="artist">Artist to be checked. Can be null.</param>
		/// <returns>True if the artist has this album or artist was null. Otherwise false.</returns>
		public virtual bool HasArtist(Artist artist)
		{
			if (artist == null)
				return false;

			return Artists.Any(a => artist.Equals(a.Artist));
		}

#nullable enable
		/// <summary>
		/// Checks whether this album has a specific artist.
		/// </summary>
		/// <param name="artistForAlbum">Artist to be checked. Cannot be null.</param>
		/// <returns>True if the artist has this album. Otherwise false.</returns>
		public virtual bool HasArtistForAlbum(ArtistForAlbum artistForAlbum)
		{
			ParamIs.NotNull(() => artistForAlbum);

			return Artists.Any(a => a.ArtistLinkEquals(artistForAlbum));
		}

		public virtual bool HasName(LocalizedString name)
		{
			ParamIs.NotNull(() => name);

			return Names.HasName(name);
		}

		public virtual bool HasSong(Song song)
		{
			ParamIs.NotNull(() => song);

			return Songs.Any(a => song.Equals(a.Song));
		}

		public virtual bool HasWebLink(string url)
		{
			ParamIs.NotNull(() => url);

			return WebLinks.Any(w => w.Url == url);
		}

		public virtual bool IsInUserCollection(User user)
		{
			ParamIs.NotNull(() => user);

			return UserCollections.Any(w => w.User.Equals(user));
		}
#nullable disable

		/// <summary>
		/// Returns the index for the next track counted from a track index,
		/// handling multiple discs.
		/// This assumes that a track with the specified index exists.
		/// </summary>
		/// <param name="index">Track index from which to start counting.</param>
		/// <returns>Index of the next track. Empty if the index parameter specifies the first track on the first disc.</returns>
		public virtual TrackIndex NextTrackIndex(TrackIndex index)
		{
			// Last track on disc, move to the next disc.
			if (index.TrackNumber >= GetNextTrackNumber(index.DiscNumber) - 1)
			{
				return new TrackIndex(index.DiscNumber + 1, 1);
			}

			return new TrackIndex(index.DiscNumber, index.TrackNumber + 1);
		}

#nullable enable
		public virtual void OnSongDeleting(SongInAlbum songInAlbum)
		{
			ParamIs.NotNull(() => songInAlbum);

			if (!songInAlbum.Album.Equals(this))
				throw new ArgumentException("Song is not in album");

			foreach (var song in Songs.Where(song => song.TrackNumber > songInAlbum.TrackNumber))
			{
				song.TrackNumber--;
			}

			AllSongs.Remove(songInAlbum);
		}
#nullable disable

		/// <summary>
		/// Returns the index for the previous track counted from a track index,
		/// handling multiple discs.
		/// This assumes that a track with the specified index exists.
		/// </summary>
		/// <param name="index">Track index from which to start counting.</param>
		/// <returns>Index of the previous track. Empty if the index parameter specifies the first track on the first disc.</returns>
		public virtual TrackIndex PreviousTrackIndex(TrackIndex index)
		{
			if (index.TrackNumber == 1)
			{
				if (index.DiscNumber == 1)
				{
					return TrackIndex.Empty;
				}

				var discNum = index.DiscNumber - 1;
				var trackNum = GetNextTrackNumber(discNum) - 1;

				return new TrackIndex(discNum, trackNum);
			}

			return new TrackIndex(index.DiscNumber, index.TrackNumber - 1);
		}

		public virtual async Task<CollectionDiffWithValue<ArtistForAlbum, ArtistForAlbum>> SyncArtists(
			IEnumerable<ArtistForAlbumContract> newArtists, Func<ArtistContract, Task<Artist>> artistGetter)
		{
			var create = new Func<ArtistForAlbumContract, Task<ArtistForAlbum>>(async contract =>
			{
				ArtistForAlbum link = null;

				if (contract.Artist != null)
				{
					var artist = await artistGetter(contract.Artist);

					if (!HasArtist(artist))
					{
						link = AddArtist(artist, contract.IsSupport, contract.Roles);
						link.Name = contract.IsCustomName ? contract.Name : null;
					}
				}
				else
				{
					link = AddArtist(contract.Name, contract.IsSupport, contract.Roles);
				}

				return link;
			});

			var delete = new Func<ArtistForAlbum, Task>(link =>
			{
				link.Delete();
				return Task.CompletedTask;
			});

			var update = new Func<ArtistForAlbum, ArtistForAlbumContract, Task<bool>>((old, newEntry) =>
			{
				if (!old.ContentEquals(newEntry))
				{
					old.IsSupport = newEntry.IsSupport;
					old.Roles = newEntry.Roles;
					old.Name = newEntry.IsCustomName ? newEntry.Name : null;
					return Task.FromResult(true);
				}
				else
				{
					return Task.FromResult(false);
				}
			});

			var diff = await CollectionHelper.SyncWithContentAsync(AllArtists, newArtists.ToArray(), (a1, a2) => a1.Id == a2.Id, create, update, delete);

			if (diff.Changed)
			{
				UpdateArtistString();
			}

			return diff;
		}

		public virtual async Task<CollectionDiffWithValue<AlbumDiscProperties, AlbumDiscProperties>> SyncDiscs(AlbumDiscPropertiesContract[] newDiscs)
		{
			for (var i = 0; i < newDiscs.Length; ++i)
			{
				newDiscs[i].DiscNumber = i + 1;
			}

			Func<AlbumDiscProperties, AlbumDiscPropertiesContract, bool> idEquality = ((i1, i2) => i1.Id == i2.Id);
			Func<AlbumDiscProperties, AlbumDiscPropertiesContract, bool> valueEquality = ((i1, i2) => i1.DiscNumber.Equals(i2.DiscNumber) && string.Equals(i1.Name, i2.Name) && i1.MediaType.Equals(i2.MediaType));

			Func<AlbumDiscPropertiesContract, Task<AlbumDiscProperties>> create = (data =>
			{
				var disc = new AlbumDiscProperties(this, data);
				Discs.Add(disc);
				return Task.FromResult(disc);
			});

			Func<AlbumDiscProperties, AlbumDiscPropertiesContract, Task<bool>> update = ((disc, data) =>
			{
				if (!valueEquality(disc, data))
				{
					disc.CopyContentFrom(data);
					return Task.FromResult(true);
				}
				return Task.FromResult(false);
			});

			Func<AlbumDiscProperties, Task> remove = (disc =>
			{
				Discs.Remove(disc);
				return Task.CompletedTask;
			});

			var diff = await CollectionHelper.SyncWithContentAsync(Discs, newDiscs, idEquality, create, update, remove);
			return diff;
		}

		public virtual CollectionDiff<AlbumIdentifier, AlbumIdentifier> SyncIdentifiers(string[] newIdentifiers)
		{
			Func<AlbumIdentifier, string, bool> equality = ((i1, i2) => i1.Value == i2);
			Func<string, AlbumIdentifier> create = (data =>
			{
				var id = new AlbumIdentifier(this, data);
				Identifiers.Add(id);
				return id;
			});

			var diff = CollectionHelper.Sync(Identifiers, newIdentifiers, equality, create);
			return diff;
		}

#nullable enable
		public virtual CollectionDiffWithValue<PVForAlbum, PVForAlbum> SyncPVs(IEnumerable<PVContract> newPVs)
		{
			ParamIs.NotNull(() => newPVs);

			var diff = CollectionHelper.Diff(PVs, newPVs, (n1, n2) => n1.Id == n2.Id);
			var created = new List<PVForAlbum>();
			var edited = new List<PVForAlbum>();

			foreach (var n in diff.Removed)
			{
				PVs.Remove(n);
			}

			foreach (var newEntry in diff.Added)
			{
				var l = CreatePV(newEntry);
				created.Add(l);
			}

			foreach (var linkEntry in diff.Unchanged)
			{
				var entry = linkEntry;
				var newEntry = newPVs.First(e => e.Id == entry.Id);

				if (!entry.ContentEquals(newEntry))
				{
					linkEntry.CopyMetaFrom(newEntry);
					edited.Add(linkEntry);
				}
			}

			return new CollectionDiffWithValue<PVForAlbum, PVForAlbum>(created, diff.Removed, diff.Unchanged, edited);
		}
#nullable disable

		public virtual async Task<CollectionDiffWithValue<SongInAlbum, SongInAlbum>> SyncSongs(
			IEnumerable<SongInAlbumEditContract> newTracks, Func<SongInAlbumEditContract, Task<Song>> songGetter,
			Func<Song, ArtistContract[], Task> updateArtistsFunc)
		{
			var diff = CollectionHelper.Diff(Songs, newTracks, (n1, n2) => n1.Id == n2.SongInAlbumId);
			var created = new List<SongInAlbum>();
			var edited = new List<SongInAlbum>();

			foreach (var n in diff.Removed)
			{
				n.Delete();
			}

			foreach (var newEntry in diff.Added)
			{
				SongInAlbum link;

				if (!newEntry.IsCustomTrack)
				{
					var song = await songGetter(newEntry);

					if (!TrackArtistsEqual(song, newEntry))
						await updateArtistsFunc(song, newEntry.Artists);

					link = AddSong(song, newEntry.TrackNumber, newEntry.DiscNumber);
				}
				else
				{
					link = AddSong(newEntry.SongName, newEntry.TrackNumber, newEntry.DiscNumber);
				}

				created.Add(link);
			}

			foreach (var linkEntry in diff.Unchanged)
			{
				var entry = linkEntry;
				var newEntry = newTracks.First(e => e.SongInAlbumId == entry.Id);

				if (!TrackPropertiesEqual(linkEntry, newEntry))
				{
					linkEntry.DiscNumber = newEntry.DiscNumber;
					linkEntry.TrackNumber = newEntry.TrackNumber;
					edited.Add(linkEntry);
				}

				if (!TrackArtistsEqual(linkEntry.Song, newEntry))
					await updateArtistsFunc(linkEntry.Song, newEntry.Artists);
			}

			return new CollectionDiffWithValue<SongInAlbum, SongInAlbum>(created, diff.Removed, diff.Unchanged, edited);
		}

		public override string ToString()
		{
			return $"album '{DefaultName}' [{Id}]";
		}

		public virtual void UpdateArtistString()
		{
			ArtistString = ArtistHelper.GetArtistString(Artists, AlbumHelper.GetContentFocus(DiscType));
		}

		public virtual void UpdateRatingTotals()
		{
			RatingCount = UserCollections.Where(a => a.Rating != AlbumForUser.NotRated).Count();
			RatingTotal = UserCollections.Sum(a => a.Rating);
			RatingAverageInt = RatingCount > 0 ? (RatingTotal * 100 / RatingCount) : 0;
		}
	}
}
