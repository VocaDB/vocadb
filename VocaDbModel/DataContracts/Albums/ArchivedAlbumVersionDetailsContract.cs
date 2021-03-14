#nullable disable

using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Albums
{
	public class ArchivedAlbumVersionDetailsContract
	{
		public ArchivedAlbumVersionDetailsContract() { }

		public ArchivedAlbumVersionDetailsContract(ArchivedAlbumVersion archived, ArchivedAlbumVersion comparedVersion,
			IUserPermissionContext permissionContext)
		{
			ParamIs.NotNull(() => archived);

			Album = new AlbumContract(archived.Album, permissionContext.LanguagePreference);
			ArchivedVersion = new ArchivedAlbumVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedAlbumVersionContract(comparedVersion) : null;
			Name = Album.Name;

			ComparableVersions = archived.Album.ArchivedVersionsManager
				.GetPreviousVersions(archived, permissionContext)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.Reason))
				.ToArray();

			Versions = ComparedAlbumsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;
		}

		public AlbumContract Album { get; init; }

		public ArchivedAlbumVersionContract ArchivedVersion { get; init; }

		public bool CanBeReverted => ArchivedVersion.Version < Album.Version - 1;

		public ArchivedObjectVersionWithFieldsContract<AlbumEditableFields, AlbumArchiveReason>[] ComparableVersions { get; init; }

		public ArchivedAlbumVersionContract ComparedVersion { get; init; }

		public int ComparedVersionId { get; init; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; init; }

		public ComparedAlbumsContract Versions { get; init; }
	}
}
