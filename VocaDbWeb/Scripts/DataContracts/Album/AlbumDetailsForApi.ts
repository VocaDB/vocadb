import { AlbumDetailsContract } from '@/DataContracts/Album/AlbumDetailsContract';
import { AlbumDiscPropertiesContract } from '@/DataContracts/Album/AlbumDiscPropertiesContract';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { AlbumReviewContract } from '@/DataContracts/Album/AlbumReviewContract';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { OptionalDateTimeContract } from '@/DataContracts/OptionalDateTimeContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongInAlbumContract } from '@/DataContracts/Song/SongInAlbumContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import {
	MediaType,
	PurchaseStatus,
} from '@/DataContracts/User/AlbumForUserForApiContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { AlbumHelper } from '@/Helpers/AlbumHelper';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ArtistCategories } from '@/Models/Artists/ArtistCategories';
import { ArtistRoles } from '@/Models/Artists/ArtistRoles';
import { ContentFocus } from '@/Models/ContentFocus';
import { EntryStatus } from '@/Models/EntryStatus';
import dayjs from 'dayjs';
import UTC from 'dayjs/plugin/utc';
import { has } from 'lodash-es';

dayjs.extend(UTC);

export enum DiscMediaType {
	Audio = 'Audio',
	Video = 'Video',
}

// Corresponds to the AlbumDisc class in C#.
export class AlbumDisc {
	readonly discNumber: number;
	readonly isVideo: boolean;
	readonly totalLengthSeconds: number;
	readonly name?: string;
	readonly songs: SongInAlbumContract[];

	constructor(
		discNumber: number,
		songs: SongInAlbumContract[],
		discProperties?: AlbumDiscPropertiesContract,
	) {
		this.discNumber = discNumber;
		this.songs = songs;

		this.isVideo =
			!!discProperties && discProperties.mediaType === DiscMediaType.Video;
		this.name = discProperties?.name;
		this.totalLengthSeconds = this.songs.every(
			(s) => s.song && s.song.lengthSeconds > 0,
		)
			? this.songs.sumBy((s) => s.song!.lengthSeconds)
			: 0;
	}
}

// Corresponds to the AlbumDetails class in C#.
export class AlbumDetailsForApi {
	readonly additionalNames: string;
	readonly albumMediaType: MediaType;
	readonly albumPurchaseStatus: PurchaseStatus;
	readonly collectionRating: number;
	readonly artistString: string;
	readonly bands: ArtistForAlbumContract[];
	readonly canEditPersonalDescription: boolean;
	readonly canRemoveTagUsages: boolean;
	readonly catNum?: string;
	readonly circles: ArtistForAlbumContract[];
	readonly commentCount: number;
	readonly contentFocus: ContentFocus;
	readonly createDate: string;
	readonly cultureCodes: string[];
	readonly deleted: boolean;
	readonly description: EnglishTranslatedStringContract;
	readonly discs: AlbumDisc[];
	readonly discType: AlbumType;
	readonly discTypeTag?: TagBaseContract;
	readonly draft: boolean;
	readonly fullReleaseDate?: Date;
	readonly hits: number;
	readonly id: number;
	readonly illustrators?: ArtistForAlbumContract[];
	readonly labels: ArtistForAlbumContract[];
	readonly latestComments: CommentContract[];
	readonly latestReview?: AlbumReviewContract;
	readonly latestReviewRatingScore: number;
	readonly mainPicture?: EntryThumbContract;
	readonly mergedTo?: AlbumForApiContract;
	readonly name: string;
	readonly otherArtists: ArtistForAlbumContract[];
	readonly ownedBy: number;
	readonly personalDescriptionAuthor?: ArtistApiContract;
	readonly personalDescriptionText?: string;
	readonly pictures: EntryThumbContract[];
	readonly primaryPV?: PVContract;
	readonly producers: ArtistForAlbumContract[];
	readonly pvs: PVContract[];
	readonly ratingAverage: number;
	readonly ratingCount: number;
	readonly releaseDate?: OptionalDateTimeContract;
	readonly releaseEvent?: ReleaseEventContract;
	readonly releaseEvents: ReleaseEventContract[] = [];
	readonly reviewCount: number;
	readonly status: EntryStatus;
	readonly subject: ArtistForAlbumContract[];
	readonly tags: TagUsageForApiContract[];
	readonly totalLength: number;
	readonly userHasAlbum: boolean;
	readonly version: number;
	readonly vocalists: ArtistForAlbumContract[];
	readonly webLinks: WebLinkContract[];
	readonly wishlistedBy: number;

	constructor(
		readonly contract: AlbumDetailsContract,
		primaryPV: PVContract | undefined,
	) {
		this.additionalNames = contract.additionalNames;
		this.artistString = contract.artistString;
		this.canEditPersonalDescription = contract.canEditPersonalDescription;
		this.canRemoveTagUsages = contract.canRemoveTagUsages;
		this.commentCount = contract.commentCount;
		this.createDate = contract.createDate;
		this.cultureCodes = contract.songs.flatMap(
			(s) => s.computedCultureCodes ?? [],
		);
		this.deleted = contract.deleted;
		this.description = contract.description;
		this.discType = contract.discType;
		this.discTypeTag = contract.discTypeTag;
		this.draft = contract.status === EntryStatus.Draft;
		this.hits = contract.hits;
		this.id = contract.id;
		this.latestComments = contract.latestComments;
		this.latestReview = contract.stats.latestReview;
		this.latestReviewRatingScore = contract.stats.latestReviewRatingScore;
		this.mainPicture = contract.mainPicture;
		this.mergedTo = contract.mergedTo;
		this.name = contract.name;
		this.ownedBy = contract.stats.ownedCount;
		this.personalDescriptionAuthor = contract.personalDescriptionAuthor;
		this.personalDescriptionText = contract.personalDescriptionText;
		this.pictures = contract.pictures;
		this.pvs = contract.pvs;
		this.ratingAverage = contract.ratingAverage;
		this.ratingCount = contract.ratingCount;
		this.reviewCount = contract.stats.reviewCount;
		this.status = contract.status;
		this.tags = contract.tags;
		this.totalLength = contract.totalLengthSeconds;
		this.userHasAlbum = !!contract.albumForUser;
		this.version = contract.version;
		this.webLinks = contract.webLinks;
		this.wishlistedBy = contract.stats.wishlistCount;

		const songsByDiscs = contract.songs.groupBy((s) => s.discNumber);
		this.discs = Object.entries(songsByDiscs).map(
			([key, value]) =>
				new AlbumDisc(
					Number(key),
					value,
					has(contract.discs, key) ? contract.discs[key] : undefined,
				),
		);

		if (contract.albumForUser) {
			this.albumMediaType = contract.albumForUser.mediaType;
			this.albumPurchaseStatus = contract.albumForUser.purchaseStatus;
			this.collectionRating = contract.albumForUser.rating;
		} else {
			this.albumMediaType = MediaType.Other;
			this.albumPurchaseStatus = PurchaseStatus.Nothing;
			this.collectionRating = 0;
		}

		if (contract.originalRelease) {
			this.catNum = contract.originalRelease.catNum;
			this.releaseEvent = contract.originalRelease.releaseEvent;
			this.releaseEvents = contract.originalRelease.releaseEvents;
			this.releaseDate = contract.originalRelease.releaseDate;
			this.fullReleaseDate =
				this.releaseDate &&
				this.releaseDate.year &&
				this.releaseDate.month &&
				this.releaseDate.day
					? dayjs
							.utc()
							.year(this.releaseDate.year)
							.month(this.releaseDate.month - 1)
							.date(this.releaseDate.day)
							.toDate()
					: undefined;
		}

		this.contentFocus = AlbumHelper.getContentFocus(this.discType);

		this.bands = contract.artistLinks.filter((artist) =>
			artist
				.categories!.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Band),
		);

		this.circles = contract.artistLinks.filter((artist) =>
			artist
				.categories!.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Circle),
		);

		this.illustrators =
			this.contentFocus === ContentFocus.Illustration
				? contract.artistLinks.filter((artist) =>
						artist
							.categories!.split(',')
							.map((category) => category.trim())
							.includes(ArtistCategories.Illustrator),
				  )
				: undefined;

		this.labels = contract.artistLinks.filter((artist) =>
			artist
				.categories!.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Label),
		);

		this.producers = contract.artistLinks.filter((artist) =>
			artist
				.categories!.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Producer),
		);

		this.vocalists = contract.artistLinks.filter((artist) =>
			artist
				.categories!.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Vocalist),
		);

		this.subject = contract.artistLinks.filter((artist) =>
			artist
				.categories!.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Subject),
		);

		// OPTIMIZE
		this.otherArtists = contract.artistLinks.filter(
			(artist) =>
				artist
					.categories!.split(',')
					.map((category) => category.trim())
					.includes(ArtistCategories.Other) ||
				artist
					.categories!.split(',')
					.map((category) => category.trim())
					.includes(ArtistCategories.Animator) ||
				(this.contentFocus !== ContentFocus.Illustration &&
					artist
						.categories!.split(',')
						.map((category) => category.trim())
						.includes(ArtistCategories.Illustrator)),
		);

		this.primaryPV = primaryPV;
	}

	get jsonModel(): AlbumDetailsAjax {
		return new AlbumDetailsAjax(this);
	}

	get formattedReleaseDate(): string {
		return DateTimeHelper.formatComponentDate(
			this.releaseDate?.year,
			this.releaseDate?.month,
			this.releaseDate?.day,
		);
	}

	get releaseDateIsInTheFarFuture(): boolean {
		return (
			!!this.fullReleaseDate &&
			this.fullReleaseDate > dayjs.utc().add(7, 'd').toDate()
		);
	}

	get releaseDateIsInTheNearFuture(): boolean {
		return (
			!!this.fullReleaseDate &&
			this.fullReleaseDate > dayjs.utc().toDate() &&
			this.fullReleaseDate <= dayjs.utc().add(7, 'd').toDate()
		);
	}

	get releaseDateIsInThePast(): boolean {
		return (
			!!this.fullReleaseDate && this.fullReleaseDate <= dayjs.utc().toDate()
		);
	}

	get showProducerRoles(): boolean {
		return (
			this.producers.length > 1 &&
			this.producers.some(
				(p) =>
					p.roles !== ArtistRoles[ArtistRoles.Default] &&
					p.roles !== ArtistRoles[ArtistRoles.Composer],
			)
		);
	}
}

export class AlbumDetailsAjax {
	readonly albumMediaType: MediaType;
	readonly albumPurchaseStatus: PurchaseStatus;
	readonly collectionRating: number;
	readonly id: number;
	readonly latestComments: CommentContract[];
	readonly personalDescriptionAuthor?: ArtistApiContract;
	readonly personalDescriptionText?: string;
	readonly tagUsages: TagUsageForApiContract[];
	readonly userHasAlbum: boolean;
	readonly discType: AlbumType;

	constructor(model: AlbumDetailsForApi) {
		this.albumMediaType = model.albumMediaType;
		this.albumPurchaseStatus = model.albumPurchaseStatus;
		this.collectionRating = model.collectionRating;
		this.id = model.id;
		this.latestComments = model.latestComments;
		this.personalDescriptionAuthor = model.personalDescriptionAuthor;
		this.personalDescriptionText = model.personalDescriptionText;
		this.tagUsages = model.tags;
		this.userHasAlbum = model.userHasAlbum;
		this.discType = model.discType;
	}
}
