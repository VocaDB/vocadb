import { AlbumDiscPropertiesContract } from '@/DataContracts/Album/AlbumDiscPropertiesContract';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { AlbumReleaseContract } from '@/DataContracts/Album/AlbumReleaseContract';
import { AlbumReviewContract } from '@/DataContracts/Album/AlbumReviewContract';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongInAlbumContract } from '@/DataContracts/Song/SongInAlbumContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { AlbumForUserForApiContract } from '@/DataContracts/User/AlbumForUserForApiContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { EntryStatus } from '@/Models/EntryStatus';

interface SharedAlbumStatsContract {
	latestReview?: AlbumReviewContract;
	latestReviewRatingScore: number;
	ownedCount: number;
	reviewCount: number;
	wishlistCount: number;
}

// Corresponds to the AlbumDetailsForApiContract record class in C#.
export interface AlbumDetailsContract {
	additionalNames: string;
	albumForUser?: AlbumForUserForApiContract;
	artistLinks: ArtistForAlbumContract[];
	artistString: string;
	canEditPersonalDescription: boolean;
	canRemoveTagUsages: boolean;
	commentCount: number;
	commentsLocked: boolean;
	createDate: string;
	deleted: boolean;
	description: EnglishTranslatedStringContract;
	discs: Record<string, AlbumDiscPropertiesContract>;
	discType: AlbumType;
	discTypeTag?: TagBaseContract;
	hits: number;
	id: number;
	latestComments: CommentContract[];
	mainPicture?: EntryThumbContract;
	mergedTo?: AlbumForApiContract;
	name: string;
	originalRelease?: AlbumReleaseContract;
	personalDescriptionAuthor?: ArtistApiContract;
	personalDescriptionText?: string;
	pictures: EntryThumbContract[];
	pvs: PVContract[];
	ratingAverage: number;
	ratingCount: number;
	songs: SongInAlbumContract[];
	stats: SharedAlbumStatsContract;
	status: EntryStatus;
	tags: TagUsageForApiContract[];
	totalLengthSeconds: number;
	version: number;
	webLinks: WebLinkContract[];
}
