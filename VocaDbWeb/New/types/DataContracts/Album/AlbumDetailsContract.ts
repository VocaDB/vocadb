import { AlbumDiscPropertiesContract } from '@/types/DataContracts/Album/AlbumDiscPropertiesContract';
import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { AlbumReleaseContract } from '@/types/DataContracts/Album/AlbumReleaseContract';
import { AlbumReviewContract } from '@/types/DataContracts/Album/AlbumReviewContract';
import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { ArtistForAlbumContract } from '@/types/DataContracts/ArtistForAlbumContract';
import { CommentContract } from '@/types/DataContracts/CommentContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongInAlbumContract } from '@/types/DataContracts/Song/SongInAlbumContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { AlbumForUserForApiContract } from '@/types/DataContracts/User/AlbumForUserForApiContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { AlbumType } from '@/types/Models/Albums/AlbumType';
import { EntryStatus } from '@/types/Models/EntryStatus';

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
