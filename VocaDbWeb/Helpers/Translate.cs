using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Globalization;
using Resources;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Helpers {

	public static class Translate {

		public static readonly TranslateableEnum<PurchaseStatus> AlbumCollectionStatusNames =
			new TranslateableEnum<PurchaseStatus>(() => global::Resources.AlbumCollectionStatusNames.ResourceManager);

		public static readonly TranslateableEnum<AlbumEditableFields> AlbumEditableFieldNames =
			new TranslateableEnum<AlbumEditableFields>(() => global::Resources.AlbumEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<MediaType> AlbumMediaTypeNames =
			new TranslateableEnum<MediaType>(() => global::Resources.AlbumMediaTypeNames.ResourceManager);

		public static readonly TranslateableEnum<AlbumReportType> AlbumReportTypeNames =
			new TranslateableEnum<AlbumReportType>(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<AlbumSortRule> AlbumSortRuleNames =
			new TranslateableEnum<AlbumSortRule>(() => global::Resources.AlbumSortRuleNames.ResourceManager, new[] {
				AlbumSortRule.Name, AlbumSortRule.AdditionDate, AlbumSortRule.ReleaseDate, AlbumSortRule.RatingAverage, AlbumSortRule.RatingTotal,
				AlbumSortRule.CollectionCount
			});

		public static readonly TranslateableEnum<ArtistEditableFields> ArtistEditableFieldNames =
			new TranslateableEnum<ArtistEditableFields>(() => global::Resources.ArtistEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<ArtistReportType> ArtistReportTypeNames =
			new TranslateableEnum<ArtistReportType>(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<ArtistRoles> ArtistRoleNames =
			new TranslateableEnum<ArtistRoles>(() => global::Resources.ArtistRoleNames.ResourceManager);

		public static readonly TranslateableEnum<ArtistSortRule> ArtistSortRuleNames =
			new TranslateableEnum<ArtistSortRule>(() => global::Resources.ArtistSortRuleNames.ResourceManager, new[] {
				ArtistSortRule.Name, ArtistSortRule.AdditionDate, ArtistSortRule.AdditionDateAsc,
				ArtistSortRule.SongCount, ArtistSortRule.SongRating, ArtistSortRule.FollowerCount
			});

		public static TranslateableEnum<ArtistType> ArtistTypeNames {
			get {
				return new TranslateableEnum<ArtistType>(() => Model.Resources.ArtistTypeNames.ResourceManager);
			}
		}			

		public static readonly TranslateableEnum<ContentLanguageSelection> ContentLanguageSelectionNames =
			new TranslateableEnum<ContentLanguageSelection>(() => global::Resources.ContentLanguageSelectionNames.ResourceManager);

		public static TranslateableEnum<DiscType> DiscTypeNames {
			get {
				return new TranslateableEnum<DiscType>(() => Model.Resources.Albums.DiscTypeNames.ResourceManager);
			}
		}			

		public static readonly TranslateableEnum<EntryEditEvent> EntryEditEventNames =
			new TranslateableEnum<EntryEditEvent>(() => global::Resources.EntryEditEventNames.ResourceManager);

		public static readonly TranslateableEnum<EntryStatus> EntryStatusNames =
			new TranslateableEnum<EntryStatus>(() => global::Resources.EntryStatusNames.ResourceManager);

		public static readonly TranslateableEnum<EntryType> EntryTypeNames =
			new TranslateableEnum<EntryType>(() => global::Resources.EntryTypeNames.ResourceManager);

		public static readonly TranslateableEnum<PVType> PVTypeDescriptions =
			new TranslateableEnum<PVType>(() => global::Resources.PVTypeDescriptions.ResourceManager);

		public static readonly TranslateableEnum<PVType> PVTypeNames =
			new TranslateableEnum<PVType>(() => global::Resources.PVTypeNames.ResourceManager);

		public static readonly TranslateableEnum<ReleaseEventEditableFields> ReleaseEventEditableFieldNames =
			new TranslateableEnum<ReleaseEventEditableFields>(() => global::Resources.ReleaseEventEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<SongEditableFields> SongEditableFieldNames =
			new TranslateableEnum<SongEditableFields>(() => global::Resources.SongEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<SongListEditableFields> SongListEditableFieldNames =
			new TranslateableEnum<SongListEditableFields>(() => global::Resources.SongListEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<SongListFeaturedCategory> SongListFeaturedCategoryNames =
			new TranslateableEnum<SongListFeaturedCategory>(() => global::Resources.SongListFeaturedCategoryNames.ResourceManager);

		public static readonly TranslateableEnum<SongReportType> SongReportTypeNames =
			new TranslateableEnum<SongReportType>(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<SongSortRule> SongSortRuleNames =
			new TranslateableEnum<SongSortRule>(() => global::Resources.SongSortRuleNames.ResourceManager, new[] {
				SongSortRule.Name, SongSortRule.AdditionDate, SongSortRule.PublishDate, SongSortRule.RatingScore, SongSortRule.FavoritedTimes,
			});

		public static readonly DerivedTranslateableEnum<RatedSongForUserSortRule, SongSortRule> RatedSongForUserSortRuleNames =
			new DerivedTranslateableEnum<RatedSongForUserSortRule, SongSortRule>(SongSortRuleNames, () => global::Resources.RatedSongForUserSortRuleNames.ResourceManager, new[] {
				RatedSongForUserSortRule.Name, RatedSongForUserSortRule.AdditionDate, RatedSongForUserSortRule.PublishDate,
				RatedSongForUserSortRule.RatingScore, RatedSongForUserSortRule.FavoritedTimes, RatedSongForUserSortRule.RatingDate
			});

		public static readonly TranslateableEnum<SongType> SongTypeNames =
			new TranslateableEnum<SongType>(() => Model.Resources.Songs.SongTypeNames.ResourceManager);

		public static readonly TranslateableEnum<SongVoteRating> SongVoteRatingNames =
			new TranslateableEnum<SongVoteRating>(() => global::Resources.SongVoteRatingNames.ResourceManager);

		public static readonly TranslateableEnum<TagEditableFields> TagEditableFieldNames =
			new TranslateableEnum<TagEditableFields>(() => global::Resources.TagEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<UserGroupId> UserGroups =
			new TranslateableEnum<UserGroupId>(() => UserGroupNames.ResourceManager);

		public static readonly TranslateableEnum<UserReportType> UserReportTypeNames =
			new TranslateableEnum<UserReportType>(() => Resources.Domain.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<WebLinkCategory> WebLinkCategoryNames =
			new TranslateableEnum<WebLinkCategory>(() => global::Resources.WebLinkCategoryNames.ResourceManager);

		public static string AlbumEditableField(AlbumEditableFields field) {

			return AlbumEditableFieldNames.GetName(field);

		}

		public static string AlbumArchiveReason(AlbumArchiveReason reason) {

			return AlbumArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string AllPermissionTokenNames(IEnumerable<PermissionToken> tokens) {

			return string.Join(", ", tokens.Select(t => PermissionTokenName(t)));

		}

		public static string ArtistEditableField(ArtistEditableFields field) {

			return ArtistEditableFieldNames.GetName(field);

		}

		public static string ArtistArchiveReason(ArtistArchiveReason reason) {

			return ArtistArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string ArtistTypeName(ArtistType artistType) {

			return Model.Resources.ArtistTypeNames.ResourceManager.GetString(artistType.ToString());

		}

		public static string ContentLanguagePreferenceName(ContentLanguagePreference languagePreference) {

			return global::Resources.ContentLanguageSelectionNames.ResourceManager.GetString(languagePreference.ToString());

		}

		public static string ContentLanguageSelectionName(ContentLanguageSelection languageSelection) {

			return global::Resources.ContentLanguageSelectionNames.ResourceManager.GetString(languageSelection.ToString());

		}

		public static string DiscTypeName(DiscType discType) {

			return Model.Resources.Albums.DiscTypeNames.ResourceManager.GetString(discType.ToString());

		}

		public static string EmailOptions(UserEmailOptions emailOptions) {

			return UserEmailOptionsNames.ResourceManager.GetString(emailOptions.ToString());

		}

		public static string SongArchiveReason(SongArchiveReason reason) {

			return SongArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string SongEditableField(SongEditableFields field) {

			return SongEditableFieldNames[field];

		}

		public static string PermissionTokenName(IPermissionToken token) {

			PermissionToken t;
			if (PermissionToken.TryGetById(token.Id, out t)) {
				return PermissionTokenNames.ResourceManager.GetString(t.Name) ?? t.Name;
			} else {
				return (token.Name != null ? PermissionTokenNames.ResourceManager.GetString(token.Name) : null) ?? token.Name ?? token.Id.ToString();
			}

		}

	}

}