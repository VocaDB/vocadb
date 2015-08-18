using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.TagFormatting {

	public class SongListFormatter : SongCsvFileFormatter<SongInList> {

		private readonly IEntryLinkFactory entryLinkFactory;

		public SongListFormatter(IEntryLinkFactory entryLinkFactory) {
			this.entryLinkFactory = entryLinkFactory;
		}

		private string GetPvUrl(Song song, PVType? type, PVServices services) {
			var pv = song.PVs.FirstOrDefault(p => (type == null || p.PVType == type) && services.HasFlag((PVServices)p.Service));
			return pv != null ? pv.Url : string.Empty;
        }

		protected override string GetFieldValue(string fieldName, SongInList songInList, ContentLanguagePreference languagePreference) {

			var song = songInList.Song;

			switch (fieldName) {
				case "notes":
					return songInList.Notes;
				case "publishdate":
					return song.PublishDate.DateTime.HasValue ? song.PublishDate.ToString() : string.Empty;
				case "pv.original.niconicodouga":
					return GetPvUrl(song, PVType.Original, PVServices.NicoNicoDouga);
				case "pv.original.!niconicodouga":
					return GetPvUrl(song, PVType.Original, (PVServices)(EnumVal<PVServices>.All - PVServices.NicoNicoDouga));
				case "pv.reprint":
					return GetPvUrl(song, PVType.Reprint, EnumVal<PVServices>.All);
				case "title":
					return song.Names.SortNames[languagePreference];
				case "url":
					return entryLinkFactory.GetFullEntryUrl(EntryType.Song, song.Id);
				default:
					return string.Empty;
			}

		}

		public string ApplyFormat(SongList songList, string format, ContentLanguagePreference languagePreference, bool includeHeader) {

			return ApplyFormat(songList.SongLinks, format, languagePreference, includeHeader);

		}

	}
}
