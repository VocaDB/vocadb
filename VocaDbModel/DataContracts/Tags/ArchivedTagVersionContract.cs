using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class ArchivedTagVersionContract : ArchivedObjectVersionContract {

		public ArchivedTagVersionContract() { }

		public ArchivedTagVersionContract(ArchivedTagVersion archivedVersion)
			: base(archivedVersion) {

			ChangedFields = archivedVersion.Diff.ChangedFields;
			Reason = archivedVersion.CommonEditEvent;

		}

		public TagEditableFields ChangedFields { get; set; }

		public EntryEditEvent Reason { get; set; }

	}

}
