#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ServerOnlyArchivedEventVersionDetailsContract
	{
		public ServerOnlyArchivedEventVersionDetailsContract() { }

#nullable enable
		public ServerOnlyArchivedEventVersionDetailsContract(ArchivedReleaseEventVersion archived, ArchivedReleaseEventVersion? comparedVersion, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ServerOnlyArchivedEventVersionContract(archived, userIconFactory);
			ComparedVersion = comparedVersion != null ? new ServerOnlyArchivedEventVersionContract(comparedVersion, userIconFactory) : null;
			ReleaseEvent = new ReleaseEventContract(archived.ReleaseEvent, permissionContext.LanguagePreference);
			Name = ReleaseEvent.Name;

			ComparableVersions = archived.ReleaseEvent.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ServerOnlyArchivedObjectVersionWithFieldsContract.Create(a, userIconFactory, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedEventsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}
#nullable disable

		public ServerOnlyArchivedObjectVersionContract ArchivedVersion { get; init; }

		public ServerOnlyArchivedObjectVersionContract[] ComparableVersions { get; init; }

		public ServerOnlyArchivedObjectVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public ReleaseEventContract ReleaseEvent { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public ComparedEventsContract Versions { get; init; }
	}
}
