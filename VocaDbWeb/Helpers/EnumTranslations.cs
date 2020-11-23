using System;
using System.Globalization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Helpers
{

	public class EnumTranslations : IEnumTranslations
	{

		public TranslateableEnum<AlbumReportType> AlbumReportTypeNames => Translate.AlbumReportTypeNames;

		public TranslateableEnum<ArtistReportType> ArtistReportTypeNames => Translate.ArtistReportTypeNames;

		public TranslateableEnum<SongReportType> SongReportTypeNames => Translate.SongReportTypeNames;

		public TranslateableEnum<TEnum> Translations<TEnum>() where TEnum : struct, Enum
		{
			return Translate.Translations<TEnum>();
		}

		public string Translation<TEnum>(TEnum val) where TEnum : struct, Enum
		{
			return Translations<TEnum>()[val];
		}

		public string Translation<TEnum>(TEnum val, CultureInfo culture) where TEnum : struct, Enum
		{
			return Translations<TEnum>().GetName(val, culture);
		}

	}

}