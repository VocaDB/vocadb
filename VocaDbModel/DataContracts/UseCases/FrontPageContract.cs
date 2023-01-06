#nullable disable

using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Comments;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases;

public class FrontPageContract
{
	public FrontPageContract() { }

	public FrontPageContract(IEnumerable<ActivityEntry> activityEntries,
		AlbumForApiContract[] newAlbums,
		ReleaseEventForApiContract[] newEvents,
		IEnumerable<EntryWithCommentsContract> recentComments,
		AlbumForApiContract[] topAlbums, Song[] newSongs,
		SongVoteRating firstSongRating,
		ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory, IUserPermissionContext permissionContext,
		EntryForApiContractFactory entryForApiContractFactory)
	{
		ActivityEntries = activityEntries.Select(e => new ActivityEntryForApiContract(e,
			entryForApiContractFactory.Create(e.EntryBase, EntryOptionalFields.AdditionalNames | EntryOptionalFields.MainPicture, languagePreference),
			 userIconFactory, permissionContext, ActivityEntryOptionalFields.None)).ToArray();
		NewAlbums = newAlbums;
		NewSongs = newSongs.Select(s => new SongWithPVAndVoteContract(s, SongVoteRating.Nothing, languagePreference)).ToArray();
		RecentComments = recentComments.ToArray();
		TopAlbums = topAlbums;
		NewEvents = newEvents;

		FirstSong = (newSongs.Any() ? new SongWithPVAndVoteContract(newSongs.First(), firstSongRating, languagePreference) : null);
	}

	public ActivityEntryForApiContract[] ActivityEntries { get; init; }

	public SongWithPVAndVoteContract FirstSong { get; init; }

	public AlbumForApiContract[] NewAlbums { get; init; }

	public ReleaseEventForApiContract[] NewEvents { get; init; }

	public SongWithPVAndVoteContract[] NewSongs { get; init; }

	public EntryWithCommentsContract[] RecentComments { get; init; }

	public AlbumForApiContract[] TopAlbums { get; init; }
}
