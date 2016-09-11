/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../DataContracts/SongListBaseContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
/// <reference path="../../Repositories/UserRepository.ts" />
/// <reference path="../PVRatingButtonsViewModel.ts" />

module vdb.viewModels {

    import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    // View model for the song details view.
    export class SongDetailsViewModel {
        
        public allVersionsVisible: KnockoutObservable<boolean>;

		public comments: EditableCommentsViewModel;

		private getOriginal = (linkedPages: string[]) => {
			
			if (linkedPages == null || !linkedPages.length)
				return;

			// http://utaitedb.net/S/1234 or http://utaitedb.net/Song/Details/1234
			var regex = /(http:\/\/(?:(?:utaitedb\.net)|(?:vocadb\.net))\/)(?:(?:Song)\/Details|(?:S))\/(\d+)/g;
			var page = linkedPages[0];

			var match = regex.exec(page);

			if (!match || match.length < 3)
				return;

			var siteUrl = match[1]; // either http://utaitedb.net/ or http://vocadb.net/
			var id = parseInt(match[2]);

			var repo = new rep.SongRepository(siteUrl, this.languagePreference);
			repo.getOneWithComponents(id, 'Nothing', null, song => {
				if (song.songType === "Original")
					this.originalVersion({ entry: song, url: page });
			});

		}

        public getUsers: () => void;

		public id: number;

		public originalVersion: KnockoutObservable<SongLinkWithUrl>;

		public reportViewModel: ReportEntryViewModel;

		public selectedLyrics = ko.observable<dc.songs.LyricsForSongContract>();

		public selectedLyricsId: KnockoutObservable<number>;

		public selectedPvId: KnockoutObservable<number>;

        public showAllVersions: () => void;

		public description: globalization.EnglishTranslatedStringViewModel;

        public songInListsDialog: SongInListsViewModel;

        public songListDialog: SongListsViewModel;

		public tagsEditViewModel: tags.TagsEditViewModel;

		public tagUsages: tags.TagListViewModel;

		private tagsUpdated = (usages: dc.tags.TagUsageForApiContract[]) => {
			
			this.tagUsages.tagUsages(_.sortBy(usages, u => -u.count));

		}

        public usersContent: KnockoutObservable<string>;

        public usersPopupVisible: KnockoutObservable<boolean>;

        public userRating: PVRatingButtonsViewModel;

        constructor(
            repository: rep.SongRepository,
            userRepository: rep.UserRepository,
            resources: SongDetailsResources,
			showTranslatedDescription: boolean,
			data: SongDetailsAjax,
			reportTypes: IEntryReportType[],
			loggedUserId: number,
			private languagePreference: models.globalization.ContentLanguagePreference,
			canDeleteAllComments: boolean,
            ratingCallback: () => void) {
            
            this.id = data.id;
            this.userRating = new PVRatingButtonsViewModel(userRepository, { id: data.id, vote: data.userRating }, ratingCallback);

            this.allVersionsVisible = ko.observable(false);

			this.comments = new EditableCommentsViewModel(repository, this.id, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, data.latestComments, true);

            this.getUsers = () => {
                repository.usersWithSongRating(this.id, result => {
                    this.usersContent(result);
                    this.usersPopupVisible(true);
                });
            };

			this.originalVersion = ko.observable({ entry: data.originalVersion });

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {

				repository.createReport(this.id, reportType, notes, null);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			});

            this.showAllVersions = () => {
                this.allVersionsVisible(true);
            };

            this.songInListsDialog = new SongInListsViewModel(repository, this.id);
            this.songListDialog = new SongListsViewModel(repository, resources, this.id);
			this.selectedLyricsId = ko.observable(data.selectedLyricsId);
			this.selectedPvId = ko.observable(data.selectedPvId);
			this.description = new globalization.EnglishTranslatedStringViewModel(showTranslatedDescription);

			this.tagsEditViewModel = new tags.TagsEditViewModel({
				getTagSelections: callback => userRepository.getSongTagSelections(this.id, callback),
				saveTagSelections: tags => userRepository.updateSongTags(this.id, tags, this.tagsUpdated)
			});

			this.tagUsages = new tags.TagListViewModel(data.tagUsages);

            this.usersContent = ko.observable<string>();

            this.usersPopupVisible = ko.observable(false);

			if (data.songType !== 'Original' && this.originalVersion().entry == null) {
				this.getOriginal(data.linkedPages);
			}

			this.selectedLyricsId.subscribe(id => {
				repository.getLyrics(id, lyrics => this.selectedLyrics(lyrics));
			});

			if (this.selectedLyricsId()) {
				this.selectedLyricsId.notifySubscribers(this.selectedLyricsId());
			}
        
        }
    
    }

    export class SongInListsViewModel {
        
        public contentHtml = ko.observable<string>();

        public dialogVisible = ko.observable(false);

        public show: () => void;

        constructor(repository: rep.SongRepository, songId: number) {
            
            this.show = () => {

                repository.songListsForSong(songId, result => {
                    this.contentHtml(result);
                    this.dialogVisible(true);
                });

            }
        
        }

    }

    export class SongListsViewModel {
        
		private tabName_Personal = "Personal";
		private tabName_Featured = "Featured";
		private tabName_New = "New";

        public addedToList: () => void;

        public addSongToList: () => void;

        public dialogVisible = ko.observable(false);

		private featuredLists = ko.observableArray<dc.SongListBaseContract>();

        public newListName = ko.observable("");

		public notes = ko.observable("");

		private personalLists = ko.observableArray<dc.SongListBaseContract>()

        public selectedListId: KnockoutObservable<number> = ko.observable(null);

        public showSongLists: () => void;

		public tabName = ko.observable(this.tabName_Personal);

        public songLists = ko.computed(() => this.tabName() === this.tabName_Personal ? this.personalLists() : this.featuredLists());

        constructor(repository: rep.SongRepository, resources: SongDetailsResources, songId: number) {
            
            var isValid = () => {
                return (this.selectedListId() != null || this.newListName().length > 0);
            };

            this.addSongToList = () => {
                if (isValid()) {
                    repository.addSongToList(this.selectedListId() || 0, songId, this.notes(), this.newListName(), () => {

						this.notes("");
                        this.dialogVisible(false);

                        if (this.addedToList)
                            this.addedToList();

                    });
				}
            }

            this.showSongLists = () => {
                repository.songListsForUser(songId, songLists => {

					var personalLists = _.filter(songLists, list => list.featuredCategory === "Nothing");
					var featuredLists = _.filter(songLists, list => list.featuredCategory !== "Nothing");

                    this.personalLists(personalLists);
					this.featuredLists(featuredLists);

					if (personalLists.length)
						this.tabName(this.tabName_Personal);
					else if (featuredLists.length)
						this.tabName(this.tabName_Featured);
					else
						this.tabName(this.tabName_New);

                    this.newListName("");
                    this.selectedListId(this.songLists().length > 0 ? this.songLists()[0].id : null);
                    this.dialogVisible(true);

                });
            }
        
        }
    
    }

    export interface SongDetailsAjax {
        
        id: number;

		latestComments: dc.CommentContract[];

		linkedPages?: string[];

		originalVersion?: dc.SongApiContract;

		selectedLyricsId: number;

		selectedPvId: number;

		songType: string;

		tagUsages: dc.tags.TagUsageForApiContract[];

        userRating: string;
    
    }

    export interface SongDetailsResources {

        createNewList: string;

    }

	export interface SongLinkWithUrl {
		
		entry: dc.SongApiContract;

		url?: string;

	}

}
