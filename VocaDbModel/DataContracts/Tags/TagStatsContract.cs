using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagStatsContract
	{
		public TagStatsContract() { }

		public TagStatsContract(
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbStore,
			IEnumerable<Artist> artists, int artistCount, IEnumerable<Album> albums, int albumCount,
			IEnumerable<SongList> songLists, int songListCount,
			IEnumerable<Song> songs, int songCount,
			IEnumerable<ReleaseEventSeries> eventSeries, int eventSeriesCount,
			IEnumerable<ReleaseEvent> events, int eventCount,
			int followerCount)
		{
			Albums = albums.Select(a => new AlbumContract(a, languagePreference)).ToArray();
			AlbumCount = albumCount;

			Artists = artists.Select(a => new ArtistContract(a, languagePreference)).ToArray();
			ArtistCount = artistCount;

			EventSeries = eventSeries.Select(a => new ReleaseEventSeriesContract(a, languagePreference, false)).ToArray();
			EventSeriesCount = eventSeriesCount;

			Events = events.Select(a => new ReleaseEventForApiContract(a, languagePreference, ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Venue, thumbStore)).ToArray();
			EventCount = eventCount;

			SongLists = songLists.Select(a => new SongListBaseContract(a)).ToArray();
			SongListCount = songListCount;

			Songs = songs.Select(a => new SongForApiContract(a, languagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl)).ToArray();
			SongCount = songCount;

			FollowerCount = followerCount;
		}

		public int AlbumCount { get; set; }

		public int ArtistCount { get; set; }

		public AlbumContract[] Albums { get; set; }

		public ArtistContract[] Artists { get; set; }

		public int EventCount { get; set; }

		public int EventSeriesCount { get; set; }

		public ReleaseEventForApiContract[] Events { get; set; }

		public ReleaseEventSeriesContract[] EventSeries { get; set; }

		public int FollowerCount { get; set; }

		public int SongListCount { get; set; }

		public SongListBaseContract[] SongLists { get; set; }

		public SongForApiContract[] Songs { get; set; }

		public int SongCount { get; set; }
	}
}
