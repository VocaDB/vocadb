#nullable disable

using System.Collections.Generic;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices
{
	public class NicoTitleParseResult
	{
		public NicoTitleParseResult(string title)
			: this(title, new List<Artist>(), SongType.Unspecified) { }

		public NicoTitleParseResult(string title, List<Artist> artistNames, SongType songType)
		{
			Artists = artistNames;
			Title = title;
			SongType = songType;
		}

		public List<Artist> Artists { get; set; }

		public SongType SongType { get; set; }

		public string Title { get; set; }

		public ContentLanguageSelection TitleLanguage { get; set; }
	}
}
