import { AlbumDetailsAjax } from '@/DataContracts/Album/AlbumDetailsForApi';
import { AlbumReviewContract } from '@/DataContracts/Album/AlbumReviewContract';
import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import {
	AlbumForUserForApiContract,
	MediaType,
	PurchaseStatus,
} from '@/DataContracts/User/AlbumForUserForApiContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { EntryType } from '@/Models/EntryType';
import { LoginManager } from '@/Models/LoginManager';
import {
	AlbumOptionalField,
	AlbumRepository,
} from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { functions } from '@/Shared/GlobalFunctions';
import { GlobalValues } from '@/Shared/GlobalValues';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { EnglishTranslatedStringStore } from '@/Stores/Globalization/EnglishTranslatedStringStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { SelfDescriptionStore } from '@/Stores/SelfDescriptionStore';
import { TagListStore } from '@/Stores/Tag/TagListStore';
import { TagsEditStore } from '@/Stores/Tag/TagsEditStore';
import $ from 'jquery';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export class DownloadTagsStore {
	@observable dialogVisible = false;
	@observable formatString: string;

	constructor(readonly albumId: number, formatString: string) {
		makeObservable(this);

		this.formatString = formatString;
	}

	@action show = (): void => {
		this.dialogVisible = true;
	};
}

export class EditCollectionStore {
	@observable dialogVisible = false;
	@observable albumMediaType;
	@observable albumPurchaseStatus;
	@observable collectionRating;

	constructor(
		readonly albumId: number,
		albumMediaType: MediaType,
		albumPurchaseStatus: PurchaseStatus,
		collectionRating: number,
	) {
		makeObservable(this);

		this.albumMediaType = albumMediaType;
		this.albumPurchaseStatus = albumPurchaseStatus;
		this.collectionRating = collectionRating;
	}
}

export class AlbumReviewStore {
	readonly date: string;
	@observable editedTitle = '';
	@observable editedText = '';
	readonly id?: number;
	@observable languageCode: string;
	@observable text: string;
	@observable title: string;
	readonly user: UserApiContract;

	constructor(
		contract: AlbumReviewContract,
		readonly canBeDeleted: boolean,
		readonly canBeEdited: boolean,
	) {
		makeObservable(this);

		this.date = contract.date;
		this.id = contract.id;
		this.languageCode = contract.languageCode;
		this.text = contract.text;
		this.title = contract.title;
		this.user = contract.user;
	}

	@action beginEdit = (): void => {
		this.editedTitle = this.title;
		this.editedText = this.text;
	};

	@action saveChanges = (): void => {
		this.text = this.editedText;
		this.title = this.editedTitle;
	};

	toContract = (): AlbumReviewContract => {
		return {
			date: new Date(this.date).toISOString(),
			id: this.id,
			languageCode: this.languageCode,
			text: this.text,
			title: this.title,
			user: this.user,
		};
	};
}

export class AlbumReviewsStore {
	@observable editReviewStore?: AlbumReviewStore;
	@observable languageCode = '';
	@observable newReviewText = '';
	@observable newReviewTitle = '';
	@observable reviews: AlbumReviewStore[] = [];
	@observable showCreateNewReview = false;
	@observable writeReview = false;
	@observable private userRatings: AlbumForUserForApiContract[] = [];

	constructor(
		private readonly albumRepo: AlbumRepository,
		private readonly albumId: number,
		private readonly canDeleteAllComments: boolean,
		private readonly canEditAllComments: boolean,
		private readonly loggedUserId?: number,
	) {
		makeObservable(this);
	}

	@computed get reviewAlreadySubmitted(): boolean {
		return this.reviews.some(
			(review) =>
				review.user.id === this.loggedUserId &&
				review.languageCode === this.languageCode,
		);
	}

	@action beginEditReview = (review: AlbumReviewStore): void => {
		review.beginEdit();
		this.editReviewStore = review;
	};

	@action cancelEditReview = (): void => {
		this.editReviewStore = undefined;
	};

	private canDeleteReview = (comment: AlbumReviewContract): boolean => {
		// If one can edit they can also delete
		return (
			this.canDeleteAllComments ||
			this.canEditAllComments ||
			(comment.user && comment.user.id === this.loggedUserId)
		);
	};

	private canEditReview = (comment: AlbumReviewContract): boolean => {
		return (
			this.canEditAllComments ||
			(comment.user && comment.user.id === this.loggedUserId)
		);
	};

	@action createNewReview = async (): Promise<void> => {
		const contract = {
			date: new Date().toLocaleDateString(),
			languageCode: this.languageCode,
			text: this.newReviewText,
			title: this.newReviewTitle,
			user: { id: this.loggedUserId! },
		};
		this.newReviewText = '';
		this.newReviewTitle = '';
		this.showCreateNewReview = false;
		this.languageCode = '';
		const result = await this.albumRepo.createOrUpdateReview({
			albumId: this.albumId,
			reviewContract: contract,
		});
		runInAction(() => {
			this.reviews.push(
				new AlbumReviewStore(
					result,
					this.canDeleteReview(result),
					this.canEditReview(result),
				),
			);
		});
	};

	@action deleteReview = (review: AlbumReviewStore): Promise<void> => {
		pull(this.reviews, review);

		return this.albumRepo.deleteReview({
			albumId: this.albumId,
			reviewId: review.id!,
		});
	};

	getRatingForUser = (userId: number): number => {
		return this.userRatings
			.filter(
				(rating) =>
					!!rating.user && rating.user.id === userId && !!rating.rating,
			)
			.map((rating) => rating.rating)
			.take(1)[0];
	};

	ratingStars = (userRating: number): { enabled: boolean }[] => {
		const ratings = [1, 2, 3, 4, 5].map((rating) => ({
			enabled: Math.round(userRating) >= rating,
		}));
		return ratings;
	};

	@action saveEditedReview = (): void => {
		if (!this.editReviewStore) return;

		this.editReviewStore.saveChanges();
		const editedContract = this.editReviewStore.toContract();

		this.albumRepo.createOrUpdateReview({
			albumId: this.albumId,
			reviewContract: editedContract,
		});

		this.editReviewStore = undefined;
	};

	loadReviews = async (): Promise<void> => {
		const [reviews, ratings] = await Promise.all([
			this.albumRepo.getReviews({ albumId: this.albumId }),
			this.albumRepo.getUserCollections({ albumId: this.albumId }),
		]);
		const reviewStores = reviews.map(
			(review) =>
				new AlbumReviewStore(
					review,
					this.canDeleteReview(review),
					this.canEditReview(review),
				),
		);
		runInAction(() => {
			this.reviews = reviewStores;
			this.userRatings = ratings;
		});
	};
}

export class AlbumDetailsStore {
	readonly comments: EditableCommentsStore;
	readonly downloadTagsDialog: DownloadTagsStore;
	readonly editCollectionDialog: EditCollectionStore;
	private readonly id: number;
	readonly reportStore: ReportEntryStore;
	readonly description: EnglishTranslatedStringStore;
	readonly personalDescription: SelfDescriptionStore;
	readonly reviewsStore: AlbumReviewsStore;
	readonly tagsEditStore: TagsEditStore;
	readonly tagUsages: TagListStore;
	@observable userHasAlbum;
	@observable usersContent?: string;
	@observable userCollectionsPopupVisible = false;

	constructor(
		values: GlobalValues,
		loginManager: LoginManager,
		albumRepo: AlbumRepository,
		userRepo: UserRepository,
		artistRepo: ArtistRepository,
		data: AlbumDetailsAjax,
		canDeleteAllComments: boolean,
		formatString: string,
		showTranslatedDescription: boolean,
	) {
		makeObservable(this);

		this.id = data.id;
		this.userHasAlbum = data.userHasAlbum;
		this.downloadTagsDialog = new DownloadTagsStore(this.id, formatString);
		this.editCollectionDialog = new EditCollectionStore(
			this.id,
			data.albumMediaType,
			data.albumPurchaseStatus,
			data.collectionRating,
		);
		this.description = new EnglishTranslatedStringStore(
			showTranslatedDescription,
		);
		this.comments = new EditableCommentsStore(
			loginManager,
			albumRepo,
			this.id,
			canDeleteAllComments,
			canDeleteAllComments,
			false,
			data.latestComments,
			true,
		);

		this.personalDescription = new SelfDescriptionStore(
			values,
			data.personalDescriptionAuthor,
			data.personalDescriptionText,
			artistRepo,
			() =>
				albumRepo
					.getOneWithComponents({
						id: this.id,
						fields: [AlbumOptionalField.Artists],
						lang: values.languagePreference,
					})
					.then((result) => {
						const artists = (result.artists ?? [])
							.filter(ArtistHelper.isValidForPersonalDescription)
							.map((a) => a.artist!);
						return artists;
					}),
			(store) =>
				albumRepo.updatePersonalDescription({
					albumId: this.id,
					text: store.text!,
					author: store.author.entry!,
				}),
		);

		this.tagsEditStore = new TagsEditStore(
			{
				getTagSelections: (): Promise<TagSelectionContract[]> =>
					userRepo.getAlbumTagSelections({ albumId: this.id }),
				saveTagSelections: (tags): Promise<void> =>
					userRepo
						.updateAlbumTags({ albumId: this.id, tags: tags })
						.then(this.tagUsages.updateTagUsages),
			},
			EntryType.Album,
			() => albumRepo.getTagSuggestions({ albumId: this.id }),
		);

		this.tagUsages = new TagListStore(data.tagUsages);

		this.reportStore = new ReportEntryStore((reportType, notes) => {
			return albumRepo.createReport({
				albumId: this.id,
				reportType: reportType,
				notes: notes,
				versionNumber: undefined,
			});
		});

		this.reviewsStore = new AlbumReviewsStore(
			albumRepo,
			this.id,
			canDeleteAllComments,
			canDeleteAllComments,
			loginManager.loggedUserId,
		);
	}

	getUsers = async (): Promise<void> => {
		await $.post(
			functions.mapAbsoluteUrl('/Album/UsersWithAlbumInCollection'),
			{ albumId: this.id },
			(result) => {
				runInAction(() => {
					this.usersContent = result;
					this.userCollectionsPopupVisible = true;
				});
			},
		);
	};
}
