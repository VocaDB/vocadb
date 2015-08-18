using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.TagFormatting {

	public class SongListFormatter : SongCsvFileFormatter<SongInList> {

		public SongListFormatter(IEntryLinkFactory entryLinkFactory)
			: base(entryLinkFactory) {
		}

		protected override string GetFieldValue(string fieldName, SongInList songInList, ContentLanguagePreference languagePreference) {

			switch (fieldName) {
				case "notes":
					return songInList.Notes;
				default:
					return GetFieldValue(fieldName, (ISongLink)songInList, languagePreference);
			}

		}

		public string ApplyFormat(SongList songList, string format, ContentLanguagePreference languagePreference, bool includeHeader) {

			return ApplyFormat(songList.SongLinks, format, languagePreference, includeHeader);

		}

	}
}
