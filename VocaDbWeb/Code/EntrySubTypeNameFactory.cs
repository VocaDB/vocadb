using System.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Code {

	public class EntrySubTypeNameFactory : IEntrySubTypeNameFactory {

		public string GetEntrySubTypeName(IEntryBase entryBase, IEnumTranslations enumTranslations, CultureInfo culture) {

			switch (entryBase) {
				case Album a:
					return enumTranslations.Translation(a.DiscType, culture);
				case Artist a:
					return enumTranslations.Translation(a.ArtistType, culture);
				case Song s:
					return enumTranslations.Translation(s.SongType, culture);
			}

			return string.Empty;

		}

	}

}