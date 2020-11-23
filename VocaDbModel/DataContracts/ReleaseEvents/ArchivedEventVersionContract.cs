using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{

	public class ArchivedEventVersionContract : ArchivedObjectVersionWithFieldsContract<ReleaseEventEditableFields, EntryEditEvent>
	{

		public ArchivedEventVersionContract() { }

		public ArchivedEventVersionContract(ArchivedReleaseEventVersion archivedVersion)
			: base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }

	}

}
