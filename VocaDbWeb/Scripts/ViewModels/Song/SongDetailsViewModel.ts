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

        public getUsers: () => void;

		public id: number;

		public reportViewModel: ReportEntryViewModel;

		public selectedLyricsId: KnockoutObservable<number>;

		public selectedPvId: KnockoutObservable<number>;

        public showAllVersions: () => void;

		public showTranslatedDescription: KnockoutObservable<boolean>;

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
			this.showTranslatedDescription = ko.observable(showTranslatedDescription);

			this.tagsEditViewModel = new tags.TagsEditViewModel({
				getTagSelections: callback => userRepository.getSongTagSelections(this.id, callback),
				saveTagSelections: tags => userRepository.updateSongTags(this.id, tags, this.tagsUpdated)
			});

			this.tagUsages = new tags.TagListViewModel(data.tagUsages);

            this.usersContent = ko.observable<string>();

            this.usersPopupVisible = ko.observable(false);
        
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
        
        public addedToList: () => void;

        public addSongToList: () => void;

        public dialogVisible = ko.observable(false);

        public newListName = ko.observable("");

		public notes = ko.observable("");

        public selectedListId: KnockoutObservable<number> = ko.observable(null);

        public showSongLists: () => void;

        public songLists = ko.observableArray<dc.SongListBaseContract>();

        constructor(repository: rep.SongRepository, resources: SongDetailsResources, songId: number) {
            
            var isValid = () => {
                return (this.selectedListId() != 0 || this.newListName().length > 0);
            };

            this.addSongToList = () => {
                if (isValid()) {
                    repository.addSongToList(this.selectedListId(), songId, this.notes(), this.newListName(),() => {

						this.notes("");
                        this.dialogVisible(false);

                        if (this.addedToList)
                            this.addedToList();

                    });
				}
            }

            this.showSongLists = () => {
                repository.songListsForUser(songId, songLists => {

					songLists.push({ id: 0, name: resources.createNewList });
                    this.songLists(songLists);
                    this.newListName("");
                    this.selectedListId(songLists.length > 0 ? songLists[0].id : undefined);
                    this.dialogVisible(true);

                });
            }
        
        }
    
    }

    export interface SongDetailsAjax {
        
        id: number;

		latestComments: dc.CommentContract[];

		selectedLyricsId: number;

		selectedPvId: number;

		tagUsages: dc.tags.TagUsageForApiContract[];

        userRating: string;
    
    }

    export interface SongDetailsResources {

        createNewList: string;

    }

}
