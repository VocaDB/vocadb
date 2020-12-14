#nullable disable

using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class ArchivedTagVersionContract : ArchivedObjectVersionWithFieldsContract<TagEditableFields, EntryEditEvent>
	{
		public ArchivedTagVersionContract() { }

		public ArchivedTagVersionContract(ArchivedTagVersion archivedVersion)
			: base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
