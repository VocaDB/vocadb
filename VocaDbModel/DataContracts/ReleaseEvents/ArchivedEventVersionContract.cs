using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ArchivedEventVersionContract : ArchivedObjectVersionWithFieldsContract<ReleaseEventEditableFields, EntryEditEvent> {

		public ArchivedEventVersionContract() { }

		public ArchivedEventVersionContract(ArchivedReleaseEventVersion archivedVersion)
			: base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) {

			ChangedFields = archivedVersion.Diff.ChangedFields.Value;
			Reason = archivedVersion.CommonEditEvent;

		}

	}

}
