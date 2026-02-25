import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { UserKnownLanguageContract } from '@/DataContracts/User/UserKnownLanguageContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { PermissionToken } from '@/Models/LoginManager';
import { UserGroup } from '@/Models/Users/UserGroup';

export interface OldUsernameContract {
	date: string;
	oldName: string;
}

interface UserDetailsContractBase {
	aboutMe: string;
	active: boolean;
	albumCollectionCount: number;
	anonymousActivity: boolean;
	artistCount: number;
	commentCount: number;
	commentsLocked: boolean;
	createDate: string;
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
	oldUsernames?: OldUsernameContract[];
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
export interface UserDetailsContract extends UserDetailsContractBase {
	additionalPermissions: PermissionToken[];
	effectivePermissions: PermissionToken[];
	email: string;
	lastLogin: string;
	lastLoginAddress: string;
}
