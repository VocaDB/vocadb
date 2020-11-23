using System.Xml.Linq;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Venues
{
	public class ArchivedVenueVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<VenueEditableFields>
	{
		public static ArchivedVenueVersion Create(Venue venue, VenueDiff diff, AgentLoginData author, EntryEditEvent commonEditEvent, string notes)
		{
			var contract = new ArchivedVenueContract(venue, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return venue.CreateArchivedVersion(data, diff, author, commonEditEvent, notes);
		}

		private VenueDiff diff;
		private Venue venue;

		public ArchivedVenueVersion()
		{
			Status = EntryStatus.Finished;
		}

		public ArchivedVenueVersion(Venue venue, XDocument data, VenueDiff diff, AgentLoginData author,
			EntryEditEvent commonEditEvent, string notes)
			: base(data, author, venue.Version, venue.Status, notes)
		{
			ParamIs.NotNull(() => diff);

			Entry = venue;
			Diff = diff;
			CommonEditEvent = commonEditEvent;
		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

		public override IEntryDiff DiffBase => Diff;

		public virtual VenueDiff Diff
		{
			get => diff;
			set
			{
				ParamIs.NotNull(() => value);
				diff = value;
			}
		}

		public override EntryEditEvent EditEvent => CommonEditEvent;

		public override IEntryWithNames EntryBase => Entry;

		public virtual Venue Entry
		{
			get => venue;
			set
			{
				ParamIs.NotNull(() => value);
				venue = value;
			}
		}

		public virtual ArchivedVenueVersion GetLatestVersionWithField(VenueEditableFields field)
		{
			if (IsIncluded(field))
				return this;

			return Entry.ArchivedVersionsManager.GetLatestVersionWithField(field, Version);
		}

		public virtual bool IsIncluded(VenueEditableFields field)
		{
			return Diff != null && Data != null && Diff.IsIncluded(field);
		}
	}
}
