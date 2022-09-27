/* eslint-disable prettier/prettier */
/* eslint-disable import/first */

// ~/bundles/shared/common
export { setLanguagePreferenceCookie } from '@/Shared/TopBar';
export { ui } from '@/Shared/MessagesTyped';
export { functions } from '@/Shared/GlobalFunctions';
export { HttpClient } from '@/Shared/HttpClient';
export { UrlMapper } from '@/Shared/UrlMapper';
import '@/KnockoutExtensions/StopBinding';
import '@/KnockoutExtensions/Show';
export { EntryReportRepository } from '@/Repositories/EntryReportRepository';
export { UserRepository } from '@/Repositories/UserRepository';
export { TopBarViewModel } from '@/ViewModels/TopBarViewModel';
export { SharedLayoutScripts } from '@/Shared/LayoutScripts';

// ~/bundles/shared/main
export { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import '@/KnockoutExtensions/ConfirmClick';
import '@/KnockoutExtensions/Dialog';
import '@/KnockoutExtensions/jqButton';
import '@/KnockoutExtensions/jqButtonset';
import '@/KnockoutExtensions/ToggleClick';
import '@/KnockoutExtensions/Song/SongTypeLabel';
import '@/KnockoutExtensions/Bootstrap/Tooltip';
import '@/KnockoutExtensions/TagAutoComplete';
import '@/KnockoutExtensions/Filters/Truncate';
export { RepositoryFactory } from '@/Repositories/RepositoryFactory';
export { TagRepository } from '@/Repositories/TagRepository';
export { ResourceRepository } from '@/Repositories/ResourceRepository';
export { SongRepository } from '@/Repositories/SongRepository';
export { ArtistRepository } from '@/Repositories/ArtistRepository';

// ~/bundles/shared/edit
export { DialogService } from '@/Shared/DialogService';
import '@/KnockoutExtensions/ArtistAutoComplete';
import '@/KnockoutExtensions/SongAutoComplete';
import '@/KnockoutExtensions/FocusOut';
import '@/KnockoutExtensions/InitialValue';

// ~/bundles/Home/Index
export { NewsListViewModel } from '@/ViewModels/NewsListViewModel';
export { HomeIndex } from '@/Home/Index';

// ~/bundles/ActivityEntry/Index
import '@/KnockoutExtensions/MomentJsTimeAgo';
//export { ResourceRepository } from '@/Repositories/ResourceRepository';
export { ActivityEntryListViewModel } from '@/ViewModels/ActivityEntry/ActivityEntryListViewModel';
export { ActivityEntryIndex } from '@/ActivityEntry/Index';

// ~/bundles/Admin/ManageEntryTagMappings
import '@/KnockoutExtensions/TagAutoComplete';
export { ManageEntryTagMappingsViewModel } from '@/ViewModels/Admin/ManageEntryTagMappingsViewModel';
export { AdminManageEntryTagMappings } from '@/Admin/ManageEntryTagMappings';

// ~/bundles/Admin/ManageTagMappings
import '@/KnockoutExtensions/TagAutoComplete';
export { ManageTagMappingsViewModel } from '@/ViewModels/Admin/ManageTagMappingsViewModel';
export { AdminManageTagMappings } from '@/Admin/ManageTagMappings';

// ~/bundles/Admin/ManageIPRules
import '@/KnockoutExtensions/FormatDateFilter';
export { AdminRepository } from '@/Repositories/AdminRepository';
export { ManageIPRulesViewModel } from '@/ViewModels/Admin/ManageIPRulesViewModel';
export { AdminManageIPRules } from '@/Admin/ManageIPRules';

// ~/bundles/Admin/ManageWebhooks
export { AdminManageWebhooks } from '@/Admin/ManageWebhooks';

// ~/bundles/Admin/PVsByAuthor
export { AdminPVsByAuthor } from '@/Admin/PVsByAuthor';

// ~/bundles/Admin/ViewAuditLog
export { ViewAuditLogViewModel } from '@/ViewModels/Admin/ViewAuditLogViewModel';
export { AdminViewAuditLog } from '@/Admin/ViewAuditLog';

// ~/bundles/Album/Create
export { AlbumCreateViewModel } from '@/ViewModels/Album/AlbumCreateViewModel';
export { AlbumCreate } from '@/Album/Create';

// ~/bundles/Album/Deleted
export { AlbumRepository } from '@/Repositories/AlbumRepository';
export { DeletedAlbumsViewModel } from '@/ViewModels/Album/DeletedAlbumsViewModel';
export { AlbumDeleted } from '@/Album/Deleted';

// ~/bundles/Album/Details
import '@/KnockoutExtensions/MomentJsTimeAgo';
import '@/KnockoutExtensions/FormatDateFilter';
export { AlbumDetailsViewModel } from '@/ViewModels/Album/AlbumDetailsViewModel';
export { AlbumDetails } from '@/Album/Details';

// ~/bundles/Album/Edit
import '@/KnockoutExtensions/ParseInteger';
import '@/KnockoutExtensions/FormatLengthSecondsFilter';
import '@/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
import '@/KnockoutExtensions/FormatDateFilter';
export { AlbumEditViewModel } from '@/ViewModels/Album/AlbumEditViewModel';
export { AlbumEdit } from '@/Album/Edit';

// ~/bundles/Album/Merge
import '@/KnockoutExtensions/AlbumAutoComplete';
//export { AlbumRepository } from '@/Repositories/AlbumRepository';
export { AlbumMergeViewModel } from '@/ViewModels/Album/AlbumMergeViewModel';
export { AlbumMerge } from '@/Album/Merge';

// ~/bundles/Album/ViewVersion
//export { AlbumRepository } from '@/Repositories/AlbumRepository';
export { ArchivedAlbumViewModel } from '@/ViewModels/Album/ArchivedAlbumViewModel';
export { AlbumViewVersion } from '@/Album/ViewVersion';

// ~/bundles/Artist/Create
export { ArtistCreateViewModel } from '@/ViewModels/ArtistCreateViewModel';
export { ArtistCreate } from '@/Artist/Create';

// ~/bundles/Artist/Details
import '@/KnockoutExtensions/MomentJsTimeAgo';
import '@/KnockoutExtensions/SlideVisible';
import '@/KnockoutExtensions/ScrollEnd';
import '@/KnockoutExtensions/Highcharts';
export { PVPlayersFactory } from '@/ViewModels/PVs/PVPlayersFactory';
export { ArtistDetailsViewModel } from '@/ViewModels/Artist/ArtistDetailsViewModel';
export { ArtistDetails } from '@/Artist/Details';

// ~/bundles/Artist/Edit
import '@/KnockoutExtensions/BindingHandlers/DatePicker';
export { ArtistEditViewModel } from '@/ViewModels/Artist/ArtistEditViewModel';
export { ArtistEdit } from '@/Artist/Edit';

// ~/bundles/Artist/Merge
import '@/KnockoutExtensions/ArtistAutoComplete';
export { ArtistMergeViewModel } from '@/ViewModels/Artist/ArtistMergeViewModel';
export { ArtistMerge } from '@/Artist/Merge';

// ~/bundles/Artist/ViewVersion
export { ArchivedArtistViewModel } from '@/ViewModels/Artist/ArchivedArtistViewModel';
export { ArtistViewVersion } from '@/Artist/ViewVersion';

// ~/bundles/Comment/Index
export { CommentListViewModel } from '@/ViewModels/Comment/CommentListViewModel';

// ~/bundles/Comment/CommentsByUser
export { CommentCommentsByUser } from '@/Comment/CommentsByUser';

// ~/bundles/Discussion/Index
import '@/KnockoutExtensions/FormatDateFilter';
import '@/KnockoutExtensions/MomentJsTimeAgo';
export { DiscussionIndexViewModel } from '@/ViewModels/Discussion/DiscussionIndexViewModel';
export { DiscussionIndex } from '@/Discussion/Index';

// ~/bundles/EventSeries/Details
export { EventSeriesDetailsViewModel } from '@/ViewModels/ReleaseEvent/EventSeriesDetailsViewModel';
export { EventSeriesDetails } from '@/Event/SeriesDetails';

// ~/bundles/EventSeries/Edit
export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
export { ReleaseEventSeriesEditViewModel } from '@/ViewModels/ReleaseEvent/ReleaseEventSeriesEditViewModel';
export { EventEditSeries } from '@/Event/EditSeries';

// ~/bundles/EventSeries/ViewVersion
//export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
export { ArchivedEntryViewModel } from '@/ViewModels/ArchivedEntryViewModel';
export { EventViewSeriesVersion } from '@/Event/ViewSeriesVersion';

// ~/bundles/MikuDbAlbum/Index
export { MikuDbAlbumIndex } from '@/MikuDbAlbum/Index';

// ~/bundles/MikuDbAlbum/PrepareForImport
export { MikuDbAlbumPrepareForImport } from '@/MikuDbAlbum/PrepareForImport';

// ~/bundles/ReleaseEvent/Details
//export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
export { ReleaseEventDetailsViewModel } from '@/ViewModels/ReleaseEvent/ReleaseEventDetailsViewModel';
export { EventDetails } from '@/Event/Details';

// ~/bundles/ReleaseEvent/Edit
import '@/KnockoutExtensions/BindingHandlers/DatePicker';
import '@/KnockoutExtensions/ReleaseEventSeriesAutoComplete';
import '@/KnockoutExtensions/BindingHandlers/SongListAutoComplete';
import '@/KnockoutExtensions/BindingHandlers/VenueAutoComplete';
import '@/KnockoutExtensions/FormatDateFilter';
import '@/KnockoutExtensions/FormatLengthSecondsFilter';
export { ReleaseEventEditViewModel } from '@/ViewModels/ReleaseEvent/ReleaseEventEditViewModel';
export { EventEdit } from '@/Event/Edit';

// ~/bundles/ReleaseEvent/EventsBySeries
export { EventEventsBySeries } from '@/Event/EventsBySeries';

// ~/bundles/ReleaseEvent/EventsByVenue
export { EventEventsByVenue } from '@/Event/EventsByVenue';

// ~/bundles/ReleaseEvent/Index
export { EventIndex } from '@/Event/Index';

// ~/bundles/ReleaseEvent/ViewVersion
//export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
//export { ReleaseEventDetailsViewModel } from '@/ViewModels/ReleaseEvent/ReleaseEventDetailsViewModel'
export { EventViewVersion } from '@/Event/ViewVersion';

// ~/bundles/Search/Index
import '@/KnockoutExtensions/Artist/ArtistTypeLabel';
import '@/KnockoutExtensions/Tag/TagCategoryAutoComplete';
import '@/KnockoutExtensions/ArtistAutoComplete';
import '@/KnockoutExtensions/SlideVisible';
import '@/KnockoutExtensions/ScrollEnd';
import '@/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
import '@/KnockoutExtensions/BindingHandlers/DatePicker';
import '@/KnockoutExtensions/SongAutoComplete';
//export { PVPlayersFactory } from '@/ViewModels/PVs/PVPlayersFactory';
export { SearchViewModel } from '@/ViewModels/Search/SearchViewModel';
export { SearchIndex } from '@/Search/Index';

// ~/bundles/Song/Create
import '@/KnockoutExtensions/Artist/ArtistTypeLabel';
export { SongCreateViewModel } from '@/ViewModels/SongCreateViewModel';
export { SongCreate } from '@/Song/Create';

// ~/bundles/Song/Details
import '@/KnockoutExtensions/MomentJsTimeAgo';
export { SongDetailsViewModel } from '@/ViewModels/Song/SongDetailsViewModel';
export { SongDetails } from '@/Song/Details';

// ~/bundles/Song/Edit
import '@/KnockoutExtensions/FormatDateFilter';
import '@/KnockoutExtensions/FormatLengthSecondsFilter';
import '@/KnockoutExtensions/BindingHandlers/DatePicker';
import '@/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
export { SongEditViewModel } from '@/ViewModels/Song/SongEditViewModel';
export { SongEdit } from '@/Song/Edit';

// ~/bundles/Song/Merge
export { SongMergeViewModel } from '@/ViewModels/Song/SongMergeViewModel';
export { SongMerge } from '@/Song/Merge';

// ~/bundles/Song/TopRated
import '@/KnockoutExtensions/SlideVisible';
import '@/KnockoutExtensions/FormatDateFilter';
export { RankingsViewModel } from '@/ViewModels/Song/RankingsViewModel';
export { SongRankings } from '@/Song/Rankings';

// ~/bundles/Song/ViewVersion
export { ArchivedSongViewModel } from '@/ViewModels/Song/ArchivedSongViewModel';
export { SongViewVersion } from '@/Song/ViewVersion';

// ~/bundles/SongList/Details
import '@/KnockoutExtensions/SlideVisible';
import '@/KnockoutExtensions/ScrollEnd';
import '@/KnockoutExtensions/ArtistAutoComplete';
//export { PVPlayersFactory } from '@/ViewModels/PVs/PVPlayersFactory';
export { SongListViewModel } from '@/ViewModels/SongList/SongListViewModel';
export { SongListDetails } from '@/SongList/Details';

// ~/bundles/SongList/Edit
import '@/KnockoutExtensions/BindingHandlers/DatePicker';
export { SongListEditViewModel } from '@/ViewModels/SongList/SongListEditViewModel';
export { SongListEdit } from '@/SongList/Edit';

// ~/bundles/SongList/Featured
import '@/KnockoutExtensions/FormatDateFilter';
export { FeaturedSongListsViewModel } from '@/ViewModels/SongList/FeaturedSongListsViewModel';
export { SongListFeatured } from '@/SongList/Featured';

// ~/bundles/SongList/Import
export { ImportSongListViewModel } from '@/ViewModels/SongList/ImportSongListViewModel';
export { SongListImport } from '@/SongList/Import';

// ~/bundles/Stats/Index
import '@/KnockoutExtensions/Highcharts';
export { StatsViewModel } from '@/ViewModels/StatsViewModel';
export { StatsIndex } from '@/Stats/Index';

// ~/bundles/Tag/Details
import '@/KnockoutExtensions/MomentJsTimeAgo';
//export { TagRepository } from '@/Repositories/TagRepository';
export { TagDetailsViewModel } from '@/ViewModels/Tag/TagDetailsViewModel';
export { TagDetails } from '@/Tag/Details';

// ~/bundles/Tag/Edit
import '@/KnockoutExtensions/Tag/TagCategoryAutoComplete';
//export { TagRepository } from '@/Repositories/TagRepository';
export { TagEditViewModel } from '@/ViewModels/TagEditViewModel';
export { TagEdit } from '@/Tag/Edit';

// ~/bundles/Tag/Index
//export { TagRepository } from '@/Repositories/TagRepository';
export { TagCreateViewModel } from '@/ViewModels/Tag/TagCreateViewModel';
export { TagIndex } from '@/Tag/Index';

// ~/bundles/Tag/Merge
//export { TagRepository } from '@/Repositories/TagRepository';
export { TagMergeViewModel } from '@/ViewModels/Tag/TagMergeViewModel';
export { TagMerge } from '@/Tag/Merge';

// ~/bundles/Tag/ViewVersion
//export { TagRepository } from '@/Repositories/TagRepository';
//export { ArchivedEntryViewModel } from '@/ViewModels/ArchivedEntryViewModel';
export { TagViewVersion } from '@/Tag/ViewVersion';

// ~/bundles/User/AlbumCollection
import '@/KnockoutExtensions/ArtistAutoComplete';
import '@/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
export { AlbumCollectionViewModel } from '@/ViewModels/User/AlbumCollectionViewModel';
export { UserAlbumCollection } from '@/User/AlbumCollection';

// ~/bundles/User/Details
import '@/KnockoutExtensions/SlideVisible';
import '@/KnockoutExtensions/ArtistAutoComplete';
import '@/KnockoutExtensions/MomentJsTimeAgo';
import '@/KnockoutExtensions/BindingHandlers/ReleaseEventAutoComplete';
import '@/KnockoutExtensions/Highcharts';
import '@/KnockoutExtensions/ScrollEnd';
import '@/KnockoutExtensions/FormatDateFilter';
//export { PVPlayersFactory } from '@/ViewModels/PVs/PVPlayersFactory';
export { FollowedArtistsViewModel } from '@/ViewModels/User/FollowedArtistsViewModel';
export { RatedSongsSearchViewModel } from '@/ViewModels/User/RatedSongsSearchViewModel';
//export { AlbumCollectionViewModel } from '@/ViewModels/User/AlbumCollectionViewModel';
export { UserDetailsViewModel } from '@/ViewModels/User/UserDetailsViewModel';
export { UserDetails } from '@/User/Details';

// ~/bundles/User/Edit
export { UserEdit } from '@/User/Edit';

// ~/bundles/User/EntryEdits
export { UserEntryEdits } from '@/User/EntryEdits';

// ~/bundles/User/FavoriteSongs
export { UserFavoriteSongs } from '@/User/FavoriteSongs';

// ~/bundles/User/Index
export { ListUsersViewModel } from '@/ViewModels/User/ListUsersViewModel';
export { UserIndex } from '@/User/Index';

// ~/bundles/User/Messages
import '@/KnockoutExtensions/BindingHandlers/UserAutoComplete';
export { UserMessagesViewModel } from '@/ViewModels/User/UserMessagesViewModel';
export { UserMessages } from '@/User/Messages';

// ~/bundles/User/MySettings
export { MySettingsViewModel } from '@/ViewModels/User/MySettingsViewModel';
export { UserMySettings } from '@/User/MySettings';

// ~/bundles/User/RatedSongs
import '@/KnockoutExtensions/SlideVisible';
import '@/KnockoutExtensions/ArtistAutoComplete';
import '@/KnockoutExtensions/ScrollEnd';
//export { PVPlayersFactory } from '@/ViewModels/PVs/PVPlayersFactory';
//export { RatedSongsSearchViewModel } from '@/ViewModels/User/RatedSongsSearchViewModel';

// ~/bundles/User/RequestVerification
export { RequestVerificationViewModel } from '@/ViewModels/User/RequestVerificationViewModel';
export { UserRequestVerification } from '@/User/RequestVerification';

// ~/bundles/User/Stats
export { UserStats } from '@/User/Stats';

// ~/bundles/Venue/Details
export { VenueDetailsViewModel } from '@/ViewModels/Venue/VenueDetailsViewModel';
export { VenueDetails } from '@/Venue/Details';

// ~/bundles/Venue/Edit
export { VenueEditViewModel } from '@/ViewModels/Venue/VenueEditViewModel';
export { VenueEdit } from '@/Venue/Edit';

// ~/bundles/Venue/ViewVersion
export { VenueRepository } from '@/Repositories/VenueRepository';
export { VenueViewVersion } from '@/Venue/ViewVersion';
