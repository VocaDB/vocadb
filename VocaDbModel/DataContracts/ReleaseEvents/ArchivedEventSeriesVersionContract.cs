using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ArchivedEventSeriesVersionContract : ArchivedObjectVersionWithFieldsContract<ReleaseEventSeriesEditableFields, EntryEditEvent>
	{
		public ArchivedEventSeriesVersionContract() { }

		public ArchivedEventSeriesVersionContract(ArchivedReleaseEventSeriesVersion archivedVersion)
			: base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
