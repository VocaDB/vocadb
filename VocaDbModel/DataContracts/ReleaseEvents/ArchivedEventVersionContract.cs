#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ArchivedEventVersionContract : ArchivedObjectVersionWithFieldsContract<ReleaseEventEditableFields, EntryEditEvent>
	{
		public ArchivedEventVersionContract() { }

		public ArchivedEventVersionContract(ArchivedReleaseEventVersion archivedVersion, IUserIconFactory userIconFactory)
			: base(archivedVersion, userIconFactory, archivedVersion.Diff.ChangedFields.Value, archivedVersion.CommonEditEvent) { }
	}
}
