using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedTagVersionDetailsForApiContract
{
	[DataMember]
	public TagForApiContract Tag { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract ArchivedVersion { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ComparableVersions { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? ComparedVersion { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public ComparedVersionsForApiContract<ArchivedTagForApiContract> Versions { get; init; }

	public ArchivedTagVersionDetailsForApiContract(
		ArchivedTagVersion archived,
		ArchivedTagVersion? comparedVersion,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
	{
		Tag = new TagForApiContract(
			tag: archived.Tag,
			languagePreference: permissionContext.LanguagePreference,
			permissionContext,
			optionalFields: TagOptionalFields.None
		);
		ArchivedVersion = ArchivedObjectVersionForApiContract.FromTag(archived, userIconFactory);
		ComparedVersion = comparedVersion != null
			? ArchivedObjectVersionForApiContract.FromTag(comparedVersion, userIconFactory)
			: null;
		Name = Tag.Name;

		ComparableVersions = archived.Tag.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionForApiContract.FromTag(a, userIconFactory))
			.ToArray();

		Versions = ComparedVersionsForApiContract.FromTag(archived, comparedVersion, permissionContext);
	}

	public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);
}
