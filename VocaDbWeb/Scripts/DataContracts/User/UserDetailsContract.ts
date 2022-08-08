import { PermissionToken } from '@/Models/LoginManager';
import UserGroup from '@/Models/Users/UserGroup';

import AlbumForApiContract from '../Album/AlbumForApiContract';
import ArtistApiContract from '../Artist/ArtistApiContract';
import CommentContract from '../CommentContract';
import EntryThumbContract from '../EntryThumbContract';
import SongApiContract from '../Song/SongApiContract';
import TagBaseContract from '../Tag/TagBaseContract';
import WebLinkContract from '../WebLinkContract';
import ArtistForUserForApiContract from './ArtistForUserForApiContract';
import UserKnownLanguageContract from './UserKnownLanguageContract';

export interface OldUsernameContract {
	date: Date;
	oldName: string;
}

interface UserDetailsContractBase {
	aboutMe: string;
	active: boolean;
	albumCollectionCount: number;
	anonymousActivity: boolean;
	artistCount: number;
	commentCount: number;
	createDate: Date;
	customTitle: string;
	designatedStaff: boolean;
	editCount: number;
	emailVerified: boolean;
	favoriteAlbums: AlbumForApiContract[];
	favoriteSongCount: number;
	favoriteTags: TagBaseContract[];
	followedArtists: ArtistApiContract[];
	groupId: UserGroup;
	id: number;
	isVeteran: boolean;
	knownLanguages: UserKnownLanguageContract[];
	latestComments: CommentContract[];
	latestRatedSongs: SongApiContract[];
	level: number;
	location: string;
	mainPicture?: EntryThumbContract;
	name: string;
	oldUsernames: OldUsernameContract[];
	ownedArtistEntries: ArtistForUserForApiContract[];
	possibleProducerAccount: boolean;
	power: number;
	publicAlbumCollection: boolean;
	standalone: boolean;
	submitCount: number;
	supporter: boolean;
	tagVotes: number;
	twitterName: string;
	verifiedArtist: boolean;
	webLinks: WebLinkContract[];
}

// Corresponds to the UserDetailsForApiContract in C#.
export default interface UserDetailsContract extends UserDetailsContractBase {
	additionalPermissions: PermissionToken[];
	effectivePermissions: PermissionToken[];
	email: string;
	lastLogin: Date;
	lastLoginAddress: string;
}
