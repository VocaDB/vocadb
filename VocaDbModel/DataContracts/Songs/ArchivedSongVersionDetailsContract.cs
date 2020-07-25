using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class ArchivedSongVersionDetailsContract {

		public ArchivedSongVersionDetailsContract() { }

		public ArchivedSongVersionDetailsContract(ArchivedSongVersion archived, ArchivedSongVersion comparedVersion, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedSongVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedSongVersionContract(comparedVersion) : null;
			ComparedVersionId = comparedVersion != null ? comparedVersion.Id : 0;
			Song = new SongContract(archived.Song, languagePreference);
			Name = Song.Name;

			ComparableVersions = archived.Song.ArchivedVersionsManager
				.GetPreviousVersions(archived)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.Reason))
				.ToArray();

			Versions = ComparedSongsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;

		}

		public ArchivedSongVersionContract ArchivedVersion { get; set; }

		public bool CanBeReverted => ArchivedVersion.Version < Song.Version - 1;

		public ArchivedObjectVersionWithFieldsContract<SongEditableFields, SongArchiveReason>[] ComparableVersions { get; set; }

		public ArchivedSongVersionContract ComparedVersion { get; set; }

		public int ComparedVersionId { get; set; }

		public bool Hidden => ArchivedVersion.Hidden || (ComparedVersion != null && ComparedVersion.Hidden);

		public string Name { get; set; }

		public SongContract Song { get; set; }

		public ComparedSongsContract Versions { get; set; }


	}

}
