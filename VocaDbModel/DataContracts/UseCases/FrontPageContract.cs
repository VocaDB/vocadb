using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Comments;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.UseCases {

	public class FrontPageContract {

		public FrontPageContract() { }

		public FrontPageContract(IEnumerable<ActivityEntry> activityEntries,
			AlbumContract[] newAlbums, IEnumerable<EntryWithCommentsContract> recentComments, AlbumContract[] topAlbums, Song[] newSongs,
			SongVoteRating firstSongRating,
			ContentLanguagePreference languagePreference, IEntryThumbPersister entryThumbPersister, IEntryImagePersisterOld entryImagePersisterOld,
			bool ssl) {

			ActivityEntries = activityEntries.Select(e => new ActivityEntryContract(e, languagePreference, entryThumbPersister, entryImagePersisterOld, ssl)).ToArray();
			NewAlbums = newAlbums;
			NewSongs = newSongs.Select(s => new SongWithPVAndVoteContract(s, SongVoteRating.Nothing, languagePreference)).ToArray();
			RecentComments = recentComments.ToArray();
			TopAlbums = topAlbums;

			FirstSong = (newSongs.Any() ? new SongWithPVAndVoteContract(newSongs.First(), firstSongRating, languagePreference) : null);

		}

		public ActivityEntryContract[] ActivityEntries { get; set; }

		public SongWithPVAndVoteContract FirstSong { get; set; }

		public AlbumContract[] NewAlbums { get; set; }	

		public SongWithPVAndVoteContract[] NewSongs { get; set; }

		public EntryWithCommentsContract[] RecentComments { get; set; }

		public AlbumContract[] TopAlbums { get; set; }

	}
}
