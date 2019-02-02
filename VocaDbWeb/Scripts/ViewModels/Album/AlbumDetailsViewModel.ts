/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

module vdb.viewModels {

	import cls = vdb.models;
	import dc = dataContracts;
	import rep = repositories;

    export class AlbumDetailsViewModel {

		public comments: EditableCommentsViewModel;

        public downloadTagsDialog: DownloadTagsViewModel;

		private id: number;

		public reportViewModel: ReportEntryViewModel;

		public description: globalization.EnglishTranslatedStringViewModel;

		public personalDescription: SelfDescriptionViewModel;

		public reviewsViewModel: AlbumReviewsViewModel;

		public tagsEditViewModel: tags.TagsEditViewModel;

		public tagUsages: tags.TagListViewModel;

        public usersContent = ko.observable<string>();

        public getUsers = () => {

            $.post(vdb.functions.mapAbsoluteUrl("/Album/UsersWithAlbumInCollection"), { albumId: this.id }, result => {

                this.usersContent(result);
                $("#userCollectionsPopup").dialog("open");

            });

            return false;

        };

        constructor(
			repo: rep.AlbumRepository,
			userRepo: rep.UserRepository,
			artistRepository: rep.ArtistRepository,
			data: AlbumDetailsAjax,
			reportTypes: IEntryReportType[],
			loggedUserId: number,
			languagePreference: models.globalization.ContentLanguagePreference,
			canDeleteAllComments: boolean,
			formatString: string,
			showTranslatedDescription: boolean) {

			this.id = data.id;
            this.downloadTagsDialog = new DownloadTagsViewModel(this.id, formatString);
			this.description = new globalization.EnglishTranslatedStringViewModel(showTranslatedDescription);
			this.comments = new EditableCommentsViewModel(repo, this.id, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, data.latestComments, true);

			this.personalDescription = new SelfDescriptionViewModel(data.personalDescriptionAuthor, data.personalDescriptionText, artistRepository, callback => {
				repo.getOneWithComponents(this.id, 'Artists', cls.globalization.ContentLanguagePreference[languagePreference], result => {
					var artists = _.chain(result.artists)
						.filter(helpers.ArtistHelper.isValidForPersonalDescription)
						.map(a => a.artist).value();
					callback(artists);
				});
			}, vm => repo.updatePersonalDescription(this.id, vm.text(), vm.author.entry()));

			this.tagsEditViewModel = new tags.TagsEditViewModel({
				getTagSelections: callback => userRepo.getAlbumTagSelections(this.id, callback),
				saveTagSelections: tags => userRepo.updateAlbumTags(this.id, tags, this.tagUsages.updateTagUsages)
			}, cls.EntryType.Album, callback => repo.getTagSuggestions(this.id, callback));

			this.tagUsages = new tags.TagListViewModel(data.tagUsages);

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {

				repo.createReport(this.id, reportType, notes, null);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			});

			this.reviewsViewModel = new AlbumReviewsViewModel(repo, this.id, loggedUserId);
			this.reviewsViewModel.loadReviews();

        }

    }

    export interface AlbumDetailsAjax {

        id: number;
		latestComments: dc.CommentContract[];
		personalDescriptionText?: string;
		personalDescriptionAuthor?: dc.ArtistApiContract;
		tagUsages: dc.tags.TagUsageForApiContract[];

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

		constructor(private readonly albumRepository: rep.AlbumRepository, private readonly albumId: number, private readonly loggedUserId) {

		}

		public async createNewReview() {
			this.reviews.push({
				date: new Date().toLocaleDateString(),
				languageCode: this.languageCode(),
				text: this.newReviewText(),
				title: this.newReviewTitle(),
				user: { id: this.loggedUserId }
			});
			this.newReviewText("");
			this.newReviewTitle("");
			this.showCreateNewReview(false);
		}

		public async loadReviews() {
			const reviews = await this.albumRepository.getReviews(this.albumId);
			this.reviews(reviews);
		}

		public languageCode = ko.observable("");

		public newReviewText = ko.observable("");

		public newReviewTitle = ko.observable("");

		public reviews = ko.observableArray<dc.albums.AlbumReviewContract>();

		public showCreateNewReview = ko.observable(false);

		public writeReview = ko.observable(false);

	}

}