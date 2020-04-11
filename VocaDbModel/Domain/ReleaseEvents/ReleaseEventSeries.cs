using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ReleaseEventSeries : 
		IEntryWithNames<EventSeriesName>, IEntryWithVersions<ArchivedReleaseEventSeriesVersion, ReleaseEventSeriesEditableFields>, 
		IEntryBase, IEquatable<ReleaseEventSeries>, IWebLinkFactory<ReleaseEventSeriesWebLink>, IEntryImageInformation, IEntryWithStatus,
		IEntryWithTags<EventSeriesTagUsage>, INameFactory<EventSeriesName> {

		public static ImageSizes ImageSizes = ImageSizes.Original | ImageSizes.SmallThumb | ImageSizes.TinyThumb;

		string IEntryBase.DefaultName => TranslatedName.Default;
		string IEntryImageInformation.Mime => PictureMime;
		ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;
		INameManager IEntryWithNames.Names => Names;
		INameManager<EventSeriesName> IEntryWithNames<EventSeriesName>.Names => Names;

		private ArchivedVersionManager<ArchivedReleaseEventSeriesVersion, ReleaseEventSeriesEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedReleaseEventSeriesVersion, ReleaseEventSeriesEditableFields>();
		private string description;
		private IList<ReleaseEvent> events = new List<ReleaseEvent>();
		private NameManager<EventSeriesName> names = new NameManager<EventSeriesName>();
		private TagManager<EventSeriesTagUsage> tags = new TagManager<EventSeriesTagUsage>();
		private IList<ReleaseEventSeriesWebLink> webLinks = new List<ReleaseEventSeriesWebLink>();

		public ReleaseEventSeries() {
			Category = EventCategory.Unspecified;
			Deleted = false;
			Description = string.Empty;
			Status = EntryStatus.Draft;
		}

		public ReleaseEventSeries(ContentLanguageSelection defaultLanguage, ICollection<ILocalizedString> names, string description)
			: this() {

			ParamIs.NotNull(() => names);

			if (!names.Any()) {
				throw new ArgumentException("Need at least one name", nameof(names));
			}

			TranslatedName.DefaultLanguage = defaultLanguage;
			Description = description;

			foreach (var a in names)
				CreateName(a);

		}

		public virtual IList<ReleaseEvent> AllEvents {
			get => events;
			set {
				ParamIs.NotNull(() => value);
				events = value;
			}
		}

		public virtual bool AllowNotifications => true;

		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;

		public virtual ArchivedVersionManager<ArchivedReleaseEventSeriesVersion, ReleaseEventSeriesEditableFields> ArchivedVersionsManager {
			get => archivedVersions;
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual EventCategory Category { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual string Description {
			get => description;
			set {
				ParamIs.NotNull(() => value);
				description = value; 
			}
		}

		public virtual EntryType EntryType => EntryType.ReleaseEventSeries;

		public virtual IEnumerable<ReleaseEvent> Events => AllEvents.Where(e => !e.Deleted);

		public virtual int Id { get; set; }

		public virtual NameManager<EventSeriesName> Names {
			get => names;
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual string PictureMime { get; set; }

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<EventSeriesTagUsage> Tags {
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

		public virtual int Version { get; set; }

		public virtual IList<ReleaseEventSeriesWebLink> WebLinks {
			get => webLinks;
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual EventSeriesName CreateName(string val, ContentLanguageSelection language) {
			return CreateName(new LocalizedString(val, language));
		}

		public virtual EventSeriesName CreateName(ILocalizedString localizedString) {

			ParamIs.NotNull(() => localizedString);

			var name = new EventSeriesName(this, localizedString);
			Names.Add(name);

			return name;

		}

		public virtual ArchivedReleaseEventSeriesVersion CreateArchivedVersion(XDocument data, ReleaseEventSeriesDiff diff, AgentLoginData author, EntryEditEvent reason, string notes) {

			var archived = new ArchivedReleaseEventSeriesVersion(this, data, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual ReleaseEventSeriesWebLink CreateWebLink(string description, string url, WebLinkCategory category) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new ReleaseEventSeriesWebLink(this, description, url, category);
			WebLinks.Add(link);

			return link;

		}

		public virtual bool Equals(ReleaseEventSeries another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEventSeries);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual string GetEventName(int number, string suffix, string name) {

			if (string.IsNullOrEmpty(name))
				return string.Empty;

			if (string.IsNullOrEmpty(suffix)) {
				return string.Format("{0} {1}", name, number);
			} else {
				return string.Format("{0} {1} {2}", name, number, suffix);
			}

		}

		public override string ToString() {
			return string.Format("release event series '{0}' [{1}]", TranslatedName.Default, Id);
		}

	}

}
