using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Comments;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.UseCases {

	public class FrontPageContract {

		public FrontPageContract() { }

		public FrontPageContract(IEnumerable<ActivityEntry> activityEntries,
			AlbumContract[] newAlbums, 
			EntryForApiContract[] newEvents, 
			IEnumerable<EntryWithCommentsContract> recentComments, 
			AlbumContract[] topAlbums, Song[] newSongs,
			SongVoteRating firstSongRating,
			ContentLanguagePreference languagePreference, bool ssl, IUserIconFactory userIconFactory, IUserPermissionContext permissionContext, 
			EntryForApiContractFactory entryForApiContractFactory) {

			ActivityEntries = activityEntries.Select(e => new ActivityEntryForApiContract(e,
				entryForApiContractFactory.Create(e.EntryBase, EntryOptionalFields.AdditionalNames | EntryOptionalFields.MainPicture, languagePreference, ssl), 
				 userIconFactory, permissionContext, ActivityEntryOptionalFields.None)).ToArray();
			NewAlbums = newAlbums;
			NewSongs = newSongs.Select(s => new SongWithPVAndVoteContract(s, SongVoteRating.Nothing, languagePreference)).ToArray();
			RecentComments = recentComments.ToArray();
			TopAlbums = topAlbums;
			NewEvents = newEvents;

			FirstSong = (newSongs.Any() ? new SongWithPVAndVoteContract(newSongs.First(), firstSongRating, languagePreference) : null);

		}

		public ActivityEntryForApiContract[] ActivityEntries { get; set; }

		public SongWithPVAndVoteContract FirstSong { get; set; }

		public AlbumContract[] NewAlbums { get; set; }	

		/// <summary>
		/// Album release events and concerts
		/// </summary>
		public EntryForApiContract[] NewEvents { get; set; }

		public SongWithPVAndVoteContract[] NewSongs { get; set; }

		public EntryWithCommentsContract[] RecentComments { get; set; }

		public AlbumContract[] TopAlbums { get; set; }

	}
}
