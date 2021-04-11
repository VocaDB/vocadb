#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NLog;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using System;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Artists
{
	public class Artist : IEntryBase, IEntryWithNames<ArtistName>, IEntryWithVersions, IEntryWithStatus, IDeletableEntry,
		IEquatable<Artist>, INameFactory<ArtistName>, IWebLinkFactory<ArtistWebLink>, IEntryWithTags<ArtistTagUsage>, IEntryWithComments<ArtistComment>,
		IEntryWithLinks<ArtistWebLink>,
		IEntryWithArtists
	{
		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;

		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		private IList<ArtistForAlbum> _albums = new List<ArtistForAlbum>();
		private ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields> _archivedVersions = new();
		private IList<Artist> _childVoicebanks = new List<Artist>();
		private IList<ArtistComment> _comments = new List<ArtistComment>();
		private User _createdBy;
		private EnglishTranslatedString _description;
		private IList<ArtistForEvent> _events = new List<ArtistForEvent>();
		private IList<ArtistForArtist> _groups = new List<ArtistForArtist>();
		private IList<ArtistHit> _hits = new List<ArtistHit>();
		private IList<ArtistForArtist> _members = new List<ArtistForArtist>();
		private NameManager<ArtistName> _names = new();
		private IList<OwnedArtistForUser> _ownerUsers = new List<OwnedArtistForUser>();
		private EntryPictureFileManager<ArtistPictureFile> _pictureManager = new();
		private IList<ArtistForSong> _songs = new List<ArtistForSong>();
		private TagManager<ArtistTagUsage> _tags = new();
		private IList<ArtistForUser> _users = new List<ArtistForUser>();
		private IList<ArtistWebLink> _webLinks = new List<ArtistWebLink>();

		public Artist()
		{
			ArtistType = ArtistType.Unknown;
			CreateDate = DateTime.Now;
			Deleted = false;
			Description = new EnglishTranslatedString();
			Status = EntryStatus.Draft;
			Version = 0;
		}

#nullable enable
		public Artist(TranslatedString translatedName)
			: this()
		{
			ParamIs.NotNull(() => translatedName);

			foreach (var name in translatedName.AllLocalized)
				Names.Add(new ArtistName(this, name));
		}
#nullable disable

		/// <summary>
		/// List of albums for this artist (not including deleted). 
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IEnumerable<ArtistForAlbum> Albums => AllAlbums.Where(a => !a.Album.Deleted);

		/// <summary>
		/// List of all albums for this artist (including deleted). 
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IList<ArtistForAlbum> AllAlbums
		{
			get => _albums;
			set
			{
				ParamIs.NotNull(() => value);
				_albums = value;
			}
		}

		public virtual IList<ArtistForEvent> AllEvents
		{
			get => _events;
			set
			{
				ParamIs.NotNull(() => value);
				_events = value;
			}
		}

		public virtual IList<ArtistForArtist> AllGroups
		{
			get => _groups;
			set
			{
				ParamIs.NotNull(() => value);
				_groups = value;
			}
		}

		public virtual IList<ArtistForArtist> AllMembers
		{
			get => _members;
			set
			{
				ParamIs.NotNull(() => value);
				_members = value;
			}
		}

		/// <summary>
		/// List of all songs for this artist (including deleted). 
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IList<ArtistForSong> AllSongs
		{
			get => _songs;
			set
			{
				ParamIs.NotNull(() => value);
				_songs = value;
			}
		}

		public virtual bool AllowNotifications => true;

		public virtual ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields> ArchivedVersionsManager
		{
			get => _archivedVersions;
			set
			{
				ParamIs.NotNull(() => value);
				_archivedVersions = value;
			}
		}

		public virtual IEnumerable<Artist> ArtistLinksOfType(ArtistLinkType linkType, LinkDirection direction, bool allowInheritance = false)
		{
			if (!ArtistHelper.CanHaveRelatedArtists(ArtistType, linkType, direction))
				return Enumerable.Empty<Artist>();

			var result = (direction == LinkDirection.ManyToOne ? Groups : Members)
				.Where(g => g.LinkType == linkType)
				.Select(g => g.GetArtist(direction));

			// ReSharper disable PossibleMultipleEnumeration
			return allowInheritance && BaseVoicebank != null && !result.Any()
				? BaseVoicebank.ArtistLinksOfType(linkType, direction, true)
				: result;
			// ReSharper restore PossibleMultipleEnumeration
		}

		public virtual IEnumerable<Artist> ArtistList => Enumerable.Repeat(this, 1);

		public virtual ArtistType ArtistType { get; set; }

		public virtual Artist BaseVoicebank { get; set; }

		/// <summary>
		/// Whether it makes sense for this artist to have child voicebanks. 
		/// If false, the ChildVoicebanks collection should be ignored.
		/// In practice, only vocalists can have child voicebanks.
		/// This is mostly a performance optimization thing, so that the child voicebanks list doesn't need to be loaded unless needed.
		/// </summary>
		public virtual bool CanHaveChildVoicebanks => ArtistHelper.VocalistTypes.Contains(ArtistType) || ArtistType == ArtistType.Unknown;

		public virtual IList<Artist> ChildVoicebanks
		{
			get => _childVoicebanks;
			set
			{
				ParamIs.NotNull(() => value);
				_childVoicebanks = value;
			}
		}

		public virtual IList<ArtistComment> AllComments
		{
			get => _comments;
			set
			{
				ParamIs.NotNull(() => value);
				_comments = value;
			}
		}

		public virtual IEnumerable<ArtistComment> Comments => AllComments.Where(c => !c.Deleted);

		public virtual User CreatedBy
		{
			get => _createdBy;
			set
			{
				ParamIs.NotNull(() => value);
				_createdBy = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual string DefaultName => TranslatedName.Default;

		public virtual EnglishTranslatedString Description
		{
			get => _description;
			set
			{
				ParamIs.NotNull(() => value);
				_description = value;
			}
		}

		public virtual EntryType EntryType => EntryType.Artist;

		public virtual IEnumerable<ArtistForArtist> Groups => AllGroups.Where(g => !g.Parent.Deleted);

		public virtual int Id { get; set; }

		public virtual IList<ArtistHit> Hits
		{
			get => _hits;
			set
			{
				ParamIs.NotNull(() => value);
				_hits = value;
			}
		}

		/// <summary>
		/// Tests whether an artist is a valid base voicebank for this artist.
		/// 
		/// Base voicebank can be null (not set). 
		/// If set, the base voicebank must not have a base voicebank set, otherwise circular dependencies might occur.
		/// An artist cannot be the base voicebank of itself obviously.
		/// </summary>
		/// <param name="artist"></param>
		/// <returns></returns>
		public virtual bool IsValidBaseVoicebank(Artist artist)
		{
			return artist == null || (!artist.Equals(this) && !artist.HasBaseVoicebank(this));
		}

		public virtual TranslatedString TranslatedName => Names.SortNames;

		public virtual IEnumerable<ArtistForArtist> Members => AllMembers.Where(m => !m.Member.Deleted);

		public virtual NameManager<ArtistName> Names
		{
			get => _names;
			set
			{
				ParamIs.NotNull(() => value);
				_names = value;
			}
		}

		INameManager<ArtistName> IEntryWithNames<ArtistName>.Names => Names;

		INameManager IEntryWithNames.Names => Names;

		/// <summary>
		/// List of users who are verifid owners of this artist. Cannot be null.
		/// </summary>
		public virtual IList<OwnedArtistForUser> OwnerUsers
		{
			get => _ownerUsers;
			set
			{
				ParamIs.NotNull(() => value);
				_ownerUsers = value;
			}
		}

		public virtual PictureData Picture { get; set; }

		public virtual string PictureMime { get; set; }

		public virtual EntryPictureFileManager<ArtistPictureFile> Pictures
		{
			get => _pictureManager;
			set
			{
				ParamIs.NotNull(() => value);
				_pictureManager = value;
			}
		}

		public virtual Date ReleaseDate { get; set; }

		/// <summary>
		/// List of songs for this artist (not including deleted). 
		/// Warning: this list can be huge! Avoid traversing the list if possible.
		/// </summary>
		public virtual IEnumerable<ArtistForSong> Songs => AllSongs.Where(s => !s.Song.Deleted);

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<ArtistTagUsage> Tags
		{
			get => _tags;
			set
			{
				ParamIs.NotNull(() => value);
				_tags = value;
			}
		}

		ITagManager IEntryWithTags.Tags => Tags;

		public virtual EntryThumbMain Thumb => !string.IsNullOrEmpty(PictureMime) ? new EntryThumbMain(this, PictureMime) : null;

		/// <summary>
		/// List of users who follow this artist. Cannot be null.
		/// </summary>
		public virtual IList<ArtistForUser> Users
		{
			get => _users;
			set
			{
				ParamIs.NotNull(() => value);
				_users = value;
			}
		}

		public virtual int Version { get; set; }

		public virtual IList<ArtistWebLink> WebLinks
		{
			get => _webLinks;
			set
			{
				ParamIs.NotNull(() => value);
				_webLinks = value;
			}
		}

		public virtual IEnumerable<string> AllNames => Names.AllValues;

		public virtual ArtistForAlbum AddAlbum(Album album)
		{
			return AddAlbum(album, false, ArtistRoles.Default);
		}

#nullable enable
		public virtual ArtistForAlbum AddAlbum(Album album, bool support, ArtistRoles roles)
		{
			ParamIs.NotNull(() => album);

			// Check is too slow for now
			//if (HasAlbum(album))
			//	throw new InvalidOperationException($"{album} has already been added for {this}");

			var link = new ArtistForAlbum(album, this, support, roles);
			AllAlbums.Add(link);
			album.AllArtists.Add(link);

			return link;
		}

		public virtual ArtistForArtist AddGroup(Artist grp, ArtistLinkType linkType)
		{
			ParamIs.NotNull(() => grp);

			var link = new ArtistForArtist(grp, this, linkType);
			AllGroups.Add(link);
			grp.AllMembers.Add(link);

			return link;
		}

		public virtual ArtistForArtist AddMember(Artist member, ArtistLinkType linkType)
		{
			ParamIs.NotNull(() => member);

			return member.AddGroup(this, linkType);
		}
#nullable disable

		public virtual ArtistForSong AddSong(Song song)
		{
			return AddSong(song, false, ArtistRoles.Default);
		}

#nullable enable
		public virtual ArtistForSong AddSong(Song song, bool support, ArtistRoles roles)
		{
			ParamIs.NotNull(() => song);

			var link = new ArtistForSong(song, this, support, roles);
			AllSongs.Add(link);
			song.AllArtists.Add(link);

			return link;
		}
#nullable disable

		public virtual ArchivedArtistVersion CreateArchivedVersion(XDocument data, ArtistDiff diff, AgentLoginData author, ArtistArchiveReason reason, string notes)
		{
			var archived = new ArchivedArtistVersion(this, data, diff, author, Version, Status, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;
		}

#nullable enable
		public virtual Comment CreateComment(string message, AgentLoginData author)
		{
			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => author);

			var comment = new ArtistComment(this, message, author);
			AllComments.Add(comment);

			return comment;
		}

		public virtual ArtistName CreateName(string val, ContentLanguageSelection language)
		{
			ParamIs.NotNullOrEmpty(() => val);

			var name = new ArtistName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;
		}
#nullable disable

		public virtual ArtistPictureFile CreatePicture(string name, string mime, User author)
		{
			var f = new ArtistPictureFile(name, mime, author, this);
			Pictures.Add(f);

			s_log.Info("{0} created {1}", author, f);

			return f;
		}

#nullable enable
		public virtual ArtistWebLink CreateWebLink(string description, string url, WebLinkCategory category, bool disabled)
		{
			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrWhiteSpace(() => url);

			var link = new ArtistWebLink(this, description, url, category, disabled);
			WebLinks.Add(link);

			return link;
		}
#nullable disable

		public virtual void Delete()
		{
			Deleted = true;
		}

#nullable enable
		public virtual bool Equals(Artist? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as Artist);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
#nullable disable

		public virtual ArchivedArtistVersion GetLatestVersion()
		{
			return ArchivedVersionsManager.GetLatestVersion();
		}

#nullable enable
		/// <summary>
		/// Checks whether this artist has a specific album.
		/// </summary>
		/// <param name="album">Album to be checked. Cannot be null.</param>
		/// <returns>True if the artist has the album. Otherwise false.</returns>
		/// <remarks>
		/// Warning: This check can be slow if the artist has too many albums and the collection needs to be loaded.
		/// Most of the time the check should be done from the album side, since usually albums have fewer artist links than artist have linked albums.
		/// </remarks>
		public virtual bool HasAlbum(Album album)
		{
			ParamIs.NotNull(() => album);

			return Albums.Any(a => a.Album.Equals(album));
		}
#nullable disable

		/// <summary>
		/// Tests whether this artist has a specific artist as base voicebank.
		/// </summary>
		/// <param name="artist">Artist to be checked. Cannot be null.</param>
		/// <returns>True if <paramref name="artist"/> is a base voicebank of this artist. Otherwise false.</returns>
		public virtual bool HasBaseVoicebank(Artist artist)
		{
			return BaseVoicebank != null && (BaseVoicebank.Equals(artist) || BaseVoicebank.HasBaseVoicebank(artist));
		}

#nullable enable
		public virtual bool HasGroup(Artist grp)
		{
			ParamIs.NotNull(() => grp);

			return Groups.Any(a => a.Parent.Equals(grp));
		}

		public virtual bool HasMember(Artist member)
		{
			ParamIs.NotNull(() => member);

			return Members.Any(a => a.Member.Equals(member));
		}

		public virtual bool HasName(LocalizedString name)
		{
			ParamIs.NotNull(() => name);

			return Names.HasName(name);
		}

		public virtual bool HasOwnerUser(User user)
		{
			ParamIs.NotNull(() => user);

			return OwnerUsers.Any(a => a.User.Equals(user));
		}

		public virtual bool HasSong(Song song)
		{
			ParamIs.NotNull(() => song);

			return Songs.Any(a => a.Song.Equals(song));
		}

		public virtual bool HasWebLink(string url)
		{
			ParamIs.NotNull(() => url);

			return WebLinks.Any(w => w.Url == url);
		}
#nullable disable

		public virtual void SetBaseVoicebank(Artist newBaseVoicebank)
		{
			if (Equals(BaseVoicebank, newBaseVoicebank))
				return;

			BaseVoicebank?.ChildVoicebanks.Remove(this);
			newBaseVoicebank?.ChildVoicebanks.Add(this);

			BaseVoicebank = newBaseVoicebank;
		}

#nullable enable
		public override string ToString()
		{
			return $"artist '{DefaultName}' [{Id}]";
		}
#nullable disable
	}
}
