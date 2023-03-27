using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedEventVersionDetailsForApiContract
{
	[DataMember]
	public ReleaseEventForApiContract ReleaseEvent { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract ArchivedVersion { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ComparableVersions { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? ComparedVersion { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public ComparedVersionsForApiContract<ArchivedEventContract> Versions { get; init; }

	public ArchivedEventVersionDetailsForApiContract(
		ArchivedReleaseEventVersion archived,
		ArchivedReleaseEventVersion? comparedVersion,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
	{
		ReleaseEvent = new ReleaseEventForApiContract(
			rel: archived.ReleaseEvent,
			languagePreference: permissionContext.LanguagePreference,
			permissionContext,
			fields: ReleaseEventOptionalFields.None,
			thumbPersister: null
		);
		ArchivedVersion = ArchivedObjectVersionForApiContract.FromReleaseEvent(archived, userIconFactory);
		ComparedVersion = comparedVersion != null
			? ArchivedObjectVersionForApiContract.FromReleaseEvent(comparedVersion, userIconFactory)
			: null;
		Name = ReleaseEvent.Name;

		ComparableVersions = archived.ReleaseEvent.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionForApiContract.FromReleaseEvent(a, userIconFactory))
			.ToArray();

		Versions = ComparedVersionsForApiContract.FromReleaseEvent(archived, comparedVersion);
	}

	public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);
}
