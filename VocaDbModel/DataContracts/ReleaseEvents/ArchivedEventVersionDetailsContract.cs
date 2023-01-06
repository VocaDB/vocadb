#nullable disable

using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[Obsolete]
public class ArchivedEventVersionDetailsContract
{
	public ArchivedEventVersionDetailsContract() { }

#nullable enable
	public ArchivedEventVersionDetailsContract(ArchivedReleaseEventVersion archived, ArchivedReleaseEventVersion? comparedVersion, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory)
	{
		ParamIs.NotNull(() => archived);

		ArchivedVersion = new ArchivedEventVersionContract(archived, userIconFactory);
		ComparedVersion = comparedVersion != null ? new ArchivedEventVersionContract(comparedVersion, userIconFactory) : null;
		ReleaseEvent = new ReleaseEventContract(archived.ReleaseEvent, permissionContext.LanguagePreference);
		Name = ReleaseEvent.Name;

		ComparableVersions = archived.ReleaseEvent.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, userIconFactory, a.Diff.ChangedFields.Value, a.CommonEditEvent))
			.ToArray();

		Versions = ComparedEventsContract.Create(archived, comparedVersion);

		ComparedVersionId = Versions.SecondId;
	}
#nullable disable

	public ArchivedObjectVersionContract ArchivedVersion { get; init; }

	public ArchivedObjectVersionContract[] ComparableVersions { get; init; }

	public ArchivedObjectVersionContract ComparedVersion { get; init; }

	public int ComparedVersionId { get; init; }

	public ReleaseEventContract ReleaseEvent { get; init; }

	public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

	public string Name { get; init; }

	public ComparedEventsContract Versions { get; init; }
}
