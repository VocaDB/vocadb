import ArtistApiContract from '../Artist/ArtistApiContract';
import ArtistForAlbumContract from '../ArtistForAlbumContract';
import CommentContract from '../CommentContract';
import EntryThumbContract from '../EntryThumbContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import PVContract from '../PVs/PVContract';
import SongInAlbumContract from '../Song/SongInAlbumContract';
import TagBaseContract from '../Tag/TagBaseContract';
import TagUsageForApiContract from '../Tag/TagUsageForApiContract';
import AlbumForUserForApiContract from '../User/AlbumForUserForApiContract';
import WebLinkContract from '../WebLinkContract';
import AlbumDiscPropertiesContract from './AlbumDiscPropertiesContract';
import AlbumForApiContract from './AlbumForApiContract';
import AlbumReleaseContract from './AlbumReleaseContract';
import AlbumReviewContract from './AlbumReviewContract';

interface SharedAlbumStatsContract {
	latestReview?: AlbumReviewContract;
	latestReviewRatingScore: number;
	ownedCount: number;
	reviewCount: number;
	wishlistCount: number;
}

// Corresponds to the AlbumDetailsForApiContract record class in C#.
export default interface AlbumDetailsContract {
	additionalNames: string;
	albumForUser?: AlbumForUserForApiContract;
	artistLinks: ArtistForAlbumContract[];
	artistString: string;
	canEditPersonalDescription: boolean;
	canRemoveTagUsages: boolean;
	commentCount: number;
	createDate: Date;
	deleted: boolean;
	description: EnglishTranslatedStringContract;
	discs: Record<string, AlbumDiscPropertiesContract>;
	discType: string /* TODO: enum */;
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
	status: string /* TODO: enum */;
	tags: TagUsageForApiContract[];
	totalLengthSeconds: number;
	version: number;
	webLinks: WebLinkContract[];
}
