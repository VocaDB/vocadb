/* eslint-disable prettier/prettier */
/* eslint-disable import/first */

// ~/bundles/shared/common
export { setLanguagePreferenceCookie } from '@/Shared/TopBar';
export { ui } from '@/Shared/MessagesTyped';
export { functions } from '@/Shared/GlobalFunctions';
export { HttpClient } from '@/Shared/HttpClient';
export { UrlMapper } from '@/Shared/UrlMapper';
export { EntryReportRepository } from '@/Repositories/EntryReportRepository';
export { UserRepository } from '@/Repositories/UserRepository';

// ~/bundles/shared/main
export { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
export { RepositoryFactory } from '@/Repositories/RepositoryFactory';
export { TagRepository } from '@/Repositories/TagRepository';
export { ResourceRepository } from '@/Repositories/ResourceRepository';
export { SongRepository } from '@/Repositories/SongRepository';
export { ArtistRepository } from '@/Repositories/ArtistRepository';

// ~/bundles/shared/edit
export { DialogService } from '@/Shared/DialogService';

// ~/bundles/Home/Index

// ~/bundles/ActivityEntry/Index
//export { ResourceRepository } from '@/Repositories/ResourceRepository';

// ~/bundles/Admin/ManageEntryTagMappings

// ~/bundles/Admin/ManageTagMappings

// ~/bundles/Admin/ManageIPRules
export { AdminRepository } from '@/Repositories/AdminRepository';

// ~/bundles/Admin/ManageWebhooks

// ~/bundles/Admin/PVsByAuthor
export { AdminPVsByAuthor } from '@/Admin/PVsByAuthor';

// ~/bundles/Admin/ViewAuditLog

// ~/bundles/Album/Create

// ~/bundles/Album/Deleted
export { AlbumRepository } from '@/Repositories/AlbumRepository';

// ~/bundles/Album/Details

// ~/bundles/Album/Edit

// ~/bundles/Album/Merge
//export { AlbumRepository } from '@/Repositories/AlbumRepository';

// ~/bundles/Album/ViewVersion
//export { AlbumRepository } from '@/Repositories/AlbumRepository';

// ~/bundles/Artist/Create

// ~/bundles/Artist/Details

// ~/bundles/Artist/Edit

// ~/bundles/Artist/Merge

// ~/bundles/Artist/ViewVersion

// ~/bundles/Comment/Index

// ~/bundles/Comment/CommentsByUser

// ~/bundles/Discussion/Index

// ~/bundles/EventSeries/Details

// ~/bundles/EventSeries/Edit
export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';

// ~/bundles/EventSeries/ViewVersion
//export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';

// ~/bundles/MikuDbAlbum/Index
export { MikuDbAlbumIndex } from '@/MikuDbAlbum/Index';

// ~/bundles/MikuDbAlbum/PrepareForImport
export { MikuDbAlbumPrepareForImport } from '@/MikuDbAlbum/PrepareForImport';

// ~/bundles/ReleaseEvent/Details
//export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';

// ~/bundles/ReleaseEvent/Edit

// ~/bundles/ReleaseEvent/EventsBySeries
export { EventEventsBySeries } from '@/Event/EventsBySeries';

// ~/bundles/ReleaseEvent/EventsByVenue
export { EventEventsByVenue } from '@/Event/EventsByVenue';

// ~/bundles/ReleaseEvent/Index
export { EventIndex } from '@/Event/Index';

// ~/bundles/ReleaseEvent/ViewVersion
//export { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';

// ~/bundles/Search/Index

// ~/bundles/Song/Create

// ~/bundles/Song/Details

// ~/bundles/Song/Edit

// ~/bundles/Song/Merge

// ~/bundles/Song/TopRated

// ~/bundles/Song/ViewVersion

// ~/bundles/SongList/Details

// ~/bundles/SongList/Edit

// ~/bundles/SongList/Featured

// ~/bundles/SongList/Import

// ~/bundles/Stats/Index

// ~/bundles/Tag/Details
//export { TagRepository } from '@/Repositories/TagRepository';

// ~/bundles/Tag/Edit
//export { TagRepository } from '@/Repositories/TagRepository';

// ~/bundles/Tag/Index
//export { TagRepository } from '@/Repositories/TagRepository';

// ~/bundles/Tag/Merge
//export { TagRepository } from '@/Repositories/TagRepository';

// ~/bundles/Tag/ViewVersion
//export { TagRepository } from '@/Repositories/TagRepository';
//export { ArchivedEntryViewModel } from '@/ViewModels/ArchivedEntryViewModel';

// ~/bundles/User/AlbumCollection

// ~/bundles/User/Details

// ~/bundles/User/Edit
export { UserEdit } from '@/User/Edit';

// ~/bundles/User/EntryEdits

// ~/bundles/User/FavoriteSongs

// ~/bundles/User/Index

// ~/bundles/User/Messages

// ~/bundles/User/MySettings

// ~/bundles/User/RatedSongs

// ~/bundles/User/RequestVerification

// ~/bundles/User/Stats
export { UserStats } from '@/User/Stats';

// ~/bundles/Venue/Details

// ~/bundles/Venue/Edit

// ~/bundles/Venue/ViewVersion
export { VenueRepository } from '@/Repositories/VenueRepository';
