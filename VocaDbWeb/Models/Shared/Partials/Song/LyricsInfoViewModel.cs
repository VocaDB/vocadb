using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song
{

	public class LyricsInfoViewModel
	{

		public LyricsInfoViewModel(LyricsForSongContract lyrics)
		{
			Lyrics = lyrics;
		}

		public LyricsForSongContract Lyrics { get; set; }

	}

}