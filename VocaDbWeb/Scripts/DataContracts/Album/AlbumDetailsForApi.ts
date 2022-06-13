import AlbumHelper from '@Helpers/AlbumHelper';
import PVHelper from '@Helpers/PVHelper';
import AlbumType from '@Models/Albums/AlbumType';
import ArtistCategories from '@Models/Artists/ArtistCategories';
import ArtistRoles from '@Models/Artists/ArtistRoles';
import ContentFocus from '@Models/ContentFocus';
import EntryStatus from '@Models/EntryStatus';
import _ from 'lodash';
import moment from 'moment';

import ArtistApiContract from '../Artist/ArtistApiContract';
import ArtistForAlbumContract from '../ArtistForAlbumContract';
import CommentContract from '../CommentContract';
import EntryThumbContract from '../EntryThumbContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import OptionalDateTimeContract from '../OptionalDateTimeContract';
import PVContract from '../PVs/PVContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';
import SongInAlbumContract from '../Song/SongInAlbumContract';
import TagBaseContract from '../Tag/TagBaseContract';
import TagUsageForApiContract from '../Tag/TagUsageForApiContract';
import { MediaType, PurchaseStatus } from '../User/AlbumForUserForApiContract';
import WebLinkContract from '../WebLinkContract';
import AlbumDetailsContract from './AlbumDetailsContract';
import AlbumDiscPropertiesContract from './AlbumDiscPropertiesContract';
import AlbumForApiContract from './AlbumForApiContract';
import AlbumReviewContract from './AlbumReviewContract';

export enum DiscMediaType {
	Audio = 'Audio',
	Video = 'Video',
}

// Corresponds to the AlbumDisc class in C#.
export class AlbumDisc {
	public readonly discNumber: number;
	public readonly isVideo: boolean;
	public readonly totalLengthSeconds: number;
	public readonly name?: string;
	public readonly songs: SongInAlbumContract[];

	public constructor(
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
			? _.sumBy(this.songs, (s) => s.song.lengthSeconds)
			: 0;
	}
}

// Corresponds to the AlbumDetails class in C#.
export default class AlbumDetailsForApi {
	public readonly additionalNames: string;
	public readonly albumMediaType: MediaType;
	public readonly albumPurchaseStatus: PurchaseStatus;
	public readonly collectionRating: number;
	public readonly artistString: string;
	public readonly bands: ArtistForAlbumContract[];
	public readonly canEditPersonalDescription: boolean;
	public readonly canRemoveTagUsages: boolean;
	public readonly catNum?: string;
	public readonly circles: ArtistForAlbumContract[];
	public readonly commentCount: number;
	public readonly contentFocus: ContentFocus;
	public readonly createDate: Date;
	public readonly deleted: boolean;
	public readonly description: EnglishTranslatedStringContract;
	public readonly discs: AlbumDisc[];
	public readonly discType: AlbumType;
	public readonly discTypeTag?: TagBaseContract;
	public readonly draft: boolean;
	public readonly fullReleaseDate?: Date;
	public readonly hits: number;
	public readonly id: number;
	public readonly illustrators?: ArtistForAlbumContract[];
	public readonly labels: ArtistForAlbumContract[];
	public readonly latestComments: CommentContract[];
	public readonly latestReview?: AlbumReviewContract;
	public readonly latestReviewRatingScore: number;
	public readonly mainPicture?: EntryThumbContract;
	public readonly mergedTo?: AlbumForApiContract;
	public readonly name: string;
	public readonly otherArtists: ArtistForAlbumContract[];
	public readonly ownedBy: number;
	public readonly personalDescriptionAuthor?: ArtistApiContract;
	public readonly personalDescriptionText?: string;
	public readonly pictures: EntryThumbContract[];
	public readonly primaryPV?: PVContract;
	public readonly producers: ArtistForAlbumContract[];
	public readonly pvs: PVContract[];
	public readonly ratingAverage: number;
	public readonly ratingCount: number;
	public readonly releaseDate?: OptionalDateTimeContract;
	public readonly releaseEvent?: ReleaseEventContract;
	public readonly reviewCount: number;
	public readonly status: string /* TODO: enum */;
	public readonly subject: ArtistForAlbumContract[];
	public readonly tags: TagUsageForApiContract[];
	public readonly totalLength: number;
	public readonly userHasAlbum: boolean;
	public readonly version: number;
	public readonly vocalists: ArtistForAlbumContract[];
	public readonly webLinks: WebLinkContract[];
	public readonly wishlistedBy: number;

	public constructor(public readonly contract: AlbumDetailsContract) {
		this.additionalNames = contract.additionalNames;
		this.artistString = contract.artistString;
		this.canEditPersonalDescription = contract.canEditPersonalDescription;
		this.canRemoveTagUsages = contract.canRemoveTagUsages;
		this.commentCount = contract.commentCount;
		this.createDate = contract.createDate;
		this.deleted = contract.deleted;
		this.description = contract.description;
		this.discType = contract.discType;
		this.discTypeTag = contract.discTypeTag;
		this.draft = contract.status === EntryStatus[EntryStatus.Draft];
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

		const songsByDiscs = _.groupBy(contract.songs, (s) => s.discNumber);
		this.discs = Object.entries(songsByDiscs).map(
			([key, value]) =>
				new AlbumDisc(
					Number(key),
					value,
					_.has(contract.discs, key) ? contract.discs[key] : undefined,
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
			this.releaseDate = contract.originalRelease.releaseDate;
			this.fullReleaseDate =
				this.releaseDate &&
				this.releaseDate.year &&
				this.releaseDate.month &&
				this.releaseDate.day
					? moment
							.utc([
								this.releaseDate.year,
								this.releaseDate.month - 1,
								this.releaseDate.day,
							])
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

		this.primaryPV = PVHelper.primaryPV(this.pvs);
	}

	public get jsonModel(): AlbumDetailsAjax {
		return new AlbumDetailsAjax(this);
	}

	public get releaseDateIsInTheFarFuture(): boolean {
		return (
			!!this.fullReleaseDate &&
			this.fullReleaseDate > moment.utc().add(7, 'd').toDate()
		);
	}

	public get releaseDateIsInTheNearFuture(): boolean {
		return (
			!!this.fullReleaseDate &&
			this.fullReleaseDate > moment.utc().toDate() &&
			this.fullReleaseDate <= moment.utc().add(7, 'd').toDate()
		);
	}

	public get releaseDateIsInThePast(): boolean {
		return (
			!!this.fullReleaseDate && this.fullReleaseDate <= moment.utc().toDate()
		);
	}

	public get showProducerRoles(): boolean {
		return (
			this.producers.length > 1 &&
			_.some(
				this.producers,
				(p) =>
					p.roles !== ArtistRoles[ArtistRoles.Default] &&
					p.roles !== ArtistRoles[ArtistRoles.Composer],
			)
		);
	}
}

export class AlbumDetailsAjax {
	public readonly albumMediaType: MediaType;
	public readonly albumPurchaseStatus: PurchaseStatus;
	public readonly collectionRating: number;
	public readonly id: number;
	public readonly latestComments: CommentContract[];
	public readonly personalDescriptionAuthor?: ArtistApiContract;
	public readonly personalDescriptionText?: string;
	public readonly tagUsages: TagUsageForApiContract[];
	public readonly userHasAlbum: boolean;

	public constructor(model: AlbumDetailsForApi) {
		this.albumMediaType = model.albumMediaType;
		this.albumPurchaseStatus = model.albumPurchaseStatus;
		this.collectionRating = model.collectionRating;
		this.id = model.id;
		this.latestComments = model.latestComments;
		this.personalDescriptionAuthor = model.personalDescriptionAuthor;
		this.personalDescriptionText = model.personalDescriptionText;
		this.tagUsages = model.tags;
		this.userHasAlbum = model.userHasAlbum;
	}
}
