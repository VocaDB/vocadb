using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.Translations {

	public interface IEnumTranslations {

		TranslateableEnum<AlbumReportType> AlbumReportTypeNames { get; }

		TranslateableEnum<ArtistReportType> ArtistReportTypeNames { get; }

		TranslateableEnum<SongReportType> SongReportTypeNames { get; }

	}

}
