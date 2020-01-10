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

	// Included on all pages (including front page)
	// Generally the references go from viewmodels -> repositories -> models -> support classes
	/*.scripts([
		"Scripts/Shared/TopBar.js", 
		"Scripts/Shared/MessagesTyped.js",
		"Scripts/Shared/GlobalFunctions.js",
		"Scripts/Shared/UrlMapper.js",
		"Scripts/Helpers/AjaxHelper.js", 
		"Scripts/KnockoutExtensions/StopBinding.js",
		"Scripts/KnockoutExtensions/Show.js",
		"Scripts/Repositories/EntryReportRepository.js",
		"Scripts/Repositories/UserRepository.js",
		"Scripts/Models/SongVoteRating.js",               // Referred by UserRepository
		"Scripts/ViewModels/TopBarViewModel.js",
		"Scripts/ViewModels/PVRatingButtonsViewModel.js"
	], "bundles/shared/common.js")*/

	// Included on all pages except the front page (to optimize front page load time).
	/*.scripts([
		"Scripts/moment-with-locales.js",
		"Scripts/Helpers/HtmlHelper.js",
		"Scripts/Helpers/DateTimeHelper.js",
		"Scripts/Models/EntryType.js",
		"Scripts/Shared/EntryUrlMapper.js",
		"Scripts/Shared/EntryAutoComplete.js",
		"Scripts/KnockoutExtensions/ConfirmClick.js",
		"Scripts/KnockoutExtensions/Dialog.js",
		"Scripts/KnockoutExtensions/EntryToolTip.js",
		"Scripts/KnockoutExtensions/jqButton.js",
		"Scripts/KnockoutExtensions/jqButtonset.js",
		"Scripts/KnockoutExtensions/Markdown.js",
		"Scripts/KnockoutExtensions/ToggleClick.js",
		"Scripts/KnockoutExtensions/Song/SongTypeLabel.js",
		"Scripts/KnockoutExtensions/Bootstrap/Tooltip.js",
		"Scripts/KnockoutExtensions/qTip.js",
		"Scripts/KnockoutExtensions/TagAutoComplete.js",
		"Scripts/KnockoutExtensions/Filters/Truncate.js",
		"Scripts/Models/Songs/SongType.js",
		"Scripts/Models/NameMatchMode.js",
		"Scripts/Models/Artists/ArtistType.js",
		"Scripts/Models/PVs/PVService.js",
		"Scripts/Models/PVServiceIcons.js",				
		"Scripts/Models/Globalization/ContentLanguagePreference.js",
		"Scripts/Models/EntryOptionalFields.js",
		"Scripts/Models/Artists/ArtistRoles.js",
		"Scripts/Models/ContentFocus.js",
		"Scripts/Helpers/ArtistHelper.js", // Depends on ArtistType, ArtistRoles
		"Scripts/Repositories/EntryCommentRepository.js",
		"Scripts/Repositories/BaseRepository.js",
		"Scripts/Repositories/RepositoryFactory.js",
		"Scripts/Repositories/AdminRepository.js",
		"Scripts/Repositories/SongRepository.js",
		"Scripts/Repositories/ArtistRepository.js",
		"Scripts/ViewModels/BasicEntryLinkViewModel.js",
		"Scripts/ViewModels/CommentViewModel.js",
		"Scripts/ViewModels/EditableCommentsViewModel.js",
		"Scripts/ViewModels/PagedItemsViewModel.js",
		"Scripts/ViewModels/ServerSidePagingViewModel.js",
		"Scripts/ViewModels/ReportEntryViewModel.js",
		"Scripts/ViewModels/Globalization/EnglishTranslatedStringViewModel.js",
		"Scripts/ViewModels/SelfDescriptionViewModel.js"
	], "bundles/shared/main.js")*/

	// Included on all entry edit and create pages (album, artist, my settings etc.)
	/*.scripts([
		"Scripts/knockout-sortable.js",
		"Scripts/Models/WebLinkCategory.js",
		"Scripts/Models/EntryStatus.js",
		"Scripts/Models/Globalization/ContentLanguageSelection.js",
		"Scripts/Models/Albums/AlbumType.js",
		"Scripts/Models/Events/EventCategory.js",
		"Scripts/Shared/WebLinkMatcher.js",
		"Scripts/Shared/DialogService.js",
		"Scripts/Helpers/SearchTextQueryHelper.js",
		"Scripts/ViewModels/DeleteEntryViewModel.js",
		"Scripts/ViewModels/WebLinkEditViewModel.js",
		"Scripts/ViewModels/WebLinksEditViewModel.js",
		"Scripts/ViewModels/Globalization/EnglishTranslatedStringEditViewModel.js",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.js",
		"Scripts/KnockoutExtensions/SongAutoComplete.js",
		"Scripts/KnockoutExtensions/FocusOut.js",
		"Scripts/KnockoutExtensions/InitialValue.js"
	], "bundles/shared/edit.js")*/

	/*.scripts([
		"Scripts/jquery.tools.min.js",
		"Scripts/ViewModels/NewsListViewModel.js",
		"Scripts/Home/Index.js"
	], "bundles/Home/Index.js")*/

	.scripts([
		"Scripts/jqwidgets27/jqxcore.js", "Scripts/jqwidgets27/jqxrating.js"
	], "bundles/jqxRating.js")


	// VIEW-SPECIFIC BUNDLES
	/*.scripts([
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
		"Scripts/Models/ActivityEntries/EntryEditEvent.js",
		"Scripts/Models/ResourcesManager.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/ViewModels/ActivityEntry/ActivityEntryListViewModel.js"
	], "bundles/ActivityEntry/Index.js")*/


	/*.scripts([
		"Scripts/Repositories/AlbumRepository.js",
		"Scripts/ViewModels/Album/AlbumCreateViewModel.js"
	], "bundles/Album/Create.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/Repositories/AlbumRepository.js",
		"Scripts/ViewModels/Tag/TagListViewModel.js",
		"Scripts/ViewModels/Tag/TagsEditViewModel.js",
		"Scripts/ViewModels/Album/AlbumDetailsViewModel.js",
		"Scripts/Album/Details.js"
	], "bundles/Album/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/ParseInteger.js",
		"Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/Helpers/KnockoutHelper.js",
		"Scripts/Repositories/AlbumRepository.js",
		"Scripts/Repositories/PVRepository.js",
		"Scripts/Repositories/ReleaseEventRepository.js",
		"Scripts/ViewModels/BasicListEditViewModel.js",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.js",
		"Scripts/ViewModels/PVs/PVEditViewModel.js",
		"Scripts/ViewModels/PVs/PVListEditViewModel.js",
		"Scripts/ViewModels/EntryPictureFileEditViewModel.js",
		"Scripts/ViewModels/EntryPictureFileListEditViewModel.js",
		"Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
		"Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
		"Scripts/ViewModels/CustomNameEditViewModel.js",
		"Scripts/ViewModels/SongInAlbumEditViewModel.js",
		"Scripts/ViewModels/Album/AlbumDiscPropertiesEditViewModel.js",
		"Scripts/ViewModels/Album/AlbumEditViewModel.js",
		"Scripts/Album/Edit.js"
	], "bundles/Album/Edit.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.js",
		"Scripts/KnockoutExtensions/AlbumAutoComplete.js",
		"Scripts/Repositories/AlbumRepository.js",
		"Scripts/ViewModels/Album/AlbumMergeViewModel.js"
	], "bundles/Album/Merge.js")*/

	/*.scripts([
		"Scripts/ViewModels/ArtistCreateViewModel.js"
	], "bundles/Artist/Create.js")*/

	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/Repositories/AlbumRepository.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/Models/ResourcesManager.js",
		"Scripts/Models/Aggregate/TimeUnit.js",
		"Scripts/Helpers/PVHelper.js",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
		"Scripts/KnockoutExtensions/SlideVisible.js",				
		"Scripts/KnockoutExtensions/ScrollEnd.js",
		"Scripts/KnockoutExtensions/Highcharts.js",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
		"Scripts/ViewModels/PVs/PVPlayersFactory.js",
		"Scripts/ViewModels/PVs/PVPlayerFile.js",
		"Scripts/ViewModels/PVs/PVPlayerNico.js",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.js",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.js",
		"Scripts/ViewModels/Search/ArtistFilter.js",
		"Scripts/ViewModels/Search/ArtistFilters.js",
		"Scripts/ViewModels/Search/TagFilter.js",
		"Scripts/ViewModels/Search/TagFilters.js",
		"Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
		"Scripts/ViewModels/Search/AlbumSearchViewModel.js",
		"Scripts/ViewModels/Search/SongSearchViewModel.js",
		"Scripts/ViewModels/Tag/TagListViewModel.js",
		"Scripts/ViewModels/Tag/TagsEditViewModel.js",
		"Scripts/ViewModels/Artist/ArtistDetailsViewModel.js",
		"Scripts/Artist/Details.js"
	], "bundles/Artist/Details.js")*/

	/*.scripts([
		"Scripts/Helpers/KnockoutHelper.js",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.js",
		"Scripts/ViewModels/EntryPictureFileEditViewModel.js",
		"Scripts/ViewModels/EntryPictureFileListEditViewModel.js",
		"Scripts/ViewModels/Artist/ArtistEditViewModel.js",
		"Scripts/Artist/Edit.js"
	], "bundles/Artist/Edit.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.js",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.js",
		"Scripts/ViewModels/Artist/ArtistMergeViewModel.js"
	], "bundles/Artist/Merge.js")*/

	/*.scripts([
		"Scripts/page.js",
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
		"Scripts/Repositories/DiscussionRepository.js",
		"Scripts/ViewModels/Discussion/DiscussionTopicViewModel.js",
		"Scripts/ViewModels/Discussion/DiscussionIndexViewModel.js"
	], "bundles/Discussion/Index.js")*/

	/*.scripts([
		"Scripts/ViewModels/Tag/TagListViewModel.js",
		"Scripts/ViewModels/Tag/TagsEditViewModel.js",
		"Scripts/ViewModels/ReleaseEvent/EventSeriesDetailsViewModel.js"
	], "bundles/EventSeries/Details.js")*/

	/*.scripts([
		"Scripts/Helpers/KnockoutHelper.js",
		"Scripts/Repositories/ReleaseEventRepository.js",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.js",
		"Scripts/ViewModels/ReleaseEvent/ReleaseEventSeriesEditViewModel.js",
		"Scripts/Event/SeriesEdit.js"
	], "bundles/EventSeries/Edit.js")*/

	/*.scripts([
		"Scripts/Models/Users/UserEventRelationshipType.js",
		"Scripts/Repositories/CommentRepository.js",
		"Scripts/Repositories/ReleaseEventRepository.js",
		"Scripts/ViewModels/Tag/TagListViewModel.js",
		"Scripts/ViewModels/Tag/TagsEditViewModel.js",
		"Scripts/ViewModels/ReleaseEvent/ReleaseEventDetailsViewModel.js"
	], "bundles/ReleaseEvent/Details.js")*/

	/*.scripts([
		"Scripts/Helpers/KnockoutHelper.js",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
		"Scripts/KnockoutExtensions/ReleaseEventSeriesAutoComplete.js",
		"Scripts/KnockoutExtensions/BindingHandlers/SongListAutoComplete.js",
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
		"Scripts/Repositories/PVRepository.js",
		"Scripts/Repositories/ReleaseEventRepository.js",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.js",
		"Scripts/ViewModels/PVs/PVEditViewModel.js",
		"Scripts/ViewModels/PVs/PVListEditViewModel.js",
		"Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
		"Scripts/ViewModels/ReleaseEvent/ArtistForEventEditViewModel.js",
		"Scripts/ViewModels/ReleaseEvent/ReleaseEventEditViewModel.js",
		"Scripts/Event/Edit.js"
	], "bundles/ReleaseEvent/Edit.js")*/

	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/KnockoutExtensions/Artist/ArtistTypeLabel.js",
		"Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.js",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.js",
		"Scripts/KnockoutExtensions/SlideVisible.js",				
		"Scripts/KnockoutExtensions/ScrollEnd.js",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
		"Scripts/KnockoutExtensions/SongAutoComplete.js",
		"Scripts/Models/ResourcesManager.js",
		"Scripts/Models/Tags/Tag.js",
		"Scripts/Helpers/PVHelper.js",
		"Scripts/Helpers/SearchTextQueryHelper.js",
		"Scripts/Repositories/AlbumRepository.js",
		"Scripts/Repositories/EntryRepository.js",
		"Scripts/Repositories/ReleaseEventRepository.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/Repositories/TagRepository.js",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
		"Scripts/ViewModels/PVs/PVPlayersFactory.js",
		"Scripts/ViewModels/PVs/PVPlayerFile.js",
		"Scripts/ViewModels/PVs/PVPlayerNico.js",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.js",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.js",
		"Scripts/ViewModels/Search/ArtistFilter.js",
		"Scripts/ViewModels/Search/ArtistFilters.js",
		"Scripts/ViewModels/Search/TagFilter.js",
		"Scripts/ViewModels/Search/TagFilters.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.js",
		"Scripts/ViewModels/Search/SearchViewModel.js",
		"Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
		"Scripts/ViewModels/Search/AnythingSearchViewModel.js",
		"Scripts/ViewModels/Search/ArtistSearchViewModel.js",
		"Scripts/ViewModels/Search/AlbumSearchViewModel.js",
		"Scripts/ViewModels/Search/EventSearchViewModel.js",
		"Scripts/ViewModels/Search/SongSearchViewModel.js",
		"Scripts/ViewModels/Search/TagSearchViewModel.js"
	], "bundles/Search/Index.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/Artist/ArtistTypeLabel.js",
		"Scripts/Helpers/SongHelper.js",
		"Scripts/ViewModels/SongCreateViewModel.js"
	], "bundles/Song/Create.js")*/

	/*.scripts([
		"Scripts/MediaElement/mediaelement-and-player.min.js",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
		"Scripts/ViewModels/Tag/TagListViewModel.js",
		"Scripts/ViewModels/Tag/TagsEditViewModel.js",
		"Scripts/ViewModels/Song/SongDetailsViewModel.js",
		"Scripts/Song/Details.js"
	], "bundles/Song/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
		"Scripts/Models/Globalization/TranslationType.js",
		"Scripts/Models/Tags/Tag.js",
		"Scripts/Models/PVs/PVType.js",
		"Scripts/Helpers/KnockoutHelper.js",
		"Scripts/Helpers/SongHelper.js",
		"Scripts/Repositories/PVRepository.js",
		"Scripts/ViewModels/BasicListEditViewModel.js",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.js",
		"Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
		"Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
		"Scripts/ViewModels/CustomNameEditViewModel.js",
		"Scripts/ViewModels/PVs/PVEditViewModel.js",
		"Scripts/ViewModels/PVs/PVListEditViewModel.js",
		"Scripts/ViewModels/Song/LyricsForSongEditViewModel.js",
		"Scripts/ViewModels/Song/SongEditViewModel.js",
		"Scripts/Song/Edit.js"
	], "bundles/Song/Edit.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.js",
		"Scripts/ViewModels/Song/SongMergeViewModel.js"
	], "bundles/Song/Merge.js")*/

	/*.scripts([
		"Scripts/url.js",
		"Scripts/Shared/Routing/ObservableUrlParamRouter.js",
		"Scripts/KnockoutExtensions/SlideVisible.js",				
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
		"Scripts/ViewModels/Song/RankingsViewModel.js"
	], "bundles/Song/TopRated.js")*/

	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/KnockoutExtensions/SlideVisible.js",				
		"Scripts/KnockoutExtensions/ScrollEnd.js",
		"Scripts/KnockoutExtensions/ArtistAutoComplete.js",
		"Scripts/Helpers/PVHelper.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/Repositories/SongListRepository.js",
		"Scripts/Models/ResourcesManager.js",
		"Scripts/ViewModels/Search/ArtistFilter.js",
		"Scripts/ViewModels/Search/ArtistFilters.js",
		"Scripts/ViewModels/Search/TagFilter.js",
		"Scripts/ViewModels/Search/TagFilters.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.js",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
		"Scripts/ViewModels/PVs/PVPlayersFactory.js",
		"Scripts/ViewModels/PVs/PVPlayerFile.js",
		"Scripts/ViewModels/PVs/PVPlayerNico.js",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.js",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongListAdapter.js",
		"Scripts/ViewModels/Tag/TagListViewModel.js",
		"Scripts/ViewModels/Tag/TagsEditViewModel.js",
		"Scripts/ViewModels/SongList/SongListViewModel.js"
	], "bundles/SongList/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
		"Scripts/Repositories/SongListRepository.js",
		"Scripts/ViewModels/SongList/SongListEditViewModel.js",
		"Scripts/SongList/Edit.js"
	], "bundles/SongList/Edit.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/Repositories/SongListRepository.js",
		"Scripts/Repositories/TagRepository.js",
		"Scripts/ViewModels/Search/TagFilter.js",
		"Scripts/ViewModels/Search/TagFilters.js",
		"Scripts/ViewModels/SongList/SongListsBaseViewModel.js",
		"Scripts/ViewModels/SongList/FeaturedSongListsViewModel.js"
	], "bundles/SongList/Featured.js")*/

	/*.scripts([
		"Scripts/ViewModels/SongList/ImportSongListViewModel.js"
	], "bundles/SongList/Import.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
		"Scripts/Repositories/TagRepository.js",
		"Scripts/ViewModels/Tag/TagDetailsViewModel.js",
		"Scripts/Tag/Details.js"
	], "bundles/Tag/Details.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.js",
		"Scripts/Repositories/TagRepository.js",
		"Scripts/Helpers/KnockoutHelper.js",
		"Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
		"Scripts/ViewModels/Globalization/NamesEditViewModel.js",
		"Scripts/ViewModels/TagEditViewModel.js",
		"Scripts/Tag/Edit.js"
	], "bundles/Tag/Edit.js")*/

	/*.scripts([
		"Scripts/Repositories/TagRepository.js",
		"Scripts/ViewModels/Tag/TagCreateViewModel.js"
	], "bundles/Tag/Index.js")*/

	/*.scripts([
		"Scripts/Helpers/EntryMergeValidationHelper.js",
		"Scripts/Repositories/TagRepository.js",
		"Scripts/ViewModels/Tag/TagMergeViewModel.js"
	], "bundles/Tag/Merge.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/ArtistAutoComplete.js",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.js",
		"Scripts/ViewModels/User/AlbumCollectionViewModel.js"
	], "bundles/User/AlbumCollection.js")*/

	/*.scripts([
		"Scripts/soundcloud-api.js",
		"Scripts/Models/Users/UserEventRelationshipType.js",
		"Scripts/KnockoutExtensions/SlideVisible.js",				
		"Scripts/KnockoutExtensions/ArtistAutoComplete.js",
		"Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
		"Scripts/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete.js",
		"Scripts/KnockoutExtensions/Highcharts.js",				
		"Scripts/KnockoutExtensions/ScrollEnd.js",
		"Scripts/KnockoutExtensions/FormatDateFilter.js",
		"Scripts/Helpers/HighchartsHelper.js",				
		"Scripts/Helpers/PVHelper.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/Repositories/TagRepository.js",
		"Scripts/ViewModels/DeleteEntryViewModel.js",
		"Scripts/ViewModels/PVs/PVPlayersFactory.js",
		"Scripts/ViewModels/PVs/PVPlayerFile.js",
		"Scripts/ViewModels/PVs/PVPlayerNico.js",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.js",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForRatedSongsAdapter.js",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
		"Scripts/ViewModels/SongList/SongListsBaseViewModel.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilter.js",
		"Scripts/ViewModels/Search/AdvancedSearchFilters.js",
		"Scripts/ViewModels/Search/ArtistFilter.js",
		"Scripts/ViewModels/Search/ArtistFilters.js",
		"Scripts/ViewModels/Search/TagFilter.js",
		"Scripts/ViewModels/Search/TagFilters.js",
		"Scripts/ViewModels/User/FollowedArtistsViewModel.js",
		"Scripts/ViewModels/User/RatedSongsSearchViewModel.js",
		"Scripts/ViewModels/User/AlbumCollectionViewModel.js",
		"Scripts/ViewModels/User/UserDetailsViewModel.js",
		"Scripts/User/Details.js"
	], "bundles/User/Details.js")*/

	/*.scripts([
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/Models/ResourcesManager.js",
		"Scripts/ViewModels/User/ListUsersViewModel.js"
	], "bundles/User/Index.js")*/

	/*.scripts([
		"Scripts/KnockoutExtensions/BindingHandlers/UserAutocomplete.js",
		"Scripts/ViewModels/User/UserMessagesViewModel.js"
	], "bundles/User/Messages.js")*/

	/*.scripts([
		"Scripts/ViewModels/User/MySettingsViewModel.js"
	], "bundles/User/MySettings.js")*/

	/*.scripts([
		"Scripts/soundcloud-api.js",				
		"Scripts/KnockoutExtensions/SlideVisible.js",				
		"Scripts/KnockoutExtensions/ArtistAutoComplete.js",
		"Scripts/KnockoutExtensions/ScrollEnd.js",
		"Scripts/Helpers/PVHelper.js",
		"Scripts/Repositories/ResourceRepository.js",
		"Scripts/ViewModels/PVs/PVPlayersFactory.js",
		"Scripts/ViewModels/PVs/PVPlayerFile.js",
		"Scripts/ViewModels/PVs/PVPlayerNico.js",
		"Scripts/ViewModels/PVs/PVPlayerYoutube.js",
		"Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
		"Scripts/ViewModels/PVs/PVPlayerViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
		"Scripts/ViewModels/Song/PlayList/PlayListRepositoryForRatedSongsAdapter.js",
		"Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
		"Scripts/ViewModels/User/RatedSongsSearchViewModel.js"
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
