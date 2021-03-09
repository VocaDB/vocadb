#nullable disable

using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ArchivedEventVersionDetailsContract
	{
		public ArchivedEventVersionDetailsContract() { }

		public ArchivedEventVersionDetailsContract(ArchivedReleaseEventVersion archived, ArchivedReleaseEventVersion comparedVersion, IUserPermissionContext permissionContext)
		{
			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedEventVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedEventVersionContract(comparedVersion) : null;
			ReleaseEvent = new ReleaseEventContract(archived.ReleaseEvent, permissionContext.LanguagePreference);
			Name = ReleaseEvent.Name;

			ComparableVersions = archived.ReleaseEvent.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedEventsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}

		public ArchivedObjectVersionContract ArchivedVersion { get; init; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; init; }

		public ArchivedObjectVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public ReleaseEventContract ReleaseEvent { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public ComparedEventsContract Versions { get; init; }
	}
}
