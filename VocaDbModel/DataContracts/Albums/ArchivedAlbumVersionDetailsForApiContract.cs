using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Albums;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedAlbumVersionDetailsForApiContract
{
	[DataMember]
	public AlbumForApiContract Album { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract ArchivedVersion { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ComparableVersions { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? ComparedVersion { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public ComparedVersionsForApiContract<ArchivedAlbumContract> Versions { get; init; }

	public ArchivedAlbumVersionDetailsForApiContract(
		ArchivedAlbumVersion archived,
		ArchivedAlbumVersion? comparedVersion,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
	{
		Album = new AlbumForApiContract(
			album: archived.Album,
			languagePreference: permissionContext.LanguagePreference,
			thumbPersister: null,
			fields: AlbumOptionalFields.None
		);
		ArchivedVersion = ArchivedObjectVersionForApiContract.FromAlbum(archived, userIconFactory);
		ComparedVersion = comparedVersion != null
			? ArchivedObjectVersionForApiContract.FromAlbum(comparedVersion, userIconFactory)
			: null;
		Name = Album.Name;

		ComparableVersions = archived.Album.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionForApiContract.FromAlbum(a, userIconFactory))
			.ToArray();

		Versions = ComparedVersionsForApiContract.FromAlbum(archived, comparedVersion);
	}

	public bool Hidden => ArchivedVersion.Hidden || ComparedVersion is not null && ComparedVersion.Hidden;
}
