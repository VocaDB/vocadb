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

			// Included on all pages
			// Generally the references go from viewmodels -> repositories -> models -> support classes
			bundles.Add(new ScriptBundle("~/bundles/shared/common").Include(
				"~/Scripts/Shared/TopBar.js", 
				"~/Scripts/Shared/MessagesTyped.js",
				"~/Scripts/Shared/GlobalFunctions.js",
				"~/Scripts/Shared/UrlMapper.js",
				"~/Scripts/Helpers/AjaxHelper.js", 
				"~/Scripts/KnockoutExtensions/StopBinding.js",
				"~/Scripts/Repositories/EntryReportRepository.js",
				"~/Scripts/Repositories/UserRepository.js",
				"~/Scripts/ViewModels/TopBarViewModel.js"
			));

			// Included on all pages except the front page (to optimize front page load time).
			bundles.Add(new ScriptBundle("~/bundles/shared/main").Include(
				"~/Scripts/Helpers/HtmlHelper.js", 
				"~/Scripts/Models/EntryType.js",
				"~/Scripts/Shared/EntryUrlMapper.js",
				"~/Scripts/Shared/EntryAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ConfirmClick.js",
				"~/Scripts/KnockoutExtensions/Dialog.js",
				"~/Scripts/KnockoutExtensions/EntryToolTip.js",
				"~/Scripts/KnockoutExtensions/jqButton.js",
				"~/Scripts/KnockoutExtensions/jqButtonset.js",
				"~/Scripts/KnockoutExtensions/Markdown.js",
				"~/Scripts/KnockoutExtensions/Show.js",
				"~/Scripts/KnockoutExtensions/ToggleClick.js",
				"~/Scripts/KnockoutExtensions/Song/SongTypeLabel.js",
				"~/Scripts/Models/NameMatchMode.js",
				"~/Scripts/Models/Artists/ArtistType.js",
				"~/Scripts/Models/SongVoteRating.js",				// Referred by UserRepository
				"~/Scripts/Models/PVs/PVService.js",
				"~/Scripts/Models/PVServiceIcons.js",				
				"~/Scripts/Models/Globalization/ContentLanguagePreference.js",
				"~/Scripts/Models/EntryOptionalFields.js",
				"~/Scripts/Repositories/BaseRepository.js",
				"~/Scripts/Repositories/RepositoryFactory.js",
				"~/Scripts/Repositories/AdminRepository.js",
				"~/Scripts/Repositories/SongRepository.js",
				"~/Scripts/Repositories/ArtistRepository.js",
				"~/Scripts/ViewModels/PagedItemsViewModel.js",
				"~/Scripts/ViewModels/ServerSidePagingViewModel.js",
				"~/Scripts/ViewModels/PVRatingButtonsViewModel.js",
				"~/Scripts/ViewModels/ReportEntryViewModel.js"
			));

			// Included on all entry edit and create pages (album, artist, my settings etc.)
			bundles.Add(new ScriptBundle("~/bundles/shared/edit").Include(
				"~/Scripts/knockout-sortable.js",
				"~/Scripts/Models/WebLinkCategory.js",
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
				"~/Scripts/KnockoutExtensions/qTip.js",
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
				"~/Scripts/moment-with-locales.js",
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
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/ViewModels/CommentViewModel.js",
				"~/Scripts/ViewModels/EditableCommentsViewModel.js",
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/Album/AlbumDetailsViewModel.js",
				"~/Scripts/Album/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Album/Edit").Include(
				"~/Scripts/KnockoutExtensions/ParseInteger.js",
				"~/Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
				"~/Scripts/KnockoutExtensions/ReleaseEventNameAutoComplete.js",
				"~/Scripts/Models/Albums/AlbumType.js",
				"~/Scripts/Models/Globalization/ContentLanguageSelection.js",
				"~/Scripts/Models/Artists/ArtistRoles.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/Repositories/PVRepository.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVListEditViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileEditViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileListEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
				"~/Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
				"~/Scripts/ViewModels/SongInAlbumEditViewModel.js",
				"~/Scripts/ViewModels/Album/AlbumEditViewModel.js",
				"~/Scripts/Album/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Album/Merge").Include(
				"~/Scripts/KnockoutExtensions/AlbumAutoComplete.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/ViewModels/BasicEntryLinkViewModel.js",
				"~/Scripts/ViewModels/Album/AlbumMergeViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Create").Include(
				"~/Scripts/ViewModels/ArtistCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Details").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Models/Songs/SongType.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/ViewModels/CommentViewModel.js",
				"~/Scripts/ViewModels/EditableCommentsViewModel.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.js",
				"~/Scripts/ViewModels/Search/ArtistFilter.js",
				"~/Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
				"~/Scripts/ViewModels/Search/AlbumSearchViewModel.js",
				"~/Scripts/ViewModels/Search/SongSearchViewModel.js",
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistDetailsViewModel.js",
				"~/Scripts/Artist/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Edit").Include(
				"~/Scripts/Models/Globalization/ContentLanguageSelection.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/BasicEntryLinkViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileEditViewModel.js",
				"~/Scripts/ViewModels/EntryPictureFileListEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistEditViewModel.js",
				"~/Scripts/Artist/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Merge").Include(
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/ViewModels/BasicEntryLinkViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistMergeViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Discussion/Index").Include(
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/page.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/Repositories/DiscussionRepository.js",
				"~/Scripts/ViewModels/CommentViewModel.js",
				"~/Scripts/ViewModels/EditableCommentsViewModel.js",
				"~/Scripts/ViewModels/Discussion/DiscussionTopicViewModel.js",
				"~/Scripts/ViewModels/Discussion/DiscussionIndexViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Search/Index").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/Models/Songs/SongType.js",
				"~/Scripts/KnockoutExtensions/Artist/ArtistTypeLabel.js",
				"~/Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/Repositories/AlbumRepository.js",
				"~/Scripts/Repositories/EntryRepository.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongsAdapter.js",
				"~/Scripts/ViewModels/Search/ArtistFilter.js",
				"~/Scripts/ViewModels/Search/SearchViewModel.js",
				"~/Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js",
				"~/Scripts/ViewModels/Search/AnythingSearchViewModel.js",
				"~/Scripts/ViewModels/Search/ArtistSearchViewModel.js",
				"~/Scripts/ViewModels/Search/AlbumSearchViewModel.js",
				"~/Scripts/ViewModels/Search/SongSearchViewModel.js",
				"~/Scripts/ViewModels/Search/TagSearchViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Create").Include(
				"~/Scripts/ViewModels/SongCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Details").Include(				
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/ViewModels/CommentViewModel.js",
				"~/Scripts/ViewModels/EditableCommentsViewModel.js",
				"~/Scripts/ViewModels/Tag/TagListViewModel.js",
				"~/Scripts/ViewModels/Tag/TagsEditViewModel.js",
				"~/Scripts/ViewModels/Song/SongDetailsViewModel.js",
				"~/Scripts/Song/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Edit").Include(
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/KnockoutExtensions/FormatLengthSecondsFilter.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
				"~/Scripts/Models/Artists/ArtistRoles.js",
				"~/Scripts/Models/Globalization/ContentLanguageSelection.js",
				"~/Scripts/Models/Songs/SongType.js",
				"~/Scripts/Models/Tags/Tag.js",
				"~/Scripts/Helpers/KnockoutHelper.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/Helpers/SongHelper.js",
				"~/Scripts/Repositories/PVRepository.js",
				"~/Scripts/ViewModels/BasicListEditViewModel.js",
				"~/Scripts/ViewModels/BasicEntryLinkViewModel.js",
				"~/Scripts/ViewModels/Globalization/LocalizedStringWithIdEditViewModel.js",
				"~/Scripts/ViewModels/Globalization/NamesEditViewModel.js",
				"~/Scripts/ViewModels/Artist/ArtistRolesEditViewModel.js",
				"~/Scripts/ViewModels/ArtistForAlbumEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVEditViewModel.js",
				"~/Scripts/ViewModels/PVs/PVListEditViewModel.js",
				"~/Scripts/ViewModels/Song/LyricsForSongEditViewModel.js",
				"~/Scripts/ViewModels/Song/SongEditViewModel.js",
				"~/Scripts/Song/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Merge").Include(
				"~/Scripts/ViewModels/BasicEntryLinkViewModel.js",
				"~/Scripts/ViewModels/Song/SongMergeViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/TopRated").Include(
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/url.js",
				"~/Scripts/Shared/Routing/UrlParamRouter.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/Song/RankingsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Details").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Repositories/SongListRepository.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForSongListAdapter.js",
				"~/Scripts/ViewModels/SongList/SongListViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Edit").Include(
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/KnockoutExtensions/BindingHandlers/DatePicker.js",
				"~/Scripts/Repositories/SongListRepository.js",
				"~/Scripts/ViewModels/SongList/SongListEditViewModel.js",
				"~/Scripts/SongList/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Featured").Include(
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/Repositories/SongListRepository.js",
				"~/Scripts/ViewModels/SongList/SongListsBaseViewModel.js",
                "~/Scripts/ViewModels/SongList/FeaturedSongListsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/SongList/Import").Include(
				"~/Scripts/ViewModels/SongList/ImportSongListViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Tag/Edit").Include(
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/KnockoutExtensions/Tag/TagCategoryAutoComplete.js",
				"~/Scripts/Repositories/TagRepository.js",
				"~/Scripts/ViewModels/TagEditViewModel.js",
				"~/Scripts/Tag/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/AlbumCollection").Include(
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ReleaseEventNameAutoComplete.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/User/AlbumCollectionViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Details").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/MomentJsTimeAgo.js",
				"~/Scripts/KnockoutExtensions/ReleaseEventNameAutoComplete.js",
				"~/Scripts/KnockoutExtensions/Highcharts.js",				
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/KnockoutExtensions/FormatDateFilter.js",
				"~/Scripts/Helpers/HighchartsHelper.js",				
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/CommentViewModel.js",
				"~/Scripts/ViewModels/EditableCommentsViewModel.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
				"~/Scripts/ViewModels/PVs/PVPlayerYoutube.js",
				"~/Scripts/ViewModels/PVs/PVPlayerSoundCloud.js",
				"~/Scripts/ViewModels/PVs/PVPlayerViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListViewModel.js",
				"~/Scripts/ViewModels/Song/PlayList/PlayListRepositoryForRatedSongsAdapter.js",
				"~/Scripts/ViewModels/Song/SongWithPreviewViewModel.js",
				"~/Scripts/ViewModels/SongList/SongListsBaseViewModel.js",
				"~/Scripts/ViewModels/User/FollowedArtistsViewModel.js",
				"~/Scripts/ViewModels/User/RatedSongsSearchViewModel.js",
				"~/Scripts/ViewModels/User/AlbumCollectionViewModel.js",
				"~/Scripts/ViewModels/User/UserDetailsViewModel.js",
				"~/Scripts/User/Details.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Index").Include(
				"~/Scripts/moment-with-locales.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/Models/ResourcesManager.js",
				"~/Scripts/ViewModels/User/ListUsersViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Messages").Include(
				"~/Scripts/KnockoutExtensions/BindingHandlers/UsernameAutocomplete.js",				
				"~/Scripts/ViewModels/User/UserMessagesViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/MySettings").Include(
				"~/Scripts/ViewModels/User/MySettingsViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/RatedSongs").Include(
				"~/Scripts/soundcloud-api.js",				
				"~/Scripts/KnockoutExtensions/SlideVisible.js",				
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/TagAutoComplete.js",
				"~/Scripts/KnockoutExtensions/ScrollEnd.js",
				"~/Scripts/Helpers/ArtistHelper.js",
				"~/Scripts/Helpers/PVHelper.js",
				"~/Scripts/Helpers/DateTimeHelper.js",
				"~/Scripts/Repositories/ResourceRepository.js",
				"~/Scripts/ViewModels/PVs/PVPlayersFactory.js",
				"~/Scripts/ViewModels/PVs/PVPlayerFile.js",
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
				.IncludeDirectory("~/Scripts/Models", "*.js", true)
				.IncludeDirectory("~/Scripts/Helpers", "*.js")
				.IncludeDirectory("~/Scripts/Repositories", "*.js", true)
				.Include("~/Scripts/ViewModels/Search/SearchCategoryBaseViewModel.js")
				.IncludeDirectory("~/Scripts/ViewModels", "*.js", true)
				.Include("~/Scripts/Shared/WebLinkMatcher.js")
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