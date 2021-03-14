#nullable disable

using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class CreateSongContract
	{
		public ArtistForSongContract[] Artists { get; init; }

		public bool Draft { get; init; }

		public LyricsForSongContract[] Lyrics { get; init; }

		public LocalizedStringContract[] Names { get; init; }

		public SongContract OriginalVersion { get; init; }

		public string[] PVUrls { get; set; }

		public string ReprintPVUrl { get; init; }

		public SongType SongType { get; set; }

		public string UpdateNotes { get; init; }

		public WebLinkContract[] WebLinks { get; init; }
	}
}
