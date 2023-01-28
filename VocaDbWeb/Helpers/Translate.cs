#nullable disable

using System.Reflection;
using Resources;
using VocaDb.Model;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Helpers;

public static class Translate
{
	private static readonly Dictionary<Type, ITranslateableEnum> s_allResourceManagers;

	private static Type GetEnumType(FieldInfo property)
	{
		return property.FieldType.GetGenericArguments().First();
	}

	private static ITranslateableEnum GetResourceManager(FieldInfo property)
	{
		return (ITranslateableEnum)property.GetValue(null);
	}

	static Translate()
	{
		var enums = typeof(Translate).GetFields().Where(p => typeof(ITranslateableEnum).IsAssignableFrom(p.FieldType));
		s_allResourceManagers = enums
			.Select(p => new
			{
				TranslateableEnum = GetResourceManager(p),
				EnumType = GetEnumType(p)
			})
			.Distinct(p => p.EnumType)
			.ToDictionary(p => p.EnumType, p => p.TranslateableEnum);
	}

	public static readonly TranslateableEnum<ActivityEntrySortRule> ActivityEntrySortRuleNames =
		new(() => global::Resources.ActivityEntrySortRuleNames.ResourceManager);

	public static readonly TranslateableEnum<EntryType> ActivityEntryTargetTypeNames =
		new(() => Resources.Domain.EntryTypeNames.ResourceManager, new[]
		{
			EntryType.Undefined,
			EntryType.Album,
			EntryType.Artist,
			EntryType.ReleaseEvent,
			EntryType.Song,
			EntryType.SongList,
			EntryType.Tag,
			EntryType.Venue,
		});

	public static readonly TranslateableEnum<PurchaseStatus> AlbumCollectionStatusNames =
		new(() => global::Resources.AlbumCollectionStatusNames.ResourceManager);

	public static readonly TranslateableEnum<AlbumEditableFields> AlbumEditableFieldNames =
		new(() => global::Resources.AlbumEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<MediaType> AlbumMediaTypeNames =
		new(() => global::Resources.AlbumMediaTypeNames.ResourceManager);

	public static readonly TranslateableEnum<AlbumReportType> AlbumReportTypeNames =
		new(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

	public static readonly TranslateableEnum<AlbumSortRule> AlbumSortRuleNames =
		new(() => global::Resources.AlbumSortRuleNames.ResourceManager, new[] {
			AlbumSortRule.Name, AlbumSortRule.AdditionDate, AlbumSortRule.ReleaseDate, AlbumSortRule.RatingAverage, AlbumSortRule.RatingTotal,
			AlbumSortRule.CollectionCount
		});

	public static readonly TranslateableEnum<ArtistEditableFields> ArtistEditableFieldNames =
		new(() => global::Resources.ArtistEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<ArtistEventRoles> ArtistEventRoleNames =
		new(() => Resources.Domain.ReleaseEvents.ArtistEventRoleNames.ResourceManager);

	public static TranslateableEnum<ArtistLinkType> ArtistLinkTypeNames => new(() => Resources.Domain.Artists.ArtistLinkTypeNames.ResourceManager);

	public static readonly TranslateableEnum<ArtistReportType> ArtistReportTypeNames =
		new(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

	public static readonly TranslateableEnum<ArtistRoles> ArtistRoleNames =
		new(() => global::Resources.ArtistRoleNames.ResourceManager);

	public static readonly TranslateableEnum<ArtistSortRule> ArtistSortRuleNames =
		new(() => global::Resources.ArtistSortRuleNames.ResourceManager, new[] {
			ArtistSortRule.Name, ArtistSortRule.AdditionDate, ArtistSortRule.AdditionDateAsc,
			ArtistSortRule.ReleaseDate,
			ArtistSortRule.SongCount, ArtistSortRule.SongRating, ArtistSortRule.FollowerCount
		});

	public static TranslateableEnum<ArtistType> ArtistTypeNames => new(() => Model.Resources.ArtistTypeNames.ResourceManager);

	public static readonly TranslateableEnum<CommentSortRule> CommentSortRuleNames =
		new(() => global::Resources.CommentSortRuleNames.ResourceManager);

	public static readonly TranslateableEnum<EntryType> CommentTargetTypeNames =
		new(() => Resources.Domain.EntryTypeNames.ResourceManager, new[]
		{
			EntryType.Undefined,
			EntryType.Album,
			EntryType.Artist,
			EntryType.DiscussionTopic,
			EntryType.ReleaseEvent,
			EntryType.Song,
			EntryType.SongList,
			EntryType.Tag,
			EntryType.User,
		});

	public static readonly TranslateableEnum<ContentLanguageSelection> ContentLanguageSelectionNames =
		new(() => global::Resources.ContentLanguageSelectionNames.ResourceManager);

	public static TranslateableEnum<DiscType> DiscTypeNames =
		new(() => Model.Resources.Albums.DiscTypeNames.ResourceManager);

	public static readonly TranslateableEnum<EntryEditEvent> EntryEditEventNames =
		new(() => global::Resources.EntryEditEventNames.ResourceManager);

	public static readonly TranslateableEnum<EntryStatus> EntryStatusNames =
		new(() => global::Resources.EntryStatusNames.ResourceManager);

	public static readonly TranslateableEnum<EntryType> EntryTypeNames =
		new(() => Resources.Domain.EntryTypeNames.ResourceManager);

	public static readonly TranslateableEnum<EventReportType> EventReportTypeNames =
		new(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

	public static readonly TranslateableEnum<EventSortRule> EventSortRuleNames =
		new(() => Resources.Domain.ReleaseEvents.EventSortRuleNames.ResourceManager, EnumVal<EventSortRule>.Values.Where(s => s != EventSortRule.None));

	public static readonly TranslateableEnum<PVType> PVTypeDescriptions =
		new(() => global::Resources.PVTypeDescriptions.ResourceManager);

	public static readonly TranslateableEnum<PVType> PVTypeNames =
		new(() => global::Resources.PVTypeNames.ResourceManager);

	public static readonly TranslateableEnum<EventCategory> ReleaseEventCategoryNames =
		new(() => Resources.Domain.ReleaseEvents.EventCategoryNames.ResourceManager);

	public static readonly TranslateableEnum<ReleaseEventEditableFields> ReleaseEventEditableFieldNames =
		new(() => global::Resources.ReleaseEventEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<ReleaseEventSeriesEditableFields> ReleaseEventSeriesEditableFieldNames =
		new(() => Resources.Domain.ReleaseEvents.ReleaseEventSeriesEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<SongEditableFields> SongEditableFieldNames =
		new(() => global::Resources.SongEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<SongListEditableFields> SongListEditableFieldNames =
		new(() => global::Resources.SongListEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<SongListFeaturedCategory> SongListFeaturedCategoryNames =
		new(() => global::Resources.SongListFeaturedCategoryNames.ResourceManager);

	public static readonly TranslateableEnum<SongReportType> SongReportTypeNames =
		new(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

	public static readonly TranslateableEnum<SongSortRule> SongSortRuleNames =
		new(() => global::Resources.SongSortRuleNames.ResourceManager, new[] {
			SongSortRule.Name, SongSortRule.AdditionDate, SongSortRule.PublishDate, SongSortRule.RatingScore, SongSortRule.FavoritedTimes,
			SongSortRule.TagUsageCount
		});

	public static readonly DerivedTranslateableEnum<RatedSongForUserSortRule, SongSortRule> RatedSongForUserSortRuleNames =
		new(SongSortRuleNames, () => global::Resources.RatedSongForUserSortRuleNames.ResourceManager, new[] {
			RatedSongForUserSortRule.Name, RatedSongForUserSortRule.AdditionDate, RatedSongForUserSortRule.PublishDate,
			RatedSongForUserSortRule.RatingScore, RatedSongForUserSortRule.FavoritedTimes, RatedSongForUserSortRule.RatingDate
		});

	public static readonly TranslateableEnum<SongType> SongTypeNames =
		new(() => Model.Resources.Songs.SongTypeNames.ResourceManager);

	public static readonly TranslateableEnum<SongVoteRating> SongVoteRatingNames =
		new(() => global::Resources.SongVoteRatingNames.ResourceManager);

	public static readonly TranslateableEnum<TagEditableFields> TagEditableFieldNames =
		new(() => global::Resources.TagEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<TagReportType> TagReportTypeNames =
		new(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

	public static readonly TranslateableEnum<TagTargetTypes> TagTargetTypeNames =
		new(() => Resources.Domain.EntryTypeNames.ResourceManager);

	public static readonly TranslateableEnum<UserGroupId> UserGroups =
		new(() => UserGroupNames.ResourceManager);

	public static readonly TranslateableEnum<UserLanguageProficiency> UserLanguageProficiencyNames =
		new(() => Resources.Domain.Users.UserLanguageProficiencyNames.ResourceManager);

	public static readonly TranslateableEnum<UserReportType> UserReportTypeNames =
		new(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

	public static readonly TranslateableEnum<VenueEditableFields> VenueEditableFieldNames =
		new(() => global::Resources.VenueEditableFieldNames.ResourceManager);

	public static readonly TranslateableEnum<VenueReportType> VenueReportTypeNames =
		new(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

	public static readonly TranslateableEnum<WebLinkCategory> WebLinkCategoryNames =
		new(() => global::Resources.WebLinkCategoryNames.ResourceManager);

	public static readonly TranslateableEnum<WebhookEvents> WebhookEventNames =
		new(() => Resources.Domain.WebhookEventNames.ResourceManager);

	public static string AlbumEditableField(AlbumEditableFields field)
	{
		return AlbumEditableFieldNames.GetName(field);
	}

	public static string AlbumArchiveReason(AlbumArchiveReason reason)
	{
		return AlbumArchiveReasonNames.ResourceManager.GetString(reason.ToString());
	}

	public static string AllPermissionTokenNames(IEnumerable<PermissionToken> tokens)
	{
		return string.Join(", ", tokens.Select(t => PermissionTokenName(t)));
	}

	public static string ArtistEditableField(ArtistEditableFields field)
	{
		return ArtistEditableFieldNames.GetName(field);
	}

	public static string ArtistArchiveReason(ArtistArchiveReason reason)
	{
		return ArtistArchiveReasonNames.ResourceManager.GetString(reason.ToString());
	}

	public static string ArtistTypeName(ArtistType artistType)
	{
		return Model.Resources.ArtistTypeNames.ResourceManager.GetString(artistType.ToString());
	}

	public static string ContentLanguagePreferenceName(ContentLanguagePreference languagePreference)
	{
		return global::Resources.ContentLanguageSelectionNames.ResourceManager.GetString(languagePreference.ToString());
	}

	public static string ContentLanguageSelectionName(ContentLanguageSelection languageSelection)
	{
		return global::Resources.ContentLanguageSelectionNames.ResourceManager.GetString(languageSelection.ToString());
	}

	public static string DiscTypeName(DiscType discType)
	{
		return Model.Resources.Albums.DiscTypeNames.ResourceManager.GetString(discType.ToString());
	}

	public static string EmailOptions(UserEmailOptions emailOptions)
	{
		return UserEmailOptionsNames.ResourceManager.GetString(emailOptions.ToString());
	}

	public static string EntrySubTypeName(EntryTypeAndSubType fullEntryType) => fullEntryType.EntryType switch
	{
		EntryType.Album => DiscTypeName(EnumVal<DiscType>.Parse(fullEntryType.SubType)),
		EntryType.Artist => ArtistTypeName(EnumVal<ArtistType>.Parse(fullEntryType.SubType)),
		EntryType.ReleaseEvent => ReleaseEventCategoryNames[EnumVal<EventCategory>.Parse(fullEntryType.SubType)],
		EntryType.Song => SongTypeNames[EnumVal<SongType>.Parse(fullEntryType.SubType)],
		_ => string.Empty,
	};

	public static string SongArchiveReason(SongArchiveReason reason)
	{
		return SongArchiveReasonNames.ResourceManager.GetString(reason.ToString());
	}

	public static string SongEditableField(SongEditableFields field)
	{
		return SongEditableFieldNames[field];
	}

	public static string VenueEditableField(VenueEditableFields field) => VenueEditableFieldNames[field];

	public static string PermissionTokenName(IPermissionToken token)
	{
		if (PermissionToken.TryGetById(token.Id, out PermissionToken t))
			return PermissionTokenNames.ResourceManager.GetString(t.Name) ?? t.Name;
		else
			return (token.Name != null ? PermissionTokenNames.ResourceManager.GetString(token.Name) : null) ?? token.Name ?? token.Id.ToString();
	}

	public static TranslateableEnum<TEnum> Translations<TEnum>() where TEnum : struct, Enum
	{
		return (TranslateableEnum<TEnum>)s_allResourceManagers[typeof(TEnum)];
	}

	public static string ArtistCategoriesName(ArtistCategories value) => Model.Resources.ArtistCategoriesNames.ResourceManager.GetString(value.ToString());
}