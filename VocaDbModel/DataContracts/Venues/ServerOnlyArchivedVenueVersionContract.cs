#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	public class ServerOnlyArchivedVenueVersionContract : ServerOnlyArchivedObjectVersionWithFieldsContract<VenueEditableFields, EntryEditEvent>
	{
		public ServerOnlyArchivedVenueVersionContract() { }

		public ServerOnlyArchivedVenueVersionContract(ArchivedVenueVersion archivedVersion) : base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
