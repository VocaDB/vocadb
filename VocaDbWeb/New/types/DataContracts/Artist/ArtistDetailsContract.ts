import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { CommentContract } from '@/types/DataContracts/CommentContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { ArtistType } from '@/types/Models/Artists/ArtistType';
import { EntryStatus } from '@/types/Models/EntryStatus';

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
	characterDesigner?: ArtistApiContract;
	characterDesignerOf: ArtistApiContract[];
	childVoicebanks: ArtistApiContract[];
	commentCount: number;
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
