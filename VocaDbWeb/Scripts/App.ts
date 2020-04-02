
// ~/bundles/shared/common
export { setLanguagePreferenceCookie } from './Shared/TopBar';
export { default as ui } from './Shared/MessagesTyped';
export { default as functions } from './Shared/GlobalFunctions';
export { default as UrlMapper } from './Shared/UrlMapper';
import './KnockoutExtensions/StopBinding';
import './KnockoutExtensions/Show';
export { default as EntryReportRepository } from './Repositories/EntryReportRepository';
export { default as UserRepository } from './Repositories/UserRepository';
export { default as TopBarViewModel } from './ViewModels/TopBarViewModel';

// ~/bundles/shared/main
export { default as EntryUrlMapper } from './Shared/EntryUrlMapper';
import './KnockoutExtensions/ConfirmClick';
import './KnockoutExtensions/Dialog';
import './KnockoutExtensions/EntryToolTip';
import './KnockoutExtensions/jqButton';
import './KnockoutExtensions/jqButtonset';
import './KnockoutExtensions/Markdown';
import './KnockoutExtensions/ToggleClick';
import './KnockoutExtensions/Song/SongTypeLabel';
import './KnockoutExtensions/Bootstrap/Tooltip';
import './KnockoutExtensions/qTip';
import './KnockoutExtensions/TagAutoComplete';
import './KnockoutExtensions/Filters/Truncate';
export { default as RepositoryFactory } from './Repositories/RepositoryFactory';
export { default as TagRepository } from './Repositories/TagRepository';
export { default as ResourceRepository } from './Repositories/ResourceRepository';
export { default as SongRepository } from './Repositories/SongRepository';
export { default as ArtistRepository } from './Repositories/ArtistRepository';

// ~/bundles/shared/edit
export { default as DialogService } from './Shared/DialogService';
import './KnockoutExtensions/ArtistAutoComplete';
import './KnockoutExtensions/SongAutoComplete';
import './KnockoutExtensions/FocusOut';
import './KnockoutExtensions/InitialValue';

// ~/bundles/Home/Index
export { default as NewsListViewModel } from './ViewModels/NewsListViewModel';
export { initPage as initHomeIndexPage } from './Home/Index';

// ~/bundles/ActivityEntry/Index
import './KnockoutExtensions/MomentJsTimeAgo';
//export { default as ResourceRepository } from './Repositories/ResourceRepository';
export { default as ActivityEntryListViewModel } from './ViewModels/ActivityEntry/ActivityEntryListViewModel';

// ~/bundles/Admin/ManageEntryTagMappings
import './KnockoutExtensions/TagAutoComplete';
export { default as ManageEntryTagMappingsViewModel } from './ViewModels/Admin/ManageEntryTagMappingsViewModel';

// ~/bundles/Admin/ManageTagMappings
import './KnockoutExtensions/TagAutoComplete';
export { default as ManageTagMappingsViewModel } from './ViewModels/Admin/ManageTagMappingsViewModel';

// ~/bundles/Admin/ManageIPRules
import './KnockoutExtensions/FormatDateFilter';
export { default as AdminRepository } from './Repositories/AdminRepository';
export { default as ManageIPRulesViewModel } from './ViewModels/Admin/ManageIPRulesViewModel';

// ~/bundles/Admin/ViewAuditLog
export { default as ViewAuditLogViewModel } from './ViewModels/Admin/ViewAuditLogViewModel';

// ~/bundles/Album/Create
export { default as AlbumCreateViewModel } from './ViewModels/Album/AlbumCreateViewModel';

// ~/bundles/Album/Deleted
export { default as AlbumRepository } from './Repositories/AlbumRepository';
export { default as DeletedAlbumsViewModel } from './ViewModels/Album/DeletedAlbumsViewModel';

// ~/bundles/Album/Details
import './KnockoutExtensions/MomentJsTimeAgo';
import './KnockoutExtensions/FormatDateFilter';
export { default as AlbumDetailsViewModel } from './ViewModels/Album/AlbumDetailsViewModel';
export { initAlbumDetailsPage } from './Album/Details';

// ~/bundles/Album/Edit
import './KnockoutExtensions/ParseInteger';
import './KnockoutExtensions/FormatLengthSecondsFilter';
import './KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
import './KnockoutExtensions/FormatDateFilter';
export { default as AlbumEditViewModel } from './ViewModels/Album/AlbumEditViewModel';
export { initPage as initAlbumEditPage } from './Album/Edit';

// ~/bundles/Album/Merge
import './KnockoutExtensions/AlbumAutoComplete';
//export { default as AlbumRepository } from './Repositories/AlbumRepository';
export { default as AlbumMergeViewModel } from './ViewModels/Album/AlbumMergeViewModel';

// ~/bundles/Album/ViewVersion
//export { default as AlbumRepository } from './Repositories/AlbumRepository';
export { default as ArchivedAlbumViewModel } from './ViewModels/Album/ArchivedAlbumViewModel';

// ~/bundles/Artist/Create
export { default as ArtistCreateViewModel } from './ViewModels/ArtistCreateViewModel';

// ~/bundles/Artist/Details
import './KnockoutExtensions/MomentJsTimeAgo';
import './KnockoutExtensions/SlideVisible';
import './KnockoutExtensions/ScrollEnd';
import './KnockoutExtensions/Highcharts';
export { default as PVPlayersFactory } from './ViewModels/PVs/PVPlayersFactory';
export { default as ArtistDetailsViewModel } from './ViewModels/Artist/ArtistDetailsViewModel';
export { initPage as initArtistDetailsPage } from './Artist/Details';

// ~/bundles/Artist/Edit
import './KnockoutExtensions/BindingHandlers/DatePicker';
export { default as ArtistEditViewModel } from './ViewModels/Artist/ArtistEditViewModel';
export { initPage as initArtistEditPage } from './Artist/Edit';

// ~/bundles/Artist/Merge
import './KnockoutExtensions/ArtistAutoComplete';
export { default as ArtistMergeViewModel } from './ViewModels/Artist/ArtistMergeViewModel';

// ~/bundles/Artist/ViewVersion
export { default as ArchivedArtistViewModel } from './ViewModels/Artist/ArchivedArtistViewModel';

// ~/bundles/Discussion/Index
import './KnockoutExtensions/FormatDateFilter';
import './KnockoutExtensions/MomentJsTimeAgo';
export { default as DiscussionIndexViewModel } from './ViewModels/Discussion/DiscussionIndexViewModel';

// ~/bundles/EventSeries/Details
export { default as EventSeriesDetailsViewModel } from './ViewModels/ReleaseEvent/EventSeriesDetailsViewModel';

// ~/bundles/EventSeries/Edit
export { default as ReleaseEventRepository } from './Repositories/ReleaseEventRepository';
export { default as ReleaseEventSeriesEditViewModel } from './ViewModels/ReleaseEvent/ReleaseEventSeriesEditViewModel';
export { initPage as initEventSeriesEditPage } from './Event/SeriesEdit';

// ~/bundles/EventSeries/ViewVersion
//export { default as ReleaseEventRepository } from './Repositories/ReleaseEventRepository';
export { default as ArchivedEntryViewModel } from './ViewModels/ArchivedEntryViewModel';

// ~/bundles/MikuDbAlbum/PrepareForImport
export { initPage as initMikuDbAlbumPrepareForImportPage } from './MikuDbAlbums/PrepareForImport';

// ~/bundles/ReleaseEvent/Details
//export { default as ReleaseEventRepository } from './Repositories/ReleaseEventRepository';
export { default as ReleaseEventDetailsViewModel } from './ViewModels/ReleaseEvent/ReleaseEventDetailsViewModel'

// ~/bundles/ReleaseEvent/Edit
import './KnockoutExtensions/BindingHandlers/DatePicker';
import './KnockoutExtensions/ReleaseEventSeriesAutoComplete';
import './KnockoutExtensions/BindingHandlers/SongListAutoComplete';
import './KnockoutExtensions/FormatDateFilter';
import './KnockoutExtensions/FormatLengthSecondsFilter';
export { default as ReleaseEventEditViewModel } from './ViewModels/ReleaseEvent/ReleaseEventEditViewModel';
export { initPage as initReleaseEventEditPage } from './Event/Edit';

// ~/bundles/ReleaseEvent/ViewVersion
//export { default as ReleaseEventRepository } from './Repositories/ReleaseEventRepository';
//export { default as ReleaseEventDetailsViewModel } from './ViewModels/ReleaseEvent/ReleaseEventDetailsViewModel'

// ~/bundles/Search/Index
import './KnockoutExtensions/Artist/ArtistTypeLabel';
import './KnockoutExtensions/Tag/TagCategoryAutoComplete';
import './KnockoutExtensions/ArtistAutoComplete';
import './KnockoutExtensions/SlideVisible';
import './KnockoutExtensions/ScrollEnd';
import './KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
import './KnockoutExtensions/BindingHandlers/DatePicker';
import './KnockoutExtensions/SongAutoComplete';
//export { default as PVPlayersFactory } from './ViewModels/PVs/PVPlayersFactory';
export { default as SearchViewModel } from './ViewModels/Search/SearchViewModel';

// ~/bundles/Song/Create
import './KnockoutExtensions/Artist/ArtistTypeLabel';
export { default as SongCreateViewModel } from './ViewModels/SongCreateViewModel';

// ~/bundles/Song/Details
import './KnockoutExtensions/MomentJsTimeAgo';
export { default as SongDetailsViewModel } from './ViewModels/Song/SongDetailsViewModel';
export { initPage as initSongDetailsPage } from './Song/Details';

// ~/bundles/Song/Edit
import './KnockoutExtensions/FormatDateFilter';
import './KnockoutExtensions/FormatLengthSecondsFilter';
import './KnockoutExtensions/BindingHandlers/DatePicker';
import './KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
export { default as SongEditViewModel } from './ViewModels/Song/SongEditViewModel';
export { initPage as initSongEditPage } from './Song/Edit';

// ~/bundles/Song/Merge
export { default as SongMergeViewModel } from './ViewModels/Song/SongMergeViewModel';

// ~/bundles/Song/TopRated
import './KnockoutExtensions/SlideVisible';
import './KnockoutExtensions/FormatDateFilter';
export { default as RankingsViewModel } from './ViewModels/Song/RankingsViewModel';

// ~/bundles/Song/ViewVersion
export { default as ArchivedSongViewModel } from './ViewModels/Song/ArchivedSongViewModel';

// ~/bundles/SongList/Details
import './KnockoutExtensions/SlideVisible';
import './KnockoutExtensions/ScrollEnd';
import './KnockoutExtensions/ArtistAutoComplete';
//export { default as PVPlayersFactory } from './ViewModels/PVs/PVPlayersFactory';
export { default as SongListViewModel } from './ViewModels/SongList/SongListViewModel';

// ~/bundles/SongList/Edit
import './KnockoutExtensions/BindingHandlers/DatePicker';
export { default as SongListEditViewModel } from './ViewModels/SongList/SongListEditViewModel';
export { initPage as initSongListEditPage } from './SongList/Edit';

// ~/bundles/SongList/Featured
import './KnockoutExtensions/FormatDateFilter';
export { default as FeaturedSongListsViewModel } from './ViewModels/SongList/FeaturedSongListsViewModel';

// ~/bundles/SongList/Import
export { default as ImportSongListViewModel } from './ViewModels/SongList/ImportSongListViewModel';

// ~/bundles/Stats/Index
import './KnockoutExtensions/Highcharts';
export { default as StatsViewModel } from './ViewModels/StatsViewModel';

// ~/bundles/Tag/Details
import './KnockoutExtensions/MomentJsTimeAgo';
//export { default as TagRepository } from './Repositories/TagRepository';
export { default as TagDetailsViewModel } from './ViewModels/Tag/TagDetailsViewModel';
export { initTagsPage, initChart } from './Tag/Details';

// ~/bundles/Tag/Edit
import './KnockoutExtensions/Tag/TagCategoryAutoComplete';
//export { default as TagRepository } from './Repositories/TagRepository';
export { default as TagEditViewModel } from './ViewModels/TagEditViewModel';
export { initPage as initTagEditPage } from './Tag/Edit';

// ~/bundles/Tag/Index
//export { default as TagRepository } from './Repositories/TagRepository';
export { default as TagCreateViewModel } from './ViewModels/Tag/TagCreateViewModel';

// ~/bundles/Tag/Merge
//export { default as TagRepository } from './Repositories/TagRepository';
export { default as TagMergeViewModel } from './ViewModels/Tag/TagMergeViewModel';

// ~/bundles/Tag/ViewVersion
//export { default as TagRepository } from './Repositories/TagRepository';
//export { default as ArchivedEntryViewModel } from './ViewModels/ArchivedEntryViewModel';

// ~/bundles/User/AlbumCollection
import './KnockoutExtensions/ArtistAutoComplete';
import './KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
export { default as AlbumCollectionViewModel } from './ViewModels/User/AlbumCollectionViewModel';

// ~/bundles/User/Details
import './KnockoutExtensions/SlideVisible';
import './KnockoutExtensions/ArtistAutoComplete';
import './KnockoutExtensions/MomentJsTimeAgo';
import './KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
import './KnockoutExtensions/Highcharts';
import './KnockoutExtensions/ScrollEnd';
import './KnockoutExtensions/FormatDateFilter';
//export { default as PVPlayersFactory } from './ViewModels/PVs/PVPlayersFactory';
export { default as FollowedArtistsViewModel } from './ViewModels/User/FollowedArtistsViewModel';
export { default as RatedSongsSearchViewModel } from './ViewModels/User/RatedSongsSearchViewModel';
//export { default as AlbumCollectionViewModel } from './ViewModels/User/AlbumCollectionViewModel';
export { default as UserDetailsViewModel } from './ViewModels/User/UserDetailsViewModel';
export { initPage as initUserDetailsPage } from './User/Details';

// ~/bundles/User/Edit
export { initPage as initUserEditPage } from './User/Edit';

// ~/bundles/User/Index
export { default as ListUsersViewModel } from './ViewModels/User/ListUsersViewModel';

// ~/bundles/User/Messages
import './KnockoutExtensions/BindingHandlers/UserAutocomplete';
export { default as UserMessagesViewModel } from './ViewModels/User/UserMessagesViewModel';

// ~/bundles/User/MySettings
export { default as MySettingsViewModel } from './ViewModels/User/MySettingsViewModel';

// ~/bundles/User/RatedSongs
import './KnockoutExtensions/SlideVisible';
import './KnockoutExtensions/ArtistAutoComplete';
import './KnockoutExtensions/ScrollEnd';
//export { default as PVPlayersFactory } from './ViewModels/PVs/PVPlayersFactory';
//export { default as RatedSongsSearchViewModel } from './ViewModels/User/RatedSongsSearchViewModel';

// ~/bundles/User/RequestVerification
export { default as RequestVerificationViewModel } from './ViewModels/User/RequestVerificationViewModel';

// ~/bundles/Venue/Details
export { default as VenueDetailsViewModel } from './ViewModels/Venue/VenueDetailsViewModel';

// ~/bundles/Venue/Edit
export { default as VenueEditViewModel } from './ViewModels/Venue/VenueEditViewModel';
export { initPage as initVenueEditPage } from './Venue/Edit';

// ~/bundles/Venue/ViewVersion
export { default as VenueRepository } from './Repositories/VenueRepository';
