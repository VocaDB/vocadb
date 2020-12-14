#nullable disable

using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	public class ArchivedVenueVersionContract : ArchivedObjectVersionWithFieldsContract<VenueEditableFields, EntryEditEvent>
	{
		public ArchivedVenueVersionContract() { }

		public ArchivedVenueVersionContract(ArchivedVenueVersion archivedVersion) : base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
