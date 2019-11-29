/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

import AlbumForUserForApiContract from '../../DataContracts/User/AlbumForUserForApiContract';
import AlbumReviewContract from '../../DataContracts/Album/AlbumReviewContract';
import AlbumRepository from '../../Repositories/AlbumRepository';
import ArtistApiContract from '../../DataContracts/Artist/ArtistApiContract';
import ArtistHelper from '../../Helpers/ArtistHelper';
import ArtistRepository from '../../Repositories/ArtistRepository';
import CommentContract from '../../DataContracts/CommentContract';
import ContentLanguagePreference from '../../Models/Globalization/ContentLanguagePreference';
import EditableCommentsViewModel from '../EditableCommentsViewModel';
import EnglishTranslatedStringViewModel from '../Globalization/EnglishTranslatedStringViewModel';
import EntryType from '../../Models/EntryType';
import { IEntryReportType } from '../ReportEntryViewModel';
import { mapAbsoluteUrl } from '../../Shared/GlobalFunctions';
import ReportEntryViewModel from '../ReportEntryViewModel';
import SelfDescriptionViewModel from '../SelfDescriptionViewModel';
import TagsEditViewModel from '../Tag/TagsEditViewModel';
import TagListViewModel from '../Tag/TagListViewModel';
import TagUsageForApiContract from '../../DataContracts/Tag/TagUsageForApiContract';
import ui from '../../Shared/MessagesTyped';
import UserApiContract from '../../DataContracts/User/UserApiContract';
import UserRepository from '../../Repositories/UserRepository';

//module vdb.viewModels {

    export class AlbumDetailsViewModel {

		public comments: EditableCommentsViewModel;

        public downloadTagsDialog: DownloadTagsViewModel;

		private id: number;

		public reportViewModel: ReportEntryViewModel;

		public description: EnglishTranslatedStringViewModel;

		public personalDescription: SelfDescriptionViewModel;

		public reviewsViewModel: AlbumReviewsViewModel;

		public tagsEditViewModel: TagsEditViewModel;

		public tagUsages: TagListViewModel;

        public usersContent = ko.observable<string>();

        public getUsers = () => {

            $.post(mapAbsoluteUrl("/Album/UsersWithAlbumInCollection"), { albumId: this.id }, result => {

                this.usersContent(result);
                $("#userCollectionsPopup").dialog("open");

            });

            return false;

        };

        constructor(
			repo: AlbumRepository,
			userRepo: UserRepository,
			artistRepository: ArtistRepository,
			data: AlbumDetailsAjax,
			reportTypes: IEntryReportType[],
			loggedUserId: number,
			languagePreference: ContentLanguagePreference,
			canDeleteAllComments: boolean,
			formatString: string,
			showTranslatedDescription: boolean) {

			this.id = data.id;
            this.downloadTagsDialog = new DownloadTagsViewModel(this.id, formatString);
			this.description = new EnglishTranslatedStringViewModel(showTranslatedDescription);
			this.comments = new EditableCommentsViewModel(repo, this.id, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, data.latestComments, true);

			this.personalDescription = new SelfDescriptionViewModel(data.personalDescriptionAuthor, data.personalDescriptionText, artistRepository, callback => {
				repo.getOneWithComponents(this.id, 'Artists', ContentLanguagePreference[languagePreference], result => {
					var artists = _.chain(result.artists)
						.filter(ArtistHelper.isValidForPersonalDescription)
						.map(a => a.artist).value();
					callback(artists);
				});
			}, vm => repo.updatePersonalDescription(this.id, vm.text(), vm.author.entry()));

			this.tagsEditViewModel = new TagsEditViewModel({
				getTagSelections: callback => userRepo.getAlbumTagSelections(this.id, callback),
				saveTagSelections: tags => userRepo.updateAlbumTags(this.id, tags, this.tagUsages.updateTagUsages)
			}, EntryType.Album, callback => repo.getTagSuggestions(this.id, callback));

			this.tagUsages = new TagListViewModel(data.tagUsages);

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {

				repo.createReport(this.id, reportType, notes, null);

				ui.showSuccessMessage(vdb.resources.shared.reportSent);

			});

			this.reviewsViewModel = new AlbumReviewsViewModel(repo, this.id, canDeleteAllComments, canDeleteAllComments, loggedUserId);

        }

    }

    export interface AlbumDetailsAjax {

        id: number;
		latestComments: CommentContract[];
		personalDescriptionText?: string;
		personalDescriptionAuthor?: ArtistApiContract;
		tagUsages: TagUsageForApiContract[];

    }

    export class DownloadTagsViewModel {

        public dialogVisible = ko.observable(false);

        public downloadTags = () => {

            this.dialogVisible(false);

            var url = "/Album/DownloadTags/" + this.albumId;
            window.location.href = url + "?setFormatString=true&formatString=" + encodeURIComponent(this.formatString());

        };

        public formatString: KnockoutObservable<string>;

        public dialogButtons = ko.observableArray([
            { text: vdb.resources.albumDetails.download, click: this.downloadTags },
        ]);

        public show = () => {

            this.dialogVisible(true);

        };

        constructor(private albumId: number, formatString: string) {
            this.formatString = ko.observable(formatString)
        }

	}

	export class AlbumReviewsViewModel {

		constructor(
			private readonly albumRepository: AlbumRepository,
			private readonly albumId: number,
			private readonly canDeleteAllComments: boolean,
			private readonly canEditAllComments: boolean,
			private readonly loggedUserId?: number) {

		}

		public beginEditReview = (review: AlbumReviewViewModel) => {

			review.beginEdit();
			this.editReviewModel(review);

		}

		public cancelEditReview = () => {
			this.editReviewModel(null);
		}

		private canDeleteReview = (comment: AlbumReviewContract) => {
			// If one can edit they can also delete
			return (this.canDeleteAllComments || this.canEditAllComments || (comment.user && comment.user.id === this.loggedUserId));
		}

		private canEditReview = (comment: AlbumReviewContract) => {
			return (this.canEditAllComments || (comment.user && comment.user.id === this.loggedUserId));
		}

		public async createNewReview() {
			const contract = {
				date: new Date().toLocaleDateString(),
				languageCode: this.languageCode(),
				text: this.newReviewText(),
				title: this.newReviewTitle(),
				user: { id: this.loggedUserId }
			};
			this.newReviewText("");
			this.newReviewTitle("");
			this.showCreateNewReview(false);
			this.languageCode("");
			const result = await this.albumRepository.createOrUpdateReview(this.albumId, contract);
			this.reviews.push(new AlbumReviewViewModel(result, this.canDeleteReview(result), this.canEditReview(result)));
		}

		public deleteReview = (review: AlbumReviewViewModel) => {

			this.reviews.remove(review);

			this.albumRepository.deleteReview(this.albumId, review.id);

		}

		public getRatingForUser(userId: number): number {
			return _.chain(this.userRatings())
				.filter(rating => rating.user && rating.user.id === userId && rating.rating)
				.map(rating => rating.rating)
				.take(1)
				.value()
				[0];
		}

		public ratingStars(userRating: number) {

			var ratings = _.map([1, 2, 3, 4, 5], rating => { return { enabled: (Math.round(userRating) >= rating) } });
			return ratings;

		}

		public saveEditedReview = () => {

			if (!this.editReviewModel())
				return;

			this.editReviewModel().saveChanges();
			var editedContract = this.editReviewModel().toContract();

			this.albumRepository.createOrUpdateReview(this.albumId, editedContract);

			this.editReviewModel(null);

		}

		public async loadReviews() {
			const [reviews, ratings] = await Promise.all([this.albumRepository.getReviews(this.albumId), this.albumRepository.getUserCollections(this.albumId)]);
			const reviewViewModels = _.map(reviews, review => new AlbumReviewViewModel(review, this.canDeleteReview(review), this.canEditReview(review)));
			this.reviews(reviewViewModels);
			this.userRatings(ratings);
		}

		public editReviewModel = ko.observable<AlbumReviewViewModel>(null);

		public languageCode = ko.observable("");

		public newReviewText = ko.observable("");

		public newReviewTitle = ko.observable("");

		public reviews = ko.observableArray<AlbumReviewViewModel>();

		public showCreateNewReview = ko.observable(false);

		public writeReview = ko.observable(false);

		public reviewAlreadySubmitted = ko.computed(() => {
			return _.some(this.reviews(), review => review.user.id === this.loggedUserId && review.languageCode() === this.languageCode());
		});

		private userRatings = ko.observableArray<AlbumForUserForApiContract>();

	}

	export class AlbumReviewViewModel {

		constructor(contract: AlbumReviewContract, public canBeDeleted: boolean, public canBeEdited: boolean) {
			this.date = new Date(contract.date);
			this.id = contract.id;
			this.languageCode = ko.observable(contract.languageCode);
			this.text = ko.observable(contract.text);
			this.title = ko.observable(contract.title);
			this.user = contract.user;
		}

		public beginEdit = () => {
			this.editedTitle(this.title());
			this.editedText(this.text());
		}

		public saveChanges = () => {
			this.text(this.editedText());
			this.title(this.editedTitle());
		}

		public toContract: () => AlbumReviewContract = () => {
			return { date: this.date.toISOString(), id: this.id, languageCode: this.languageCode(), text: this.text(), title: this.title(), user: this.user };
		}

		public date: Date;

		public editedTitle = ko.observable("");

		public editedText = ko.observable("");

		public id?: number;

		public languageCode: KnockoutObservable<string>;

		public text: KnockoutObservable<string>;

		public title: KnockoutObservable<string>;

		public user: UserApiContract;

	}

//}