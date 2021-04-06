#nullable disable

using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ServerOnlyArchivedEventSeriesVersionContract : ServerOnlyArchivedObjectVersionWithFieldsContract<ReleaseEventSeriesEditableFields, EntryEditEvent>
	{
		public ServerOnlyArchivedEventSeriesVersionContract() { }

		public ServerOnlyArchivedEventSeriesVersionContract(ArchivedReleaseEventSeriesVersion archivedVersion)
			: base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
