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
import { AlbumRepository } from '@/Repositories/AlbumRepository';
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
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export class DownloadTagsStore {
	@observable public dialogVisible = false;
	@observable public formatString: string;

	public constructor(public readonly albumId: number, formatString: string) {
		makeObservable(this);

		this.formatString = formatString;
	}

	@action public show = (): void => {
		this.dialogVisible = true;
	};
}

export class EditCollectionStore {
	@observable public dialogVisible = false;
	@observable public albumMediaType;
	@observable public albumPurchaseStatus;
	@observable public collectionRating;

	public constructor(
		public readonly albumId: number,
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
	public readonly date: Date;
	@observable public editedTitle = '';
	@observable public editedText = '';
	public readonly id?: number;
	@observable public languageCode: string;
	@observable public text: string;
	@observable public title: string;
	public readonly user: UserApiContract;

	public constructor(
		contract: AlbumReviewContract,
		public readonly canBeDeleted: boolean,
		public readonly canBeEdited: boolean,
	) {
		makeObservable(this);

		this.date = new Date(contract.date);
		this.id = contract.id;
		this.languageCode = contract.languageCode;
		this.text = contract.text;
		this.title = contract.title;
		this.user = contract.user;
	}

	@action public beginEdit = (): void => {
		this.editedTitle = this.title;
		this.editedText = this.text;
	};

	@action public saveChanges = (): void => {
		this.text = this.editedText;
		this.title = this.editedTitle;
	};

	public toContract = (): AlbumReviewContract => {
		return {
			date: this.date.toISOString(),
			id: this.id,
			languageCode: this.languageCode,
			text: this.text,
			title: this.title,
			user: this.user,
		};
	};
}

export class AlbumReviewsStore {
	@observable public editReviewStore?: AlbumReviewStore;
	@observable public languageCode = '';
	@observable public newReviewText = '';
	@observable public newReviewTitle = '';
	@observable public reviews: AlbumReviewStore[] = [];
	@observable public showCreateNewReview = false;
	@observable public writeReview = false;
	@observable private userRatings: AlbumForUserForApiContract[] = [];

	public constructor(
		private readonly albumRepo: AlbumRepository,
		private readonly albumId: number,
		private readonly canDeleteAllComments: boolean,
		private readonly canEditAllComments: boolean,
		private readonly loggedUserId?: number,
	) {
		makeObservable(this);
	}

	@computed public get reviewAlreadySubmitted(): boolean {
		return this.reviews.some(
			(review) =>
				review.user.id === this.loggedUserId &&
				review.languageCode === this.languageCode,
		);
	}

	@action public beginEditReview = (review: AlbumReviewStore): void => {
		review.beginEdit();
		this.editReviewStore = review;
	};

	@action public cancelEditReview = (): void => {
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

	@action public createNewReview = async (): Promise<void> => {
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

	@action public deleteReview = (review: AlbumReviewStore): Promise<void> => {
		_.pull(this.reviews, review);

		return this.albumRepo.deleteReview({
			albumId: this.albumId,
			reviewId: review.id!,
		});
	};

	public getRatingForUser = (userId: number): number => {
		return _.chain(this.userRatings)
			.filter(
				(rating) =>
					!!rating.user && rating.user.id === userId && !!rating.rating,
			)
			.map((rating) => rating.rating)
			.take(1)
			.value()[0];
	};

	public ratingStars = (userRating: number): { enabled: boolean }[] => {
		const ratings = [1, 2, 3, 4, 5].map((rating) => ({
			enabled: Math.round(userRating) >= rating,
		}));
		return ratings;
	};

	@action public saveEditedReview = (): void => {
		if (!this.editReviewStore) return;

		this.editReviewStore.saveChanges();
		const editedContract = this.editReviewStore.toContract();

		this.albumRepo.createOrUpdateReview({
			albumId: this.albumId,
			reviewContract: editedContract,
		});

		this.editReviewStore = undefined;
	};

	public loadReviews = async (): Promise<void> => {
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
	public readonly comments: EditableCommentsStore;
	public readonly downloadTagsDialog: DownloadTagsStore;
	public readonly editCollectionDialog: EditCollectionStore;
	private readonly id: number;
	public readonly reportStore: ReportEntryStore;
	public readonly description: EnglishTranslatedStringStore;
	public readonly personalDescription: SelfDescriptionStore;
	public readonly reviewsStore: AlbumReviewsStore;
	public readonly tagsEditStore: TagsEditStore;
	public readonly tagUsages: TagListStore;
	@observable public userHasAlbum;
	@observable public usersContent?: string;
	@observable public userCollectionsPopupVisible = false;

	public constructor(
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
						fields: 'Artists',
						lang: values.languagePreference,
					})
					.then((result) => {
						const artists = _.chain(result.artists)
							.filter(ArtistHelper.isValidForPersonalDescription)
							.map((a) => a.artist)
							.value();
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

	public getUsers = async (): Promise<void> => {
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
