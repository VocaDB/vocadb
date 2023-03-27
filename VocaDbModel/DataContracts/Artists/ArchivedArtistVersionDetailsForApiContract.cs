using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Artists;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedArtistVersionDetailsForApiContract
{
	[DataMember]
	public ArtistForApiContract Artist { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract ArchivedVersion { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ComparableVersions { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? ComparedVersion { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public ComparedVersionsForApiContract<ArchivedArtistContract> Versions { get; init; }

	public ArchivedArtistVersionDetailsForApiContract(
		ArchivedArtistVersion archived,
		ArchivedArtistVersion? comparedVersion,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
	{
		Artist = new ArtistForApiContract(
			artist: archived.Artist,
			languagePreference: permissionContext.LanguagePreference,
			permissionContext,
			thumbPersister: null,
			includedFields: ArtistOptionalFields.None
		);
		ArchivedVersion = ArchivedObjectVersionForApiContract.FromArtist(archived, userIconFactory);
		ComparedVersion = comparedVersion != null
			? ArchivedObjectVersionForApiContract.FromArtist(comparedVersion, userIconFactory)
			: null;
		Name = Artist.Name;

		ComparableVersions = archived.Artist.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionForApiContract.FromArtist(a, userIconFactory))
			.ToArray();

		Versions = ComparedVersionsForApiContract.FromArtist(archived, comparedVersion);
	}

	public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);
}
