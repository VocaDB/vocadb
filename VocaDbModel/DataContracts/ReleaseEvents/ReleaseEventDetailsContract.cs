#nullable disable

using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[Obsolete]
public class ReleaseEventDetailsContract : ReleaseEventContract
{
	public ReleaseEventDetailsContract()
	{
		Artists = Array.Empty<ArtistForEventContract>();
		PVs = Array.Empty<PVContract>();
		WebLinks = Array.Empty<WebLinkContract>();
	}

	public ReleaseEventDetailsContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference,
		IUserPermissionContext userContext, IUserIconFactory userIconFactory, IEntryTypeTagRepository entryTypeTags = null)
		: base(releaseEvent, languagePreference, true, true)
	{
		ParamIs.NotNull(() => releaseEvent);

		CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, releaseEvent);
		DefaultNameLanguage = releaseEvent.TranslatedName.DefaultLanguage;
		PVs = releaseEvent.PVs.Select(p => new PVContract(p)).ToArray();
		SeriesNumber = releaseEvent.SeriesNumber;
		SeriesSuffix = releaseEvent.SeriesSuffix;
		Tags = releaseEvent.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).OrderByDescending(t => t.Count).ToArray();
		TranslatedName = new TranslatedStringContract(releaseEvent.TranslatedName);
		WebLinks = releaseEvent.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		var categoryTag = entryTypeTags?.GetTag(Domain.EntryType.ReleaseEvent, InheritedCategory);
		InheritedCategoryTag = categoryTag != null ? new TagBaseContract(categoryTag, languagePreference) : null;

		Albums = releaseEvent.Albums
			.Select(a => new AlbumContract(a, languagePreference))
			.OrderBy(a => a.Name)
			.ToArray();

		Artists = releaseEvent.AllArtists
			.Select(a => new ArtistForEventContract(a, languagePreference))
			.OrderBy(a => a.Artist != null ? a.Artist.Name : a.Name)
			.ToArray();

		Songs = releaseEvent.Songs
			.Select(s => new SongForApiContract(s, languagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl))
			.OrderBy(s => s.Name)
			.ToArray();

		UsersAttending = releaseEvent.Users
			.Where(u => u.RelationshipType == UserEventRelationshipType.Attending)
			.Select(u => new UserForApiContract(u.User, userIconFactory, UserOptionalFields.MainPicture))
			.ToArray();

		if (releaseEvent.SongList != null)
		{
			SongListSongs = releaseEvent.SongList.SongLinks.OrderBy(s => s.Order).Select(s => new SongInListContract(s, languagePreference)).ToArray();
		}
	}

	public AlbumContract[] Albums { get; init; }

	public ReleaseEventSeriesContract[] AllSeries { get; init; }

	public ArtistForEventContract[] Artists { get; init; }

	public bool CanRemoveTagUsages { get; init; }

	public ContentLanguageSelection DefaultNameLanguage { get; set; }

	public UserEventRelationshipType? EventAssociationType { get; init; }

	public TagBaseContract InheritedCategoryTag { get; init; }

	public CommentForApiContract[] LatestComments { get; init; }

	public PVContract[] PVs { get; init; }

	public int SeriesNumber { get; init; }

	public string SeriesSuffix { get; set; }

	public SongInListContract[] SongListSongs { get; init; }

	public SongForApiContract[] Songs { get; init; }

	public TagUsageForApiContract[] Tags { get; init; }

	public TranslatedStringContract TranslatedName { get; init; }

	public UserForApiContract[] UsersAttending { get; init; }

	public WebLinkContract[] WebLinks { get; init; }
}
