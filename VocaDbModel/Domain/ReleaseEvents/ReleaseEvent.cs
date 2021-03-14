#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public class ReleaseEvent : IEntryWithNames<EventName>, IEntryWithVersions, IWebLinkFactory<ReleaseEventWebLink>, IReleaseEvent,
		IEntryImageInformation, IEntryWithComments<ReleaseEventComment>, IEntryWithStatus, INameFactory<EventName>, IEntryWithTags<EventTagUsage>, IEntryWithArtistLinks<ArtistForEvent>
	{
		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;
		string IReleaseEvent.Name => DefaultName;
		INameManager IEntryWithNames.Names => Names;
		INameManager<EventName> IEntryWithNames<EventName>.Names => Names;
		string IEntryImageInformation.Mime => PictureMime;
		ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;

		private IList<Album> _albums = new List<Album>();
		private ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> _archivedVersions = new();
		private IList<ArtistForEvent> _artists = new List<ArtistForEvent>();
		private IList<ReleaseEventComment> _comments = new List<ReleaseEventComment>();
		private string _description;
		private NameManager<EventName> _names = new();
		private PVManager<PVForEvent> _pvs = new();
		private ReleaseEventSeries _series;
		private string _seriesSuffix;
		private IList<Song> _songs = new List<Song>();
		private TagManager<EventTagUsage> _tags = new();
		private IList<EventForUser> _users = new List<EventForUser>();
		private IList<ReleaseEventWebLink> _webLinks = new List<ReleaseEventWebLink>();

		public ReleaseEvent()
		{
			Category = EventCategory.Unspecified;
			CreateDate = DateTime.Now;
			Deleted = false;
			Description = SeriesSuffix = string.Empty;
			Status = EntryStatus.Draft;
		}

		public ReleaseEvent(string description, DateTime? date, ContentLanguageSelection defaultNameLanguage)
			: this()
		{
			ParamIs.NotNull(() => _names);

			Description = description;
			Date = date;
			TranslatedName.DefaultLanguage = defaultNameLanguage;
			TranslatedName.Clear();
		}

		public ReleaseEvent(string description, DateTime? date, ReleaseEventSeries series, int seriesNumber, string seriesSuffix,
			ContentLanguageSelection defaultNameLanguage, bool customName)
			: this()
		{
			ParamIs.NotNull(() => series);

			Description = description;
			Date = date;
			Series = series;
			SeriesNumber = seriesNumber;
			SeriesSuffix = seriesSuffix;
			CustomName = customName;
			TranslatedName.Clear();

			if (customName)
			{
				TranslatedName.DefaultLanguage = defaultNameLanguage;
			}
			else
			{
				TranslatedName.DefaultLanguage = Series.TranslatedName.DefaultLanguage;
			}
		}

		public virtual IEnumerable<Album> Albums => AllAlbums.Where(a => !a.Deleted);

		public virtual IList<Album> AllAlbums
		{
			get => _albums;
			set
			{
				ParamIs.NotNull(() => value);
				_albums = value;
			}
		}

		public virtual IList<ArtistForEvent> AllArtists
		{
			get => _artists;
			set
			{
				ParamIs.NotNull(() => value);
				_artists = value;
			}
		}

		public virtual IList<Song> AllSongs
		{
			get => _songs;
			set
			{
				ParamIs.NotNull(() => value);
				_songs = value;
			}
		}

		public virtual bool AllowNotifications => true;

		public virtual ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> ArchivedVersionsManager
		{
			get => _archivedVersions;
			set
			{
				ParamIs.NotNull(() => value);
				_archivedVersions = value;
			}
		}

		public virtual IEnumerable<ArtistForEvent> Artists => AllArtists.Where(a => a.Artist == null || !a.Artist.Deleted);

		public virtual EventCategory Category { get; set; }

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		public virtual IList<ReleaseEventComment> AllComments
		{
			get => _comments;
			set
			{
				ParamIs.NotNull(() => value);
				_comments = value;
			}
		}

		public virtual IEnumerable<ReleaseEventComment> Comments => AllComments.Where(c => !c.Deleted);

		public virtual DateTime CreateDate { get; set; }

		public virtual bool CustomName { get; set; }

		public virtual Date Date { get; set; }

		public virtual string DefaultName => TranslatedName.Default;

		public virtual bool Deleted { get; set; }

		public virtual string Description
		{
			get => _description;
			set
			{
				ParamIs.NotNull(() => value);
				_description = value;
			}
		}

		public virtual Date EndDate { get; set; }

		public virtual EntryType EntryType => EntryType.ReleaseEvent;

		public virtual bool HasSeries => Series != null;

		public virtual int Id { get; set; }

		/// <summary>
		/// Event category inherited from series if there is one, otherwise event's own category.
		/// </summary>
		public virtual EventCategory InheritedCategory => HasSeries ? Series.Category : Category;

		public virtual NameManager<EventName> Names
		{
			get => _names;
			set
			{
				ParamIs.NotNull(() => value);
				_names = value;
			}
		}

		public virtual string PictureMime { get; set; }

		public virtual PVManager<PVForEvent> PVs
		{
			get => _pvs;
			set
			{
				ParamIs.NotNull(() => value);
				_pvs = value;
			}
		}

		public virtual ReleaseEventSeries Series
		{
			get => _series;
			set => _series = value;
		}

		public virtual int SeriesNumber { get; set; }

		public virtual string SeriesSuffix
		{
			get => _seriesSuffix;
			set
			{
				ParamIs.NotNull(() => value);
				_seriesSuffix = value;
			}
		}

		public virtual SongList SongList { get; set; }

		public virtual IEnumerable<Song> Songs => AllSongs.Where(a => !a.Deleted);

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<EventTagUsage> Tags
		{
			get => _tags;
			set
			{
				ParamIs.NotNull(() => value);
				_tags = value;
			}
		}

		ITagManager IEntryWithTags.Tags => Tags;

		public virtual TranslatedString TranslatedName => Names.SortNames;

		/// <summary>
		/// URL slug. Cannot be null. Can be empty.
		/// </summary>
		public virtual string UrlSlug => Utils.UrlFriendlyNameFactory.GetUrlFriendlyName(TranslatedName);

		public virtual IList<EventForUser> Users
		{
			get => _users;
			set
			{
				ParamIs.NotNull(() => value);
				_users = value;
			}
		}

		public virtual Venue Venue { get; set; }

		public virtual string VenueName { get; set; }

		public virtual int Version { get; set; }

		public virtual IList<ReleaseEventWebLink> WebLinks
		{
			get => _webLinks;
			set
			{
				ParamIs.NotNull(() => value);
				_webLinks = value;
			}
		}

		public virtual ArchivedReleaseEventVersion CreateArchivedVersion(XDocument data, ReleaseEventDiff diff, AgentLoginData author, EntryEditEvent reason, string notes)
		{
			var archived = new ArchivedReleaseEventVersion(this, data, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;
		}

#nullable enable
		public virtual Comment CreateComment(string message, AgentLoginData loginData)
		{
			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new ReleaseEventComment(this, message, loginData);
			AllComments.Add(comment);

			return comment;
		}
#nullable disable

		public virtual EventName CreateName(string val, ContentLanguageSelection language)
		{
			return CreateName(new LocalizedString(val, language));
		}

#nullable enable
		public virtual EventName CreateName(ILocalizedString localizedString)
		{
			ParamIs.NotNull(() => localizedString);

			var name = new EventName(this, localizedString);
			Names.Add(name);

			return name;
		}

		public virtual PVForEvent CreatePV(PVContract contract)
		{
			ParamIs.NotNull(() => contract);

			var pv = new PVForEvent(this, contract);
			PVs.Add(pv);

			return pv;
		}

		public virtual ReleaseEventWebLink CreateWebLink(string description, string url, WebLinkCategory category, bool disabled)
		{
			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new ReleaseEventWebLink(this, description, url, category, disabled);
			WebLinks.Add(link);

			return link;
		}
#nullable disable

		public virtual bool Equals(ReleaseEvent another)
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
			return Equals(obj as ReleaseEvent);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

#nullable enable
		private async Task<ArtistForEvent> AddArtist(ArtistForEventContract contract, Func<int, Task<Artist>> artistGetter)
		{
			ArtistForEvent link;

			if (contract.Artist == null)
			{
				link = new ArtistForEvent(this, null)
				{
					Name = contract.Name,
					Roles = contract.Roles
				};
			}
			else
			{
				var artist = await artistGetter(contract.Artist.Id);
				link = new ArtistForEvent(this, artist)
				{
					Roles = contract.Roles
				};
			}

			AllArtists.Add(link);
			return link;
		}
#nullable disable

		public virtual IEnumerable<LocalizedString> GetNamesFromSeries()
		{
			return Series.Names.Select(seriesName => new LocalizedString(Series.GetEventName(SeriesNumber, SeriesSuffix, seriesName.Value), seriesName.Language));
		}

		public virtual void SetSeries(ReleaseEventSeries newSeries)
		{
			if (Equals(Series, newSeries))
				return;

			Series?.AllEvents.Remove(this);
			newSeries?.AllEvents.Add(this);
			Series = newSeries;
		}

		public virtual void SetVenue(Venue newVenue)
		{
			if (Equals(Venue, newVenue))
				return;

			Venue?.AllEvents.Remove(this);
			newVenue?.AllEvents.Add(this);
			Venue = newVenue;
		}

#nullable enable
		public virtual async Task<CollectionDiffWithValue<ArtistForEvent, ArtistForEvent>> SyncArtists(
			IList<ArtistForEventContract> newArtists, Func<int, Task<Artist>> artistGetter)
		{
			ParamIs.NotNull(() => newArtists);

			Task<bool> Update(ArtistForEvent old, ArtistForEventContract newArtist)
			{
				if (old.Roles == newArtist.Roles)
				{
					return Task.FromResult(false);
				}
				old.Roles = newArtist.Roles;
				return Task.FromResult(true);
			}

			var diff = await CollectionHelper.SyncWithContentAsync(AllArtists, newArtists, (a1, a2) => a1.Id == a2.Id, async a => await AddArtist(a, artistGetter), Update, null);
			return diff;
		}
#nullable disable

		public override string ToString()
		{
			return $"Release event '{DefaultName}' [{Id}]";
		}
	}

	public interface IReleaseEvent : IEntryWithIntId
	{
		string Name { get; }
	}
}
