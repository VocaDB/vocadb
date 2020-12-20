#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.TagFormatting
{
	public class SongListFormatter : SongCsvFileFormatter<SongInList>
	{
		public SongListFormatter(IEntryLinkFactory entryLinkFactory)
			: base(entryLinkFactory)
		{
		}

		protected override string GetFieldValue(string fieldName, SongInList songInList, ContentLanguagePreference languagePreference) => fieldName switch
		{
			"notes" => songInList.Notes,
			_ => GetFieldValue(fieldName, (ISongLink)songInList, languagePreference),
		};

		public string ApplyFormat(SongList songList, string format, ContentLanguagePreference languagePreference, bool includeHeader)
		{
			return ApplyFormat(songList.SongLinks, format, languagePreference, includeHeader);
		}
	}
}
