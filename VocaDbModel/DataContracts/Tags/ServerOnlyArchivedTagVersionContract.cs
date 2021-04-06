#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class ServerOnlyArchivedTagVersionContract : ServerOnlyArchivedObjectVersionWithFieldsContract<TagEditableFields, EntryEditEvent>
	{
		public ServerOnlyArchivedTagVersionContract() { }

		public ServerOnlyArchivedTagVersionContract(ArchivedTagVersion archivedVersion)
			: base(archivedVersion, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
