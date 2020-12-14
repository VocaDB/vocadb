#nullable disable

using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class CreateSongContract
	{
		public ArtistForSongContract[] Artists { get; set; }

		public bool Draft { get; set; }

		public LyricsForSongContract[] Lyrics { get; set; }

		public LocalizedStringContract[] Names { get; set; }

		public SongContract OriginalVersion { get; set; }

		public string[] PVUrls { get; set; }

		public string ReprintPVUrl { get; set; }

		public SongType SongType { get; set; }

		public string UpdateNotes { get; set; }

		public WebLinkContract[] WebLinks { get; set; }
	}
}
