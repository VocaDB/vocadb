#nullable disable

using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song
{
	public class PrintArchivedSongDataViewModel
	{
		public PrintArchivedSongDataViewModel(ComparedSongsContract comparedSongs)
		{
			ComparedSongs = comparedSongs;
		}

		public ComparedSongsContract ComparedSongs { get; set; }
	}
}