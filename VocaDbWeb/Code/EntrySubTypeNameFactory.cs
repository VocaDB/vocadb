using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Code {

	public class EntrySubTypeNameFactory : IEntrySubTypeNameFactory {

		public string GetEntrySubTypeName(IEntryBase entryBase, IEnumTranslations enumTranslations) {

			switch (entryBase) {
				case Album a:
					return enumTranslations.Translation(a.DiscType);
				case Artist a:
					return enumTranslations.Translation(a.ArtistType);
				case Song s:
					return enumTranslations.Translation(s.SongType);
			}

			return string.Empty;

		}

	}

}