using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedSongVersionDetailsForApiContract
{
	[DataMember]
	public SongForApiContract Song{ get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract ArchivedVersion { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ComparableVersions { get; init; }

	[DataMember]
	public ArchivedObjectVersionForApiContract? ComparedVersion { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public ComparedVersionsForApiContract<ArchivedSongForApiContract> Versions { get; init; }

	public ArchivedSongVersionDetailsForApiContract(
		ArchivedSongVersion archived,
		ArchivedSongVersion? comparedVersion,
		IUserPermissionContext permissionContext,
		IUserIconFactory userIconFactory
	)
	{
		Song = new SongForApiContract(
			song: archived.Song,
			languagePreference: permissionContext.LanguagePreference,
			permissionContext,
			fields: SongOptionalFields.None
		);
		ArchivedVersion = ArchivedObjectVersionForApiContract.FromSong(archived, userIconFactory);
		ComparedVersion = comparedVersion != null
			? ArchivedObjectVersionForApiContract.FromSong(comparedVersion, userIconFactory)
			: null;
		Name = Song.Name;

		ComparableVersions = archived.Song.ArchivedVersionsManager
			.GetPreviousVersions(archived, permissionContext)
			.Select(a => ArchivedObjectVersionForApiContract.FromSong(a, userIconFactory))
			.ToArray();

		Versions = ComparedVersionsForApiContract.FromSong(archived, comparedVersion, permissionContext);
	}

	public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);
}
