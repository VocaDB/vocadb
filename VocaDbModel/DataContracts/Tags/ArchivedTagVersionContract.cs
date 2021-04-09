#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class ArchivedTagVersionContract : ArchivedObjectVersionWithFieldsContract<TagEditableFields, EntryEditEvent>
	{
		public ArchivedTagVersionContract() { }

		public ArchivedTagVersionContract(ArchivedTagVersion archivedVersion, IUserIconFactory userIconFactory)
			: base(archivedVersion, userIconFactory, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
