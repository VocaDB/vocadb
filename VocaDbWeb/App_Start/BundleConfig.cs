using System.Web.Optimization;

namespace VocaDb.Web.App_Start {

	public static class BundleConfig {

		public static void RegisterBundles(BundleCollection bundles) {

			bundles.Add(new ScriptBundle("~/bundles/shared/libs").Include(
				"~/Scripts/jquery-{version}.js", 
				"~/Scripts/bootstrap.js",
				//"~/Scripts/jquery-ui-1.10.1.js", // doesn't work if bundled together
				"~/Scripts/knockout-{version}.js",
				"~/Scripts/knockout.punches.min.js",
				"~/Scripts/lodash.js", 
				"~/Scripts/qTip/jquery.qtip.js",
				"~/Scripts/marked.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/shared/jqui").Include(
				"~/Scripts/jquery-ui-{version}.js"
			));

			// SHARED BUNDLES
			// Legacy common scripts - should be phased out
			bundles.Add(new ScriptBundle("~/bundles/VocaDB").Include("~/Scripts/VocaDB.js"));

			// Included on all pages (including front page)
			// Generally the references go from viewmodels -> repositories -> models -> support classes
			bundles.Add(new ScriptBundle("~/bundles/shared/common").Include(
				"~/Scripts/Shared/TopBar.js", 
				"~/Scripts/Shared/MessagesTyped.js",
				"~/Scripts/Shared/GlobalFunctions.js",
				"~/Scripts/Shared/UrlMapper.js",
				"~/Scripts/Helpers/AjaxHelper.js", 
				"~/Scripts/KnockoutExtensions/StopBinding.js",
				"~/Scripts/KnockoutExtensions/Show.js",
				"~/Scripts/Repositories/EntryReportRepository.js",
				"~/Scripts/Repositories/UserRepository.js",
				"~/Scripts/Models/SongVoteRating.js",               // Referred by UserRepository
				"~/Scripts/ViewModels/TopBarViewModel.js",
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js"
			));

			// Included on all pages except the front page (to optimize front page load time).
			bundles.Add(new ScriptBundle("~/bundles/shared/main").Include(
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/Helpers/HtmlHelper.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/Models/EntryType.js",
				"~/Scripts/Shared/EntryUrlMapper.js",
				"~/Scripts/Shared/EntryAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ConfirmClick.js",
				"~/Scripts/KnockoutExtensions/Dialog.js",
				"~/Scripts/KnockoutExtensions/EntryToolTip.js",
				"~/Scripts/KnockoutExtensions/jqButton.js",
				"~/Scripts/KnockoutExtensions/jqButtonset.js",
				"~/Scripts/KnockoutExtensions/Markdown.js",
				"~/Scripts/KnockoutExtensions/ToggleClick.js",
				"~/Scripts/KnockoutExtensions/Song/SongTypeLabel.js",
				"~/Scripts/KnockoutExtensions/Bootstrap/Tooltip.js",
				"~/Scripts/KnockoutExtensions/qTip.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/Models/Songs/SongType.js",
				"~/Scripts/Models/NameMatchMode.js",
				"~/Scripts/Models/Artists/ArtistType.js",
				"~/Scripts/Models/PVs/PVService.js",
				"~/Scripts/Models/PVServiceIcons.js",				
				"~/Scripts/Models/Globalization/ContentLanguagePreference.js",
				"~/Scripts/Models/EntryOptionalFields.js",
				"~/Scripts/Models/Artists/ArtistRoles.js",
				"~/Scripts/Models/ContentFocus.js",
				"~/Scripts/Helpers/ArtistHelper.js", // Depends on ArtistType, ArtistRoles
				"~/Scripts/Repositories/EntryCommentRepository.js",
				"~/Scripts/Repositories/BaseRepository.js",
				"~/Scripts/Repositories/RepositoryFactory.js",
				"~/Scripts/Repositories/AdminRepository.js",
				"~/Scripts/Repositories/SongRepository.js",
				"~/Scripts/Repositories/ArtistRepository.js",
				"~/Scripts/ViewModels/BasicEntryLinkViewModel.js",
				"~/Scripts/ViewModels/CommentViewModel.js",
				"~/Scripts/ViewModels/EditableCommentsViewModel.js",
				"~/Scripts/ViewModels/PagedItemsViewModel.js",
				"~/Scripts/ViewModels/ServerSidePagingViewModel.js",
				"~/Scripts/ViewModels/ReportEntryViewModel.js",
				"~/Scripts/ViewModels/Globalization/EnglishTranslatedStringViewModel.js",
				"~/Scripts/ViewModels/SelfDescriptionViewModel.js"
			));

			// Included on all entry edit and create pages (album, artist, my settings etc.)
			bundles.Add(new ScriptBundle("~/bundles/shared/edit").Include(
				"~/Scripts/knockout-sortable.js",
				"~/Scripts/Models/WebLinkCategory.js",
				"~/Scripts/Models/EntryStatus.js",
				"~/Scripts/Models/Globalization/ContentLanguageSelection.js",
				"~/Scripts/Shared/WebLinkMatcher.js",
				"~/Scripts/Shared/DialogService.js",
				"~/Scripts/Helpers/SearchTextQueryHelper.js",
				"~/Scripts/ViewModels/DeleteEntryViewModel.js",
				"~/Scripts/ViewModels/WebLinkEditViewModel.js",
				"~/Scripts/ViewModels/WebLinksEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/EnglishTranslatedStringEditViewModel.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/SongAutoComplete.js",
				"~/Scripts/KnockoutExtensions/FocusOut.js",
				"~/Scripts/KnockoutExtensions/InitialValue.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Home/Index").Include(
				"~/Scripts/jquery.tools.min.js",
				"~/Scripts/ViewModels/NewsListViewModel.js",
				"~/Scripts/Home/Index.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jqxRating").Include(
				"~/Scripts/jqwidgets27/jqxcore.js", "~/Scripts/jqwidgets27/jqxrating.js"));


			// VIEW-SPECIFIC BUNDLES
			bundles.Add(new ScriptBundle("~/bundles/ActivityEntry/Index").Include(
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/Models/ActivityEntries/EntryEditEvent.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/ActivityEntry/ActivityEntryListViewModel.js"
			));


			bundles.Add(new ScriptBundle("~/bundles/Album/Create").Include(
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/ViewModels/Album/AlbumCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Album/Details").Include(
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/Album/AlbumDetailsViewModel.js",
				"~/Scripts/Album/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Album/Edit").Include(
				"~/Scripts/KnockoutExtensions/ParseInteger.js",
				"~/Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/Models/Albums/AlbumType.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/Repositories/PVRepository.js",
				"~/Scripts/Repositories/ReleaseEventRepository.js",
				"~/Scripts/ViewModels/BasicListEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVListEditViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileEditViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileListEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
				"~/Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
				"~/Scripts/ViewModels/CustomNameEditViewModel.js",
				"~/Scripts/ViewModels/SongInAlbumEditViewModel.js",
				"~/Scripts/ViewModels/Album/AlbumDiscPropertiesEditViewModel.js",
                "~/Scripts/ViewModels/Album/AlbumEditViewModel.js",
				"~/Scripts/Album/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Album/Merge").Include(
				"~/Scripts/Helpers/EntryMergeValidationHelper.js",
				"~/Scripts/KnockoutExtensions/AlbumAutoComplete.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/ViewModels/Album/AlbumMergeViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Create").Include(
				"~/Scripts/ViewModels/ArtistCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Details").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/Models/Aggregate/TimeUnit.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/KnockoutExtensions/Highcharts.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerNico.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilter.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilters.js",
				"~/Scripts/ViewModels/Search/ArtistFilter.js",
				"~/Scripts/ViewModels/Search/ArtistFilters.js",
				"~/Scripts/ViewModels/Search/TagFilter.js",
				"~/Scripts/ViewModels/Search/TagFilters.js",
				"~/Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
				"~/Scripts/ViewModels/Search/AlbumSearchViewModel.js",
				"~/Scripts/ViewModels/Search/SongSearchViewModel.js",
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistDetailsViewModel.js",
				"~/Scripts/Artist/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Edit").Include(
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileEditViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileListEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistEditViewModel.js",
				"~/Scripts/Artist/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Merge").Include(
				"~/Scripts/Helpers/EntryMergeValidationHelper.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/ViewModels/Artist/ArtistMergeViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Discussion/Index").Include(
				"~/Scripts/page.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/Repositories/DiscussionRepository.js",
				"~/Scripts/ViewModels/Discussion/DiscussionTopicViewModel.js",
				"~/Scripts/ViewModels/Discussion/DiscussionIndexViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/EventSeries/Details").Include(
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/ReleaseEvent/EventSeriesDetailsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/EventSeries/Edit").Include(
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/Repositories/ReleaseEventRepository.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/ReleaseEvent/ReleaseEventSeriesEditViewModel.js",
				"~/Scripts/Event/SeriesEdit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/ReleaseEvent/Details").Include(
				"~/Scripts/Models/Users/UserEventRelationshipType.js",
				"~/Scripts/Repositories/CommentRepository.js",
				"~/Scripts/Repositories/ReleaseEventRepository.js",
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/ReleaseEvent/ReleaseEventDetailsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/ReleaseEvent/Edit").Include(
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
				"~/Scripts/KnockoutExtensions/ReleaseEventSeriesAutoComplete.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/SongListAutoComplete.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
				"~/Scripts/Repositories/PVRepository.js",
				"~/Scripts/Repositories/ReleaseEventRepository.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVListEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
				"~/Scripts/ViewModels/ReleaseEvent/ArtistForEventEditViewModel.js",
				"~/Scripts/ViewModels/ReleaseEvent/ReleaseEventEditViewModel.js",
				"~/Scripts/Event/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Search/Index").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/KnockoutExtensions/Artist/ArtistTypeLabel.js",
				"~/Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/Models/Tags/Tag.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/Repositories/EntryRepository.js",
				"~/Scripts/Repositories/ReleaseEventRepository.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerNico.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.js",
				"~/Scripts/ViewModels/Search/ArtistFilter.js",
				"~/Scripts/ViewModels/Search/ArtistFilters.js",
				"~/Scripts/ViewModels/Search/TagFilter.js",
				"~/Scripts/ViewModels/Search/TagFilters.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilter.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilters.js",
				"~/Scripts/ViewModels/Search/SearchViewModel.js",
				"~/Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
				"~/Scripts/ViewModels/Search/AnythingSearchViewModel.js",
				"~/Scripts/ViewModels/Search/ArtistSearchViewModel.js",
				"~/Scripts/ViewModels/Search/AlbumSearchViewModel.js",
				"~/Scripts/ViewModels/Search/EventSearchViewModel.js",
				"~/Scripts/ViewModels/Search/SongSearchViewModel.js",
				"~/Scripts/ViewModels/Search/TagSearchViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Create").Include(
				"~/Scripts/Helpers/SongHelper.js",
				"~/Scripts/ViewModels/SongCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Details").Include(
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/Song/SongDetailsViewModel.js",
				"~/Scripts/Song/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Edit").Include(
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
				"~/Scripts/Models/Globalization/TranslationType.js",
				"~/Scripts/Models/Tags/Tag.js",
				"~/Scripts/Models/PVs/PVType.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/Helpers/SongHelper.js",
				"~/Scripts/Repositories/PVRepository.js",
				"~/Scripts/ViewModels/BasicListEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
				"~/Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
				"~/Scripts/ViewModels/CustomNameEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVListEditViewModel.js",
				"~/Scripts/ViewModels/Song/LyricsForSongEditViewModel.js",
				"~/Scripts/ViewModels/Song/SongEditViewModel.js",
				"~/Scripts/Song/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Merge").Include(
				"~/Scripts/Helpers/EntryMergeValidationHelper.js",
				"~/Scripts/ViewModels/Song/SongMergeViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/TopRated").Include(
				"~/Scripts/url.js",
				"~/Scripts/Shared/Routing/ObservableUrlParamRouter.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/Song/RankingsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Details").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Repositories/SongListRepository.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/ViewModels/Search/ArtistFilter.js",
				"~/Scripts/ViewModels/Search/ArtistFilters.js",
				"~/Scripts/ViewModels/Search/TagFilter.js",
				"~/Scripts/ViewModels/Search/TagFilters.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilter.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilters.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerNico.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongListAdapter.js",
				"~/Scripts/ViewModels/SongList/SongListViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Edit").Include(
				"~/Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
				"~/Scripts/Repositories/SongListRepository.js",
				"~/Scripts/ViewModels/SongList/SongListEditViewModel.js",
				"~/Scripts/SongList/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Featured").Include(
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/Repositories/SongListRepository.js",
				"~/Scripts/ViewModels/SongList/SongListsBaseViewModel.js",
                "~/Scripts/ViewModels/SongList/FeaturedSongListsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Import").Include(
				"~/Scripts/ViewModels/SongList/ImportSongListViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Tag/Details").Include(
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/Tag/TagDetailsViewModel.js",
				"~/Scripts/Tag/Details.js"
			));
		
			bundles.Add(new ScriptBundle("~/bundles/Tag/Edit").Include(
				"~/Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/TagEditViewModel.js",
				"~/Scripts/Tag/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Tag/Index").Include(
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/Tag/TagCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Tag/Merge").Include(
				"~/Scripts/Helpers/EntryMergeValidationHelper.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/Tag/TagMergeViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/AlbumCollection").Include(
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilter.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilters.js",
				"~/Scripts/ViewModels/User/AlbumCollectionViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Details").Include(
				"~/Scripts/soundcloud-api.js",
				"~/Scripts/Models/Users/UserEventRelationshipType.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
				"~/Scripts/KnockoutExtensions/Highcharts.js",				
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/Helpers/HighchartsHelper.js",				
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerNico.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForRatedSongsAdapter.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/SongList/SongListsBaseViewModel.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilter.js",
				"~/Scripts/ViewModels/Search/AdvancedSearchFilters.js",
				"~/Scripts/ViewModels/Search/ArtistFilter.js",
				"~/Scripts/ViewModels/Search/ArtistFilters.js",
				"~/Scripts/ViewModels/Search/TagFilter.js",
				"~/Scripts/ViewModels/Search/TagFilters.js",
				"~/Scripts/ViewModels/User/FollowedArtistsViewModel.js",
				"~/Scripts/ViewModels/User/RatedSongsSearchViewModel.js",
				"~/Scripts/ViewModels/User/AlbumCollectionViewModel.js",
				"~/Scripts/ViewModels/User/UserDetailsViewModel.js",
				"~/Scripts/User/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Index").Include(
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/ViewModels/User/ListUsersViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Messages").Include(
				"~/Scripts/KnockoutExtensions/BindingHandlers/UserAutocomplete.js",
				"~/Scripts/ViewModels/User/UserMessagesViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/MySettings").Include(
				"~/Scripts/ViewModels/User/MySettingsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/RatedSongs").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerNico.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForRatedSongsAdapter.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/User/RatedSongsSearchViewModel.js"
			));


#if DEBUG
			bundles.Add(new ScriptBundle("~/bundles/tests")
				.Include("~/Scripts/moment.js")
				.IncludeDirectory("~/Scripts/Models", "*.js", true)
				.IncludeDirectory("~/Scripts/Helpers", "*.js")
				.IncludeDirectory("~/Scripts/Repositories", "*.js", true)
				.Include(
					"~/Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
					"~/Scripts/ViewModels/SongList/SongListsBaseViewModel.js"
				)
				.IncludeDirectory("~/Scripts/ViewModels", "*.js", true)
				.Include("~/Scripts/Shared/WebLinkMatcher.js")
				.Include("~/Scripts/Shared/Routing/ObservableUrlParamRouter.js")
				.IncludeDirectory("~/Scripts/Tests", "*.js", true)
			);
#endif


			// Base CSS
			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css", 
				"~/Content/bootstrap-responsive.css", 
				"~/Content/Site.css", 
				"~/Content/Styles/base.css", 
				//"~/Content/Styles/Snow2013.css",
				"~/Content/Styles/PVViewer_Black.css",
				"~/Content/Styles/ExtLinks.css", 
				"~/Content/Styles/Overrides.css",
				"~/Content/Styles/StyleOverrides.css",
				"~/Content/Styles/Search.css",
				"~/Content/Styles/song.css",
                "~/Content/Styles/userpage.css"
			));

			bundles.Add(new StyleBundle("~/Content/embedSong").Include(
				"~/Content/bootstrap.css", "~/Content/Styles/embedSong.css"));

			// CSS for jqxRating
			bundles.Add(new StyleBundle("~/Scripts/jqwidgets27/styles/css").Include(
				"~/Scripts/jqwidgets27/styles/jqx.base.css"));

		}

	}

}