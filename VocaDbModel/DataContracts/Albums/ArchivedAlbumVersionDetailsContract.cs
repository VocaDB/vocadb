using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class ArchivedAlbumVersionDetailsContract {

		public ArchivedAlbumVersionDetailsContract() { }

		public ArchivedAlbumVersionDetailsContract(ArchivedAlbumVersion archived, ArchivedAlbumVersion comparedVersion, 
			ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archived);

			Album = new AlbumContract(archived.Album, languagePreference);
			ArchivedVersion = new ArchivedAlbumVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedAlbumVersionContract(comparedVersion) : null; 
			Name = Album.Name;

			ComparableVersions = archived.Album.ArchivedVersionsManager
				.GetPreviousVersions(archived)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.Reason))
				.ToArray();

			Versions = ComparedAlbumsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;

		}

		public AlbumContract Album { get; set; }

		public ArchivedAlbumVersionContract ArchivedVersion { get; set; }

		public bool CanBeReverted => ArchivedVersion.Version < Album.Version - 1;

		public ArchivedObjectVersionWithFieldsContract<AlbumEditableFields, AlbumArchiveReason>[] ComparableVersions { get; set; }

		public ArchivedAlbumVersionContract ComparedVersion { get; set; }

		public int ComparedVersionId { get; set; }

		public string Name { get; set; }

		public ComparedAlbumsContract Versions { get; set; }

	}

}
