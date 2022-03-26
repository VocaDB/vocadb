#nullable disable

using System;
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
	[Obsolete]
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

		public int AlbumCount { get; init; }

		public int ArtistCount { get; init; }

		public AlbumContract[] Albums { get; init; }

		public ArtistContract[] Artists { get; init; }

		public int EventCount { get; init; }

		public int EventSeriesCount { get; init; }

		public ReleaseEventForApiContract[] Events { get; init; }

		public ReleaseEventSeriesContract[] EventSeries { get; init; }

		public int FollowerCount { get; init; }

		public int SongListCount { get; init; }

		public SongListBaseContract[] SongLists { get; init; }

		public SongForApiContract[] Songs { get; init; }

		public int SongCount { get; init; }
	}
}
