const mix = require('laravel-mix');

/*
 |--------------------------------------------------------------------------
 | Mix Asset Management
 |--------------------------------------------------------------------------
 |
 | Mix provides a clean, fluent API for defining some Webpack build steps
 | for your Laravel application. By default, we are compiling the Sass
 | file for the application as well as bundling up all the JS files.
 |
 */

mix

	.scripts([
		"Scripts/jquery-2.2.1.js",
		"Scripts/bootstrap.js",
		//"Scripts/jquery-ui-1.10.1.js", // doesn't work if bundled together
		"Scripts/knockout-3.4.1.js",
		"Scripts/knockout.punches.min.js",
		"Scripts/lodash.js",
		"Scripts/qTip/jquery.qtip.js",
		"Scripts/marked.js"
	], "bundles/shared/libs.js")

	.scripts([
		"Scripts/jquery-ui-1.10.4.js"
	], "bundles/shared/jqui.js")

	// SHARED BUNDLES
	// Legacy common scripts - should be phased out
	.scripts(["Scripts/VocaDB.js"], "bundles/VocaDB.js")

	.ts("Scripts/App.ts", "bundles/app.js")

	// Included on all pages (including front page)
	// Generally the references go from viewmodels -> repositories -> models -> support classes
	.scripts([
		"Scripts/Shared/GlobalFunctions.js",	// HACK TODO remove
	], "bundles/shared/common.js")
	/*.scripts([
		"Scripts/Shared/TopBar.ts", 
		"Scripts/Shared/MessagesTyped.ts",
		"Scripts/Shared/GlobalFunctions.ts",
		"Scripts/Shared/UrlMapper.ts",
		"Scripts/Helpers/AjaxHelper.ts", 
		"Scripts/KnockoutExtensions/StopBinding.ts",
		"Scripts/KnockoutExtensions/Show.ts",
		"Scripts/Repositories/EntryReportRepository.ts",
		"Scripts/Repositories/UserRepository.ts",
		"Scripts/Models/SongVoteRating.ts",               // Referred by UserRepository
		"Scripts/ViewModels/TopBarViewModel.ts",
		"Scripts/ViewModels/PVRatingButtonsViewModel.ts"
	], "bundles/shared/common.js")*/

	// Included on all pages except the front page (to optimize front page load time).
	.scripts([
		"Scripts/moment-with-locales.js",
	], "bundles/shared/main.js")
	/*.scripts([
		"Scripts/moment-with-locales.js",
		"Scripts/Helpers/HtmlHelper.ts",
		"Scripts/Helpers/DateTimeHelper.ts",
		"Scripts/Models/EntryType.ts",
		"Scripts/Shared/EntryUrlMapper.ts",
		"Scripts/Shared/EntryAutoComplete.ts",
		"Scripts/KnockoutExtensions/ConfirmClick.ts",
		"Scripts/KnockoutExtensions/Dialog.ts",
		"Scripts/KnockoutExtensions/EntryToolTip.ts",
		"Scripts/KnockoutExtensions/jqButton.ts",
		"Scripts/KnockoutExtensions/jqButtonset.ts",
		"Scripts/KnockoutExtensions/Markdown.ts",
		"Scripts/KnockoutExtensions/ToggleClick.ts",
		"Scripts/KnockoutExtensions/Song/SongTypeLabel.ts",
		"Scripts/KnockoutExtensions/Bootstrap/Tooltip.ts",
		"Scripts/KnockoutExtensions/qTip.ts",
		"Scripts/KnockoutExtensions/TagAutoComplete.ts",
		"Scripts/KnockoutExtensions/Filters/Truncate.ts",
		"Scripts/Models/Songs/SongType.ts",
		"Scripts/Models/NameMatchMode.ts",
		"Scripts/Models/Artists/ArtistType.ts",
		"Scripts/Models/PVs/PVService.ts",
		"Scripts/Models/PVServiceIcons.ts",				
		"Scripts/Models/Globalization/ContentLanguagePreference.ts",
		"Scripts/Models/EntryOptionalFields.ts",
		"Scripts/Models/Artists/ArtistRoles.ts",
		"Scripts/Models/ContentFocus.ts",
		"Scripts/Helpers/ArtistHelper.ts", // Depends on ArtistType, ArtistRoles
		"Scripts/Repositories/EntryCommentRepository.ts",
		"Scripts/Repositories/BaseRepository.ts",
		"Scripts/Repositories/RepositoryFactory.ts",
		"Scripts/Repositories/AdminRepository.ts",
		"Scripts/Repositories/SongRepository.ts",
		"Scripts/Repositories/ArtistRepository.ts",
		"Scripts/ViewModels/BasicEntryLinkViewModel.ts",
		"Scripts/ViewModels/CommentViewModel.ts",
		"Scripts/ViewModels/EditableCommentsViewModel.ts",
		"Scripts/ViewModels/PagedItemsViewModel.ts",
		"Scripts/ViewModels/ServerSidePagingViewModel.ts",
		"Scripts/ViewModels/ReportEntryViewModel.ts",
		"Scripts/ViewModels/Globalization/EnglishTranslatedStringViewModel.ts",
		"Scripts/ViewModels/SelfDescriptionViewModel.ts"
	], "bundles/shared/main.js")*/

	// Included on all entry edit and create pages (album, artist, my settings etc.)
	.scripts([
		"Scripts/knockout-sortable.js"
	], "bundles/shared/edit.js")
	/*.scripts([
		"Scripts/knockout-sortable.js",
		"Scripts/Models/WebLinkCategory.ts",
		"Scripts/Models/EntryStatus.ts",
		"Scripts/Models/Globalization/ContentLanguageSelection.ts",
		"Scripts/Models/Albums/AlbumType.ts",
		"Scripts/Models/Events/EventCategory.ts",
		"Scripts/Shared/WebLinkMatcher.ts",
		"Scripts/Shared/DialogService.ts",
		"Scripts/Helpers/SearchTextQueryHelper.ts",
		"Scripts/ViewModels/DeleteEntryViewModel.ts",
		"Scripts/ViewModels/WebLinkEditViewModel.ts",
		"Scripts/ViewModels/WebLinksEditViewModel.ts",
		"Scripts/ViewModels/Globalization/EnglishTranslatedStringEditViewModel.ts",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.ts",
		"Scripts/KnockoutExtensions/SongAutoComplete.ts",
		"Scripts/KnockoutExtensions/FocusOut.ts",
		"Scripts/KnockoutExtensions/InitialValue.ts"
	], "bundles/shared/edit.js")*/

	/*.scripts([
		"Scripts/jquery.tools.min.js",
		"Scripts/ViewModels/NewsListViewModel.ts",
		"Scripts/Home/Index.ts"
	], "bundles/Home/Index.js")*/

	.scripts([
		"Scripts/jqwidgets27/jqxcore.js", "Scripts/jqwidgets27/jqxrating.js"
	], "bundles/jqxRating.js")


	// VIEW-SPECIFIC BUNDLES
	/*.scripts([
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.ts",
		"Scripts/Models/ActivityEntries/EntryEditEvent.ts",
		"Scripts/Models/ResourcesManager.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/ViewModels/ActivityEntry/ActivityEntryListViewModel.ts"
	], "bundles/ActivityEntry/Index.js")*/


	/*.scripts([
		"Scripts/Repositories/AlbumRepository.ts",
		"Scripts/ViewModels/Album/AlbumCreateViewModel.ts"
	], "bundles/Album/Create.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.ts",
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/Repositories/AlbumRepository.ts",
		"Scripts/ViewModels/Tag/TagListViewModel.ts",
		"Scripts/ViewModels/Tag/TagsEditViewModel.ts",
		"Scripts/ViewModels/Album/AlbumDetailsViewModel.ts",
		"Scripts/Album/Details.ts"
	], "bundles/Album/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/ParseInteger.ts",
		"Scripts/KnockoutExtensions/FormatLengthSecondsFilter.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.ts",
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/Helpers/KnockoutHelper.ts",
		"Scripts/Repositories/AlbumRepository.ts",
		"Scripts/Repositories/PVRepository.ts",
		"Scripts/Repositories/ReleaseEventRepository.ts",
		"Scripts/ViewModels/BasicListEditViewModel.ts",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.ts",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.ts",
		"Scripts/ViewModels/PVs/PVEditViewModel.ts",
		"Scripts/ViewModels/PVs/PVListEditViewModel.ts",
		"Scripts/ViewModels/EntryPictureFileEditViewModel.ts",
		"Scripts/ViewModels/EntryPictureFileListEditViewModel.ts",
		"Scripts/ViewModels/Artist/ArtistRolesEditViewModel.ts",
		"Scripts/ViewModels/ArtistForAlbumEditViewModel.ts",
		"Scripts/ViewModels/CustomNameEditViewModel.ts",
		"Scripts/ViewModels/SongInAlbumEditViewModel.ts",
		"Scripts/ViewModels/Album/AlbumDiscPropertiesEditViewModel.ts",
		"Scripts/ViewModels/Album/AlbumEditViewModel.ts",
		"Scripts/Album/Edit.js"
	], "bundles/Album/Edit.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.ts",
		"Scripts/KnockoutExtensions/AlbumAutoComplete.ts",
		"Scripts/Repositories/AlbumRepository.ts",
		"Scripts/ViewModels/Album/AlbumMergeViewModel.ts"
	], "bundles/Album/Merge.js")*/

	/*.scripts([
		"Scripts/ViewModels/ArtistCreateViewModel.ts"
	], "bundles/Artist/Create.js")*/

	.scripts([
		"Scripts/soundcloud-api.js"	// REVIEW
	], "bundles/Artist/Details.js")
	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/Repositories/AlbumRepository.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/Models/ResourcesManager.ts",
		"Scripts/Models/Aggregate/TimeUnit.ts",
		"Scripts/Helpers/PVHelper.ts",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.ts",
		"Scripts/KnockoutExtensions/SlideVisible.ts",				
		"Scripts/KnockoutExtensions/ScrollEnd.ts",
		"Scripts/KnockoutExtensions/Highcharts.ts",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.ts",
		"Scripts/ViewModels/PVs/PVPlayersFactory.ts",
		"Scripts/ViewModels/PVs/PVPlayerFile.ts",
		"Scripts/ViewModels/PVs/PVPlayerNico.ts",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.ts",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.ts",
		"Scripts/ViewModels/Search/ArtistFilter.ts",
		"Scripts/ViewModels/Search/ArtistFilters.ts",
		"Scripts/ViewModels/Search/TagFilter.ts",
		"Scripts/ViewModels/Search/TagFilters.ts",
		"Scripts/ViewModels/Search/SearchCategoryBaseViewModel.ts",
		"Scripts/ViewModels/Search/AlbumSearchViewModel.ts",
		"Scripts/ViewModels/Search/SongSearchViewModel.ts",
		"Scripts/ViewModels/Tag/TagListViewModel.ts",
		"Scripts/ViewModels/Tag/TagsEditViewModel.ts",
		"Scripts/ViewModels/Artist/ArtistDetailsViewModel.ts",
		"Scripts/Artist/Details.ts"
	], "bundles/Artist/Details.js")*/

	/*.scripts([
		"Scripts/Helpers/KnockoutHelper.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.ts",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.ts",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.ts",
		"Scripts/ViewModels/EntryPictureFileEditViewModel.ts",
		"Scripts/ViewModels/EntryPictureFileListEditViewModel.ts",
		"Scripts/ViewModels/Artist/ArtistEditViewModel.ts",
		"Scripts/Artist/Edit.js"
	], "bundles/Artist/Edit.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.ts",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.ts",
		"Scripts/ViewModels/Artist/ArtistMergeViewModel.ts"
	], "bundles/Artist/Merge.js")*/

	.scripts([
		"Scripts/page.js"
	], "bundles/Discussion/Index.js")
	/*.scripts([
		"Scripts/page.js",
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.ts",
		"Scripts/Repositories/DiscussionRepository.ts",
		"Scripts/ViewModels/Discussion/DiscussionTopicViewModel.ts",
		"Scripts/ViewModels/Discussion/DiscussionIndexViewModel.ts"
	], "bundles/Discussion/Index.js")*/

	/*.scripts([
		"Scripts/ViewModels/Tag/TagListViewModel.ts",
		"Scripts/ViewModels/Tag/TagsEditViewModel.ts",
		"Scripts/ViewModels/ReleaseEvent/EventSeriesDetailsViewModel.ts"
	], "bundles/EventSeries/Details.js")*/

	/*.scripts([
		"Scripts/Helpers/KnockoutHelper.ts",
		"Scripts/Repositories/ReleaseEventRepository.ts",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.ts",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.ts",
		"Scripts/ViewModels/ReleaseEvent/ReleaseEventSeriesEditViewModel.ts",
		"Scripts/Event/SeriesEdit.js"
	], "bundles/EventSeries/Edit.js")*/

	/*.scripts([
		"Scripts/Models/Users/UserEventRelationshipType.ts",
		"Scripts/Repositories/CommentRepository.ts",
		"Scripts/Repositories/ReleaseEventRepository.ts",
		"Scripts/ViewModels/Tag/TagListViewModel.ts",
		"Scripts/ViewModels/Tag/TagsEditViewModel.ts",
		"Scripts/ViewModels/ReleaseEvent/ReleaseEventDetailsViewModel.ts"
	], "bundles/ReleaseEvent/Details.js")*/

	/*.scripts([
		"Scripts/Helpers/KnockoutHelper.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.ts",
		"Scripts/KnockoutExtensions/ReleaseEventSeriesAutoComplete.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/SongListAutoComplete.ts",
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/KnockoutExtensions/FormatLengthSecondsFilter.ts",
		"Scripts/Repositories/PVRepository.ts",
		"Scripts/Repositories/ReleaseEventRepository.ts",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.ts",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.ts",
		"Scripts/ViewModels/PVs/PVEditViewModel.ts",
		"Scripts/ViewModels/PVs/PVListEditViewModel.ts",
		"Scripts/ViewModels/Artist/ArtistRolesEditViewModel.ts",
		"Scripts/ViewModels/ReleaseEvent/ArtistForEventEditViewModel.ts",
		"Scripts/ViewModels/ReleaseEvent/ReleaseEventEditViewModel.ts",
		"Scripts/Event/Edit.js"
	], "bundles/ReleaseEvent/Edit.js")*/

	.scripts([
		"Scripts/soundcloud-api.js"	// REVIEW
	], "bundles/Search/Index.js")
	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/KnockoutExtensions/Artist/ArtistTypeLabel.ts",
		"Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.ts",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.ts",
		"Scripts/KnockoutExtensions/SlideVisible.ts",				
		"Scripts/KnockoutExtensions/ScrollEnd.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.ts",
		"Scripts/KnockoutExtensions/SongAutoComplete.ts",
		"Scripts/Models/ResourcesManager.ts",
		"Scripts/Models/Tags/Tag.ts",
		"Scripts/Helpers/PVHelper.ts",
		"Scripts/Helpers/SearchTextQueryHelper.ts",
		"Scripts/Repositories/AlbumRepository.ts",
		"Scripts/Repositories/EntryRepository.ts",
		"Scripts/Repositories/ReleaseEventRepository.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/Repositories/TagRepository.ts",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.ts",
		"Scripts/ViewModels/PVs/PVPlayersFactory.ts",
		"Scripts/ViewModels/PVs/PVPlayerFile.ts",
		"Scripts/ViewModels/PVs/PVPlayerNico.ts",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.ts",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.ts",
		"Scripts/ViewModels/Search/ArtistFilter.ts",
		"Scripts/ViewModels/Search/ArtistFilters.ts",
		"Scripts/ViewModels/Search/TagFilter.ts",
		"Scripts/ViewModels/Search/TagFilters.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.ts",
		"Scripts/ViewModels/Search/SearchViewModel.ts",
		"Scripts/ViewModels/Search/SearchCategoryBaseViewModel.ts",
		"Scripts/ViewModels/Search/AnythingSearchViewModel.ts",
		"Scripts/ViewModels/Search/ArtistSearchViewModel.ts",
		"Scripts/ViewModels/Search/AlbumSearchViewModel.ts",
		"Scripts/ViewModels/Search/EventSearchViewModel.ts",
		"Scripts/ViewModels/Search/SongSearchViewModel.ts",
		"Scripts/ViewModels/Search/TagSearchViewModel.ts"
	], "bundles/Search/Index.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/Artist/ArtistTypeLabel.ts",
		"Scripts/Helpers/SongHelper.ts",
		"Scripts/ViewModels/SongCreateViewModel.ts"
	], "bundles/Song/Create.js")*/

	.scripts([
		"Scripts/MediaElement/mediaelement-and-player.min.js",
	], "bundles/Song/Details.js")
	/*.scripts([
		"Scripts/MediaElement/mediaelement-and-player.min.js",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.ts",
		"Scripts/ViewModels/Tag/TagListViewModel.ts",
		"Scripts/ViewModels/Tag/TagsEditViewModel.ts",
		"Scripts/ViewModels/Song/SongDetailsViewModel.ts",
		"Scripts/Song/Details.js"
	], "bundles/Song/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/KnockoutExtensions/FormatLengthSecondsFilter.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.ts",
		"Scripts/Models/Globalization/TranslationType.ts",
		"Scripts/Models/Tags/Tag.ts",
		"Scripts/Models/PVs/PVType.ts",
		"Scripts/Helpers/KnockoutHelper.ts",
		"Scripts/Helpers/SongHelper.ts",
		"Scripts/Repositories/PVRepository.ts",
		"Scripts/ViewModels/BasicListEditViewModel.ts",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.ts",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.ts",
		"Scripts/ViewModels/Artist/ArtistRolesEditViewModel.ts",
		"Scripts/ViewModels/ArtistForAlbumEditViewModel.ts",
		"Scripts/ViewModels/CustomNameEditViewModel.ts",
		"Scripts/ViewModels/PVs/PVEditViewModel.ts",
		"Scripts/ViewModels/PVs/PVListEditViewModel.ts",
		"Scripts/ViewModels/Song/LyricsForSongEditViewModel.ts",
		"Scripts/ViewModels/Song/SongEditViewModel.ts",
		"Scripts/Song/Edit.js"
	], "bundles/Song/Edit.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.ts",
		"Scripts/ViewModels/Song/SongMergeViewModel.ts"
	], "bundles/Song/Merge.js")*/

	.scripts([
		"Scripts/url.js"
	], "bundles/Song/TopRated.js")
	/*.scripts([
		"Scripts/url.js",
		"Scripts/Shared/Routing/ObservableUrlParamRouter.ts",
		"Scripts/KnockoutExtensions/SlideVisible.ts",				
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.ts",
		"Scripts/ViewModels/Song/RankingsViewModel.ts"
	], "bundles/Song/TopRated.js")*/

	.scripts([
		"Scripts/soundcloud-api.js"	// REVIEW
	], "bundles/SongList/Details.js")
	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/KnockoutExtensions/SlideVisible.ts",				
		"Scripts/KnockoutExtensions/ScrollEnd.ts",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.ts",
		"Scripts/Helpers/PVHelper.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/Repositories/SongListRepository.ts",
		"Scripts/Models/ResourcesManager.ts",
		"Scripts/ViewModels/Search/ArtistFilter.ts",
		"Scripts/ViewModels/Search/ArtistFilters.ts",
		"Scripts/ViewModels/Search/TagFilter.ts",
		"Scripts/ViewModels/Search/TagFilters.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.ts",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.ts",
		"Scripts/ViewModels/PVs/PVPlayersFactory.ts",
		"Scripts/ViewModels/PVs/PVPlayerFile.ts",
		"Scripts/ViewModels/PVs/PVPlayerNico.ts",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.ts",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongListAdapter.ts",
		"Scripts/ViewModels/Tag/TagListViewModel.ts",
		"Scripts/ViewModels/Tag/TagsEditViewModel.ts",
		"Scripts/ViewModels/SongList/SongListViewModel.ts"
	], "bundles/SongList/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.ts",
		"Scripts/Repositories/SongListRepository.ts",
		"Scripts/ViewModels/SongList/SongListEditViewModel.ts",
		"Scripts/SongList/Edit.js"
	], "bundles/SongList/Edit.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/Repositories/SongListRepository.ts",
		"Scripts/Repositories/TagRepository.ts",
		"Scripts/ViewModels/Search/TagFilter.ts",
		"Scripts/ViewModels/Search/TagFilters.ts",
		"Scripts/ViewModels/SongList/SongListsBaseViewModel.ts",
		"Scripts/ViewModels/SongList/FeaturedSongListsViewModel.ts"
	], "bundles/SongList/Featured.js")*/

	/*.scripts([
		"Scripts/ViewModels/SongList/ImportSongListViewModel.ts"
	], "bundles/SongList/Import.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.ts",
		"Scripts/Repositories/TagRepository.ts",
		"Scripts/ViewModels/Tag/TagDetailsViewModel.ts",
		"Scripts/Tag/Details.ts"
	], "bundles/Tag/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.ts",
		"Scripts/Repositories/TagRepository.ts",
		"Scripts/Helpers/KnockoutHelper.ts",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.ts",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.ts",
		"Scripts/ViewModels/TagEditViewModel.ts",
		"Scripts/Tag/Edit.js"
	], "bundles/Tag/Edit.js")*/

	/*.scripts([
		"Scripts/Repositories/TagRepository.ts",
		"Scripts/ViewModels/Tag/TagCreateViewModel.ts"
	], "bundles/Tag/Index.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.ts",
		"Scripts/Repositories/TagRepository.ts",
		"Scripts/ViewModels/Tag/TagMergeViewModel.ts"
	], "bundles/Tag/Merge.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/ArtistAutoComplete.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.ts",
		"Scripts/ViewModels/User/AlbumCollectionViewModel.ts"
	], "bundles/User/AlbumCollection.js")*/

	.scripts([
		"Scripts/soundcloud-api.js"	// REVIEW
	], "bundles/User/Details.js")
	/*.scripts([
		"Scripts/soundcloud-api.js",
		"Scripts/Models/Users/UserEventRelationshipType.ts",
		"Scripts/KnockoutExtensions/SlideVisible.ts",				
		"Scripts/KnockoutExtensions/ArtistAutoComplete.ts",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.ts",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.ts",
		"Scripts/KnockoutExtensions/Highcharts.ts",				
		"Scripts/KnockoutExtensions/ScrollEnd.ts",
		"Scripts/KnockoutExtensions/FormatDateFilter.ts",
		"Scripts/Helpers/HighchartsHelper.ts",				
		"Scripts/Helpers/PVHelper.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/Repositories/TagRepository.ts",
		"Scripts/ViewModels/DeleteEntryViewModel.ts",
		"Scripts/ViewModels/PVs/PVPlayersFactory.ts",
		"Scripts/ViewModels/PVs/PVPlayerFile.ts",
		"Scripts/ViewModels/PVs/PVPlayerNico.ts",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.ts",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForRatedSongsAdapter.ts",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.ts",
		"Scripts/ViewModels/SongList/SongListsBaseViewModel.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.ts",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.ts",
		"Scripts/ViewModels/Search/ArtistFilter.ts",
		"Scripts/ViewModels/Search/ArtistFilters.ts",
		"Scripts/ViewModels/Search/TagFilter.ts",
		"Scripts/ViewModels/Search/TagFilters.ts",
		"Scripts/ViewModels/User/FollowedArtistsViewModel.ts",
		"Scripts/ViewModels/User/RatedSongsSearchViewModel.ts",
		"Scripts/ViewModels/User/AlbumCollectionViewModel.ts",
		"Scripts/ViewModels/User/UserDetailsViewModel.ts",
		"Scripts/User/Details.ts"
	], "bundles/User/Details.js")*/

	/*.scripts([
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/Models/ResourcesManager.ts",
		"Scripts/ViewModels/User/ListUsersViewModel.ts"
	], "bundles/User/Index.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/BindingHandlers/UserAutocomplete.ts",
		"Scripts/ViewModels/User/UserMessagesViewModel.ts"
	], "bundles/User/Messages.js")*/

	/*.scripts([
		"Scripts/ViewModels/User/MySettingsViewModel.ts"
	], "bundles/User/MySettings.js")*/

	.scripts([
		"Scripts/soundcloud-api.js"	// REVIEW
	], "bundles/User/RatedSongs.js")
	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/KnockoutExtensions/SlideVisible.ts",				
		"Scripts/KnockoutExtensions/ArtistAutoComplete.ts",
		"Scripts/KnockoutExtensions/ScrollEnd.ts",
		"Scripts/Helpers/PVHelper.ts",
		"Scripts/Repositories/ResourceRepository.ts",
		"Scripts/ViewModels/PVs/PVPlayersFactory.ts",
		"Scripts/ViewModels/PVs/PVPlayerFile.ts",
		"Scripts/ViewModels/PVs/PVPlayerNico.ts",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.ts",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.ts",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.ts",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForRatedSongsAdapter.ts",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.ts",
		"Scripts/ViewModels/User/RatedSongsSearchViewModel.ts"
	], "bundles/User/RatedSongs.js")*/


	// TODO


	// Base CSS
	.styles([
		"Content/bootstrap.css",
		"Content/bootstrap-responsive.css",
		"Content/Site.css",
		"Content/Styles/base.css",
		//"Content/Styles/Snow2013.css",
		"Content/Styles/PVViewer_Black.css",
		"Content/Styles/ExtLinks.css",
		"Content/Styles/Overrides.css",
		"Content/Styles/StyleOverrides.css",
		"Content/Styles/Search.css",
		"Content/Styles/song.css",
		"Content/Styles/userpage.css"
	], "Content/css.css")

	.styles([
		"Content/bootstrap.css", "Content/Styles/embedSong.css"], "Content/embedSong.css")

	// CSS for jqxRating
	.styles([
		"Scripts/jqwidgets27/styles/jqx.base.css"], "Scripts/jqwidgets27/styles/css.css");

mix.webpackConfig({
	output: {
		library: 'app'
	}
});
