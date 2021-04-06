#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ServerOnlyArchivedEventVersionContract : ServerOnlyArchivedObjectVersionWithFieldsContract<ReleaseEventEditableFields, EntryEditEvent>
	{
		public ServerOnlyArchivedEventVersionContract() { }

		public ServerOnlyArchivedEventVersionContract(ArchivedReleaseEventVersion archivedVersion)
			: base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
