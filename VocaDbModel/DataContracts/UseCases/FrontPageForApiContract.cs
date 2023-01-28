using System.Runtime.Serialization;
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

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record FrontPageForApiContract
{
	[DataMember]
	public ActivityEntryForApiContract[] ActivityEntries { get; init; }

	[DataMember]
	public SongWithPVAndVoteForApiContract? FirstSong { get; init; }

	[DataMember]
	public AlbumForApiContract[] NewAlbums { get; init; }

	[DataMember]
	public ReleaseEventForApiContract[] NewEvents { get; init; }

	[DataMember]
	public SongWithPVAndVoteForApiContract[] NewSongs { get; init; }

	[DataMember]
	public EntryWithCommentsContract[] RecentComments { get; init; }

	[DataMember]
	public AlbumForApiContract[] TopAlbums { get; init; }

	public FrontPageForApiContract(
		IEnumerable<ActivityEntry> activityEntries,
		AlbumForApiContract[] newAlbums,
		ReleaseEventForApiContract[] newEvents,
		IEnumerable<EntryWithCommentsContract> recentComments,
		AlbumForApiContract[] topAlbums,
		Song[] newSongs,
		SongVoteRating firstSongRating,
		ContentLanguagePreference languagePreference,
		IUserIconFactory userIconFactory,
		IUserPermissionContext permissionContext,
		EntryForApiContractFactory entryForApiContractFactory
	)
	{
		ActivityEntries = activityEntries
			.Select(e => new ActivityEntryForApiContract(
				activityEntry: e,
				entryForApiContract: entryForApiContractFactory.Create(
					entry: e.EntryBase,
					includedFields: EntryOptionalFields.AdditionalNames | EntryOptionalFields.MainPicture,
					languagePreference: languagePreference
				),
				userIconFactory: userIconFactory,
				permissionContext: permissionContext,
				fields: ActivityEntryOptionalFields.None
			))
			.ToArray();

		NewAlbums = newAlbums;

		NewSongs = newSongs
			.Select(s => new SongWithPVAndVoteForApiContract(
				song: s,
				vote: SongVoteRating.Nothing,
				languagePreference: languagePreference
			))
			.ToArray();

		RecentComments = recentComments.ToArray();
		TopAlbums = topAlbums;
		NewEvents = newEvents;

		FirstSong = newSongs.Any()
			? new SongWithPVAndVoteForApiContract(
				song: newSongs.First(),
				vote: firstSongRating,
				languagePreference: languagePreference
			)
			: null;
	}
}
