using System;
using System.Collections.Generic;
using System.Linq;
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

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ReleaseEvent : IEntryWithNames<EventName>, IEntryWithVersions, IWebLinkFactory<ReleaseEventWebLink>, IReleaseEvent, 
		IEntryImageInformation, IEntryWithComments<ReleaseEventComment>, IEntryWithStatus, INameFactory<EventName>, IEntryWithTags<EventTagUsage>, IEntryWithArtistLinks<ArtistForEvent> {

		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;
		string IReleaseEvent.Name => DefaultName;
		INameManager IEntryWithNames.Names => Names;
		INameManager<EventName> IEntryWithNames<EventName>.Names => Names;
		string IEntryImageInformation.Mime => PictureMime;

		private IList<Album> albums = new List<Album>();
		private ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields>();
		private IList<ArtistForEvent> artists = new List<ArtistForEvent>();
		private IList<ReleaseEventComment> comments = new List<ReleaseEventComment>();
		private string description;
		private NameManager<EventName> names = new NameManager<EventName>();
		private PVManager<PVForEvent> pvs = new PVManager<PVForEvent>();
		private ReleaseEventSeries series;
		private string seriesSuffix;
		private IList<Song> songs = new List<Song>();
		private TagManager<EventTagUsage> tags = new TagManager<EventTagUsage>();
		private IList<EventForUser> users = new List<EventForUser>();
		private IList<ReleaseEventWebLink> webLinks = new List<ReleaseEventWebLink>();

		public ReleaseEvent() {
			Category = EventCategory.Unspecified;
			CreateDate = DateTime.Now;
			Deleted = false;
			Description = SeriesSuffix = string.Empty;
			Status = EntryStatus.Draft;
		}

		public ReleaseEvent(string description, DateTime? date, ContentLanguageSelection defaultNameLanguage)
			: this() {

			ParamIs.NotNull(() => names);

			Description = description;
			Date = date;
			TranslatedName.DefaultLanguage = defaultNameLanguage;
			TranslatedName.Clear();

		}

		public ReleaseEvent(string description, DateTime? date, ReleaseEventSeries series, int seriesNumber, string seriesSuffix,
			ContentLanguageSelection defaultNameLanguage, bool customName)
			: this() {

			ParamIs.NotNull(() => series);

			Description = description;
			Date = date;
			Series = series;
			SeriesNumber = seriesNumber;
			SeriesSuffix = seriesSuffix;
			CustomName = customName;
			TranslatedName.Clear();

			if (customName) {
				TranslatedName.DefaultLanguage = defaultNameLanguage;
			} else {
				TranslatedName.DefaultLanguage = Series.TranslatedName.DefaultLanguage;
			}

		}

		public virtual IEnumerable<Album> Albums => AllAlbums.Where(a => !a.Deleted);

		public virtual IList<Album> AllAlbums {
			get => albums;
			set {
				ParamIs.NotNull(() => value);
				albums = value; 
			}
		}

		public virtual IList<ArtistForEvent> AllArtists {
			get => artists;
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		public virtual IList<Song> AllSongs {
			get => songs;
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual bool AllowNotifications => true;

		public virtual ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> ArchivedVersionsManager {
			get => archivedVersions;
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual IEnumerable<ArtistForEvent> Artists => AllArtists.Where(a => a.Artist == null || !a.Artist.Deleted);

		public virtual EventCategory Category { get; set; }

		IEnumerable<Comment> IEntryWithComments.Comments => Comments;

		public virtual IList<ReleaseEventComment> Comments {
			get => comments;
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual bool CustomName { get; set; }

		public virtual Date Date { get; set; }

		public virtual string DefaultName => TranslatedName.Default;

		public virtual bool Deleted { get; set; }

		public virtual string Description {
			get => description;
			set {
				ParamIs.NotNull(() => value);
				description = value; 
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

		public virtual NameManager<EventName> Names {
			get => names;
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual string PictureMime { get; set; }

		public virtual PVManager<PVForEvent> PVs {
			get => pvs;
			set {
				ParamIs.NotNull(() => value);
				pvs = value;
			}
		}

		public virtual ReleaseEventSeries Series {
			get => series;
			set => series = value;
		}

		public virtual int SeriesNumber { get; set; }

		public virtual string SeriesSuffix {
			get => seriesSuffix;
			set {
				ParamIs.NotNull(() => value);
				seriesSuffix = value;
			}
		}

		public virtual SongList SongList { get; set; }

		public virtual IEnumerable<Song> Songs => AllSongs.Where(a => !a.Deleted);

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<EventTagUsage> Tags {
			get => tags;
			set {
				ParamIs.NotNull(() => value);
				tags = value;
			}
		}

		ITagManager IEntryWithTags.Tags => Tags;

		public virtual TranslatedString TranslatedName => Names.SortNames;

		/// <summary>
		/// URL slug. Cannot be null. Can be empty.
		/// </summary>
		public virtual string UrlSlug => Utils.UrlFriendlyNameFactory.GetUrlFriendlyName(TranslatedName);

		public virtual IList<EventForUser> Users {
			get => users;
			set {
				ParamIs.NotNull(() => value);
				users = value;
			}
		}

		public virtual Venue Venue { get; set; }

		public virtual string VenueName { get; set; }

		public virtual int Version { get; set; }

		public virtual IList<ReleaseEventWebLink> WebLinks {
			get => webLinks;
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual ArchivedReleaseEventVersion CreateArchivedVersion(XDocument data, ReleaseEventDiff diff, AgentLoginData author, EntryEditEvent reason, string notes) {

			var archived = new ArchivedReleaseEventVersion(this, data, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual Comment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new ReleaseEventComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual EventName CreateName(string val, ContentLanguageSelection language) {
			return CreateName(new LocalizedString(val, language));
		}

		public virtual EventName CreateName(ILocalizedString localizedString) {

			ParamIs.NotNull(() => localizedString);

			var name = new EventName(this, localizedString);
			Names.Add(name);

			return name;

		}

		public virtual PVForEvent CreatePV(PVContract contract) {

			ParamIs.NotNull(() => contract);

			var pv = new PVForEvent(this, contract);
			PVs.Add(pv);

			return pv;

		}

		public virtual ReleaseEventWebLink CreateWebLink(string description, string url, WebLinkCategory category) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new ReleaseEventWebLink(this, description, url, category);
			WebLinks.Add(link);

			return link;

		}

		public virtual bool Equals(ReleaseEvent another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEvent);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		private ArtistForEvent AddArtist(ArtistForEventContract contract, Func<int, Artist> artistGetter) {

			ArtistForEvent link;

			if (contract.Artist == null) {
				link = new ArtistForEvent(this, null) {
					Name = contract.Name,
					Roles = contract.Roles
				};
			} else {
				var artist = artistGetter(contract.Artist.Id);
				link = new ArtistForEvent(this, artist) {
					Roles = contract.Roles
				};
			}

			AllArtists.Add(link);
			return link;

		}

		public virtual IEnumerable<LocalizedString> GetNamesFromSeries() {
			return Series.Names.Select(seriesName => new LocalizedString(Series.GetEventName(SeriesNumber, SeriesSuffix, seriesName.Value), seriesName.Language));
		}

		public virtual void SetSeries(ReleaseEventSeries newSeries) {
			
			if (Equals(Series, newSeries))
				return;

			Series?.AllEvents.Remove(this);
			newSeries?.AllEvents.Add(this);
			Series = newSeries;

		}

		public virtual CollectionDiffWithValue<ArtistForEvent, ArtistForEvent> SyncArtists(
			IList<ArtistForEventContract> newArtists, Func<int, Artist> artistGetter) {

			ParamIs.NotNull(() => newArtists);

			bool Update(ArtistForEvent old, ArtistForEventContract newArtist) {
				if (old.Roles == newArtist.Roles) {
					return false;
				}
				old.Roles = newArtist.Roles;
				return true;
			}

			var diff = CollectionHelper.SyncWithContent(AllArtists, newArtists, (a1, a2) => a1.Id == a2.Id, a => AddArtist(a, artistGetter), Update, null);
			return diff;

		}

		public override string ToString() {
			return string.Format("Release event '{0}' [{1}]", DefaultName, Id);
		}

	}

	public interface IReleaseEvent : IEntryWithIntId {
		string Name { get; }
	}

}
