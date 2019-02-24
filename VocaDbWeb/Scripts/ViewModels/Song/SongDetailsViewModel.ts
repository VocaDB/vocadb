/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../DataContracts/SongListBaseContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
/// <reference path="../../Repositories/UserRepository.ts" />
/// <reference path="../PVRatingButtonsViewModel.ts" />

module vdb.viewModels {

	import cls = models;
    import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    // View model for the song details view.
    export class SongDetailsViewModel {
        
        public allVersionsVisible: KnockoutObservable<boolean>;

		public comments: EditableCommentsViewModel;

		public getMatchedSite = (page: string) => {

			// http://utaitedb.net/S/1234 or http://utaitedb.net/Song/Details/1234
			const regex = /(http(?:s)?:\/\/(?:(?:utaitedb\.net)|(?:vocadb\.net)|(?:touhoudb\.com))\/)(?:(?:Song)\/Details|(?:S))\/(\d+)/g;
			const match = regex.exec(page);

			if (!match || match.length < 3)
				return null;

			const siteUrl = match[1].replace("http://", "https://"); // either http://utaitedb.net/ or http://vocadb.net/
			const id = parseInt(match[2]);

			return { siteUrl: siteUrl, id: id };

		}

		private getOriginal = (linkedPages: string[]) => {
			
			if (linkedPages == null || !linkedPages.length)
				return;

			const page = linkedPages[0];
			const match = this.getMatchedSite(page);

			if (!match)
				return;

			const {siteUrl, id} = match;

			const repo = new rep.SongRepository(siteUrl, this.languagePreference);
			// TODO: this should be cached, but first we need to make sure the other instances are not cached.
			repo.getOneWithComponents(id, 'Nothing', null, song => {
				if (song.songType === "Original")
					this.originalVersion({ entry: song, url: page, domain: siteUrl });
			});

		}

        public getUsers: () => void;

		public id: number;

		public initLyrics = () => {

			if (!this.selectedLyrics() && this.selectedLyricsId()) {
				this.selectedLyricsId.notifySubscribers(this.selectedLyricsId());
			}

		}

		public maintenanceDialogVisible = ko.observable(false);

		public originalVersion: KnockoutObservable<SongLinkWithUrl>;

		public reportViewModel: ReportEntryViewModel;

		public selectedLyrics = ko.observable<dc.songs.LyricsForSongContract>();

		public selectedLyricsId: KnockoutObservable<number>;

		public selectedPvId: KnockoutObservable<number>;

		public personalDescription: SelfDescriptionViewModel;

        public showAllVersions: () => void;

		public description: globalization.EnglishTranslatedStringViewModel;

        public songInListsDialog: SongInListsViewModel;

        public songListDialog: SongListsViewModel;

		public tagsEditViewModel: tags.TagsEditViewModel;

		public tagUsages: tags.TagListViewModel;

        public ratingsDialogViewModel = new RatingsViewModel();

        public userRating: PVRatingButtonsViewModel;

        constructor(
            private repository: rep.SongRepository,
			userRepository: rep.UserRepository,
			artistRepository: rep.ArtistRepository,
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
                repository.getRatings(this.id, result => {
                    this.ratingsDialogViewModel.ratings(result);
                    this.ratingsDialogViewModel.popupVisible(true);
                });
            };

			this.originalVersion = ko.observable({ entry: data.originalVersion });

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {

				repository.createReport(this.id, reportType, notes, null);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			});

			this.personalDescription = new SelfDescriptionViewModel(data.personalDescriptionAuthor, data.personalDescriptionText, artistRepository, callback => {
				repository.getOneWithComponents(this.id, 'Artists', cls.globalization.ContentLanguagePreference[this.languagePreference], result => {
					var artists = _.chain(result.artists)
						.filter(helpers.ArtistHelper.isValidForPersonalDescription)
						.map(a => a.artist).value();
					callback(artists);
				});
			}, vm => repository.updatePersonalDescription(this.id, vm.text(), vm.author.entry()));

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
				saveTagSelections: tags => userRepository.updateSongTags(this.id, tags, this.tagUsages.updateTagUsages)
			}, cls.EntryType.Song, callback => repository.getTagSuggestions(this.id, callback));

			this.tagUsages = new tags.TagListViewModel(data.tagUsages);

			if (data.songType !== 'Original' && this.originalVersion().entry == null) {
				this.getOriginal(data.linkedPages);
			}

			this.selectedLyricsId.subscribe(id => {
				this.selectedLyrics(null);
				repository.getLyrics(id, data.version, lyrics => this.selectedLyrics(lyrics));
			});
        
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

	export class RatingsViewModel {

		constructor() {

			const fav = cls.SongVoteRating[cls.SongVoteRating.Favorite];
			const like = cls.SongVoteRating[cls.SongVoteRating.Like];

			this.favorites = ko.computed(() => _
				.chain(this.ratings())
				.filter(r => r.user && r.rating === fav)
				.take(20)
				.map(r => r.user)
				.sortBy(u => u.name)
				.value());

			this.favoritesCount = ko.computed(() => _
				.chain(this.ratings())
				.filter(r => r.rating === fav)
				.size()
				.value());

			this.likes = ko.computed(() => _
				.chain(this.ratings())
				.filter(r => r.user && r.rating === like)
				.take(20)
				.map(r => r.user)
				.sortBy(u => u.name)
				.value());

			this.likesCount = ko.computed(() => _
				.chain(this.ratings())
				.filter(r => r.rating === like)
				.size()
				.value());

			this.hiddenRatingsCount = ko.computed(() => _
				.chain(this.ratings())
				.filter(r => !r.user)
				.size()
				.value());

			this.showFavorites = ko.computed(() => !!this.favorites().length);
			this.showLikes = ko.computed(() => !!this.likes().length);

		}

		public readonly favorites: KnockoutComputed<dc.user.UserApiContract[]>;

		public readonly favoritesCount: KnockoutComputed<number>;

		public readonly hiddenRatingsCount: KnockoutComputed<number>;

		public readonly likes: KnockoutComputed<dc.user.UserApiContract[]>;

		public readonly likesCount: KnockoutComputed<number>;

		public readonly popupVisible = ko.observable(false);

		public readonly ratings = ko.observableArray<dc.RatedSongForUserForApiContract>();

		public readonly showFavorites: KnockoutComputed<boolean>;

		public readonly showLikes: KnockoutComputed<boolean>;

	}

    export interface SongDetailsAjax {
        
        id: number;

		latestComments: dc.CommentContract[];

		linkedPages?: string[];

		originalVersion?: dc.SongApiContract;

		selectedLyricsId: number;

		selectedPvId: number;

		personalDescriptionText?: string;

		personalDescriptionAuthor?: dc.ArtistApiContract;

		songType: string;

		tagUsages: dc.tags.TagUsageForApiContract[];

        userRating: string;

		version: number;
    
    }

    export interface SongDetailsResources {

        createNewList: string;

    }

	export interface SongLinkWithUrl {
		
		entry: dc.SongApiContract;

		url?: string;

		domain?: string;

	}

}
