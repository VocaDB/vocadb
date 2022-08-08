import ArtistType from '@/Models/Artists/ArtistType';

import AlbumForApiContract from '../Album/AlbumForApiContract';
import CommentContract from '../CommentContract';
import EntryThumbContract from '../EntryThumbContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';
import SongApiContract from '../Song/SongApiContract';
import TagBaseContract from '../Tag/TagBaseContract';
import TagUsageForApiContract from '../Tag/TagUsageForApiContract';
import UserApiContract from '../User/UserApiContract';
import WebLinkContract from '../WebLinkContract';
import ArtistApiContract from './ArtistApiContract';

interface TopStatContract<T> {
	count: number;
	data: T;
}

interface AdvancedArtistStatsContract {
	topVocaloids: TopStatContract<ArtistApiContract>[];
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
export default interface ArtistDetailsContract {
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
	createDate: Date;
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
	releaseDate?: Date;
	sharedStats: SharedArtistStatsContract;
	siteNotifications: boolean;
	status: string;
	tags: TagUsageForApiContract[];
	topAlbums: AlbumForApiContract[];
	topSongs: SongApiContract[];
	version: number;
	voicebanks: ArtistApiContract[];
	voiceProviders: ArtistApiContract[];
	webLinks: WebLinkContract[];
}
