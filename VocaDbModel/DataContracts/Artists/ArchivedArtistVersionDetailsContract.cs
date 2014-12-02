using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArchivedArtistVersionDetailsContract {

		public ArchivedArtistVersionDetailsContract() { }

		public ArchivedArtistVersionDetailsContract(ArchivedArtistVersion archived, ArchivedArtistVersion comparedVersion,
			ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedArtistVersionContract(archived);
			Artist = new ArtistContract(archived.Artist, languagePreference);
			ComparedVersion = comparedVersion != null ? new ArchivedArtistVersionContract(comparedVersion) : null;
			ComparedVersionId = comparedVersion != null ? comparedVersion.Id : 0;
			Name = Artist.Name;

			ComparableVersions = archived.Artist.ArchivedVersionsManager
				.GetPreviousVersions(archived)
				.Select(a => new ArchivedObjectVersionContract(a))
				.ToArray();

			Versions = ComparedArtistsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;

		}

		public ArchivedArtistVersionContract ArchivedVersion { get; set; }

		public ArtistContract Artist { get; set; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; set; }

		public ArchivedArtistVersionContract ComparedVersion { get; set; }

		public int ComparedVersionId { get; set; }

		public string Name { get; set; }

		public ComparedArtistsContract Versions { get; set; }

	}

}
