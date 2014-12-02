using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.DataContracts.UseCases {

	public class FrontPageContract {

		public FrontPageContract() { }

		public FrontPageContract(IEnumerable<ActivityEntry> activityEntries, IEnumerable<NewsEntry> newsEntries,
			AlbumContract[] newAlbums, IEnumerable<UnifiedCommentContract> recentComments, AlbumContract[] topAlbums, Song[] newSongs,
			SongVoteRating firstSongRating,
			ContentLanguagePreference languagePreference) {

			ActivityEntries = activityEntries.Select(e => new ActivityEntryContract(e, languagePreference)).ToArray();
			NewAlbums = newAlbums;
			NewSongs = newSongs.Select(s => new SongWithPVAndVoteContract(s, SongVoteRating.Nothing, languagePreference)).ToArray();
			NewsEntries = newsEntries.Select(e => new NewsEntryContract(e)).ToArray();
			RecentComments = recentComments.ToArray();
			TopAlbums = topAlbums;

			FirstSong = (newSongs.Any() ? new SongWithPVAndVoteContract(newSongs.First(), firstSongRating, languagePreference) : null);

		}

		public ActivityEntryContract[] ActivityEntries { get; set; }

		public SongWithPVAndVoteContract FirstSong { get; set; }

		public AlbumContract[] NewAlbums { get; set; }	

		public NewsEntryContract[] NewsEntries { get; set; }

		public SongWithPVAndVoteContract[] NewSongs { get; set; }

		public UnifiedCommentContract[] RecentComments { get; set; }

		public AlbumContract[] TopAlbums { get; set; }

	}
}
