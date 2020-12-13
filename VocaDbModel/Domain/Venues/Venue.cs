using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Venues
{
	public class Venue : IEntryWithNames<VenueName>, IEntryWithVersions<ArchivedVenueVersion, VenueEditableFields>,
		IEntryBase, IWebLinkFactory<VenueWebLink>, IEntryWithStatus, INameFactory<VenueName>
	{
		IArchivedVersionsManager IEntryWithVersions.ArchivedVersionsManager => ArchivedVersionsManager;
		INameManager IEntryWithNames.Names => Names;
		INameManager<VenueName> IEntryWithNames<VenueName>.Names => Names;

		private ArchivedVersionManager<ArchivedVenueVersion, VenueEditableFields> archivedVersions = new ArchivedVersionManager<ArchivedVenueVersion, VenueEditableFields>();
		private OptionalGeoPoint coordinates;
		private IList<ReleaseEvent> events = new List<ReleaseEvent>();
		private NameManager<VenueName> names = new NameManager<VenueName>();
		private WebLinkManager<VenueWebLink> webLinks = new WebLinkManager<VenueWebLink>();

		public Venue() { }

		public Venue(ContentLanguageSelection defaultLanguage, ICollection<ILocalizedString> names, string description) : this()
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

		public virtual string Address { get; set; } = string.Empty;

		public virtual string AddressCountryCode { get; set; } = string.Empty;

		public virtual ArchivedVersionManager<ArchivedVenueVersion, VenueEditableFields> ArchivedVersionsManager
		{
			get => archivedVersions;
			set
			{
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual OptionalGeoPoint Coordinates
		{
			get => coordinates ?? (coordinates = new OptionalGeoPoint());
			set => coordinates = value;
		}

		public virtual DateTime CreateDate { get; set; } = DateTime.Now;

		public virtual string DefaultName => TranslatedName.Default;

		public virtual bool Deleted { get; set; }

		public virtual string Description { get; set; } = string.Empty;

		public virtual EntryType EntryType => EntryType.Venue;

		public virtual IList<ReleaseEvent> AllEvents
		{
			get => events;
			set
			{
				ParamIs.NotNull(() => value);
				events = value;
			}
		}

		public virtual IEnumerable<ReleaseEvent> Events => AllEvents.Where(e => !e.Deleted);

		public virtual int Id { get; set; }

		public virtual NameManager<VenueName> Names
		{
			get => names;
			set
			{
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual EntryStatus Status { get; set; } = EntryStatus.Draft;

		public virtual TranslatedString TranslatedName => Names.SortNames;

		public virtual int Version { get; set; }

		public virtual WebLinkManager<VenueWebLink> WebLinks
		{
			get => webLinks;
			set
			{
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual ArchivedVenueVersion CreateArchivedVersion(XDocument data, VenueDiff diff, AgentLoginData author, EntryEditEvent reason, string notes)
		{
			var archived = new ArchivedVenueVersion(this, data, diff, author, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;
		}

		public virtual VenueName CreateName(string val, ContentLanguageSelection language)
		{
			return CreateName(new LocalizedString(val, language));
		}

		public virtual VenueName CreateName(ILocalizedString localizedString)
		{
			ParamIs.NotNull(() => localizedString);

			var name = new VenueName(this, localizedString);
			Names.Add(name);

			return name;
		}

		public virtual VenueWebLink CreateWebLink(string description, string url, WebLinkCategory category, bool disabled)
		{
			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new VenueWebLink(this, description, url, category, disabled);
			WebLinks.Links.Add(link);

			return link;
		}

		public override string ToString() => $"venue '{DefaultName}' [{Id}]";
	}
}
