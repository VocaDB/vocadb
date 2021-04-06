#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Albums
{
	public class ServerOnlyArchivedAlbumVersionDetailsContract
	{
		public ServerOnlyArchivedAlbumVersionDetailsContract() { }

		public ServerOnlyArchivedAlbumVersionDetailsContract(ArchivedAlbumVersion archived, ArchivedAlbumVersion comparedVersion,
			IUserPermissionContext permissionContext)
		{
			ParamIs.NotNull(() => archived);

			Album = new AlbumContract(archived.Album, permissionContext.LanguagePreference);
			ArchivedVersion = new ServerOnlyArchivedAlbumVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ServerOnlyArchivedAlbumVersionContract(comparedVersion) : null;
			Name = Album.Name;

			ComparableVersions = archived.Album.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ServerOnlyArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.Reason))
				.ToArray();

			Versions = ComparedAlbumsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}

		public AlbumContract Album { get; init; }

		public ServerOnlyArchivedAlbumVersionContract ArchivedVersion { get; init; }

		public bool CanBeReverted => ArchivedVersion.Version < Album.Version - 1;

		public ServerOnlyArchivedObjectVersionWithFieldsContract<AlbumEditableFields, AlbumArchiveReason>[] ComparableVersions { get; init; }

		public ServerOnlyArchivedAlbumVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public ComparedAlbumsContract Versions { get; init; }
	}
}
