#nullable disable

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

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public class ReleaseEventSeries :
		IEntryWithNames<EventSeriesName>, IEntryWithVersions<ArchivedReleaseEventSeriesVersion, ReleaseEventSeriesEditableFields>,
		IEntryBase, IEquatable<ReleaseEventSeries>, IWebLinkFactory<ReleaseEventSeriesWebLink>, IEntryImageInformation, IEntryWithStatus,
		IEntryWithTags<EventSeriesTagUsage>, INameFactory<EventSeriesName>
	{
		public static ImageSizes ImageSizes = ImageSizes.Original | ImageSizes.SmallThumb | ImageSizes.TinyThumb;

		string IEntryBase.DefaultName => TranslatedName.Default;
		string IEntryImageInformation.Mime => PictureMime;
		ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;
		INameManager IEntryWithNames.Names => Names;
		INameManager<EventSeriesName> IEntryWithNames<EventSeriesName>.Names => Names;

		private ArchivedVersionManager<ArchivedReleaseEventSeriesVersion, ReleaseEventSeriesEditableFields> _archivedVersions = new();
		private string _description;
		private IList<ReleaseEvent> _events = new List<ReleaseEvent>();
		private NameManager<EventSeriesName> _names = new();
		private TagManager<EventSeriesTagUsage> _tags = new();
		private IList<ReleaseEventSeriesWebLink> _webLinks = new List<ReleaseEventSeriesWebLink>();

		public ReleaseEventSeries()
		{
			Category = EventCategory.Unspecified;
			Deleted = false;
			Description = string.Empty;
			Status = EntryStatus.Draft;
		}

		public ReleaseEventSeries(ContentLanguageSelection defaultLanguage, ICollection<ILocalizedString> names, string description)
			: this()
		{
			ParamIs.NotNull(() => names);

			if (!names.Any())
			{
				throw new ArgumentException("Need at least one name", nameof(names));
			}

			TranslatedName.DefaultLanguage = defaultLanguage;
			Description = description;

			foreach (var a in names)
				CreateName(a);
		}

		public virtual IList<ReleaseEvent> AllEvents
		{
			get => _events;
			set
			{
				ParamIs.NotNull(() => value);
				_events = value;
			}
		}

		public virtual bool AllowNotifications => true;

		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;

		public virtual ArchivedVersionManager<ArchivedReleaseEventSeriesVersion, ReleaseEventSeriesEditableFields> ArchivedVersionsManager
		{
			get => _archivedVersions;
			set
			{
				ParamIs.NotNull(() => value);
				_archivedVersions = value;
			}
		}

		public virtual EventCategory Category { get; set; }

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

		public virtual EntryType EntryType => EntryType.ReleaseEventSeries;

		public virtual IEnumerable<ReleaseEvent> Events => AllEvents.Where(e => !e.Deleted);

		public virtual int Id { get; set; }

		public virtual NameManager<EventSeriesName> Names
		{
			get => _names;
			set
			{
				ParamIs.NotNull(() => value);
				_names = value;
			}
		}

		public virtual string PictureMime { get; set; }

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<EventSeriesTagUsage> Tags
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

		public virtual int Version { get; set; }

		public virtual IList<ReleaseEventSeriesWebLink> WebLinks
		{
			get => _webLinks;
			set
			{
				ParamIs.NotNull(() => value);
				_webLinks = value;
			}
		}

		public virtual EventSeriesName CreateName(string val, ContentLanguageSelection language)
		{
			return CreateName(new LocalizedString(val, language));
		}

#nullable enable
		public virtual EventSeriesName CreateName(ILocalizedString localizedString)
		{
			ParamIs.NotNull(() => localizedString);

			var name = new EventSeriesName(this, localizedString);
			Names.Add(name);

			return name;
		}
#nullable disable

		public virtual ArchivedReleaseEventSeriesVersion CreateArchivedVersion(XDocument data, ReleaseEventSeriesDiff diff, AgentLoginData author, EntryEditEvent reason, string notes)
		{
			var archived = new ArchivedReleaseEventSeriesVersion(this, data, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;
		}

#nullable enable
		public virtual ReleaseEventSeriesWebLink CreateWebLink(string description, string url, WebLinkCategory category, bool disabled)
		{
			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new ReleaseEventSeriesWebLink(this, description, url, category, disabled);
			WebLinks.Add(link);

			return link;
		}

		public virtual bool Equals(ReleaseEventSeries? another)
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
			return Equals(obj as ReleaseEventSeries);
		}
#nullable disable

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual string GetEventName(int number, string suffix, string name)
		{
			if (string.IsNullOrEmpty(name))
				return string.Empty;

			if (string.IsNullOrEmpty(suffix))
			{
				return $"{name} {number}";
			}
			else
			{
				return $"{name} {number} {suffix}";
			}
		}

		public override string ToString()
		{
			return $"release event series '{TranslatedName.Default}' [{Id}]";
		}
	}
}
