using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ArchivedEventVersionContract : ArchivedObjectVersionContract {

		public ArchivedEventVersionContract() { }

		public ArchivedEventVersionContract(ArchivedReleaseEventVersion archivedVersion)
			: base(archivedVersion) {

			ChangedFields = archivedVersion.Diff.ChangedFields;
			Reason = archivedVersion.CommonEditEvent;

		}

		public ReleaseEventEditableFields ChangedFields { get; set; }

		public EntryEditEvent Reason { get; set; }

	}

}
