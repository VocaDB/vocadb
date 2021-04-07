#nullable disable

using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ServerOnlyArchivedEventSeriesVersionContract : ServerOnlyArchivedObjectVersionWithFieldsContract<ReleaseEventSeriesEditableFields, EntryEditEvent>
	{
		public ServerOnlyArchivedEventSeriesVersionContract() { }

		public ServerOnlyArchivedEventSeriesVersionContract(ArchivedReleaseEventSeriesVersion archivedVersion, IUserIconFactory userIconFactory)
			: base(archivedVersion, userIconFactory, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
