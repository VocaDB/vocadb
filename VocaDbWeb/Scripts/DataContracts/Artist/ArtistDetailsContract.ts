import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryStatus } from '@/Models/EntryStatus';

interface TopStatContract<T> {
	count: number;
	data: T;
}

interface AdvancedArtistStatsContract {
	topVocaloids: TopStatContract<ArtistApiContract>[];
	topLanguages: TopStatContract<string>[];
}

interface PersonalArtistStatsContract {
	songRatingCount: number;
}

interface SharedArtistStatsContract {
	albumCount: number;
	albumRatingAverage: number;
	eventCount: number;
	followerCount: number;
	ratedAlbumCount: number;
	ratedSongCount: number;
	songCount: number;
}

// Corresponds to the ArtistDetailsForApiContract record class in C#.
export interface ArtistDetailsContract {
	additionalNames: string;
	advancedStats?: AdvancedArtistStatsContract;
	artistType: ArtistType;
	artistTypeTag: TagBaseContract;
	baseVoicebank?: ArtistApiContract;
	canRemoveTagUsages: boolean;
	characterDesigners: ArtistApiContract[];
	characterDesignerOf: ArtistApiContract[];
	childVoicebanks: ArtistApiContract[];
	commentCount: number;
	commentsLocked: boolean;
	createDate: string;
	cultureCodes: string[];
	deleted: boolean;
	description: EnglishTranslatedStringContract;
	draft: boolean;
	emailNotifications: boolean;
	groups: ArtistApiContract[];
	id: number;
	illustrators: ArtistApiContract[];
	illustratorOf: ArtistApiContract[];
	isAdded: boolean;
	latestAlbums: AlbumForApiContract[];
	latestComments: CommentContract[];
	latestEvents: ReleaseEventContract[];
	latestSongs: SongApiContract[];
	mainPicture?: EntryThumbContract;
	managerOf: ArtistApiContract[];
	managers: ArtistApiContract[];
	members: ArtistApiContract[];
	mergedTo?: ArtistApiContract;
	name: string;
	ownerUsers: UserApiContract[];
	personalStats?: PersonalArtistStatsContract;
	pictures: EntryThumbContract[];
	releaseDate?: string;
	sharedStats: SharedArtistStatsContract;
	siteNotifications: boolean;
	status: EntryStatus;
	tags: TagUsageForApiContract[];
	topAlbums: AlbumForApiContract[];
	topSongs: SongApiContract[];
	version: number;
	voicebanks: ArtistApiContract[];
	voiceProviders: ArtistApiContract[];
	webLinks: WebLinkContract[];
}
