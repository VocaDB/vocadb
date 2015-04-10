
module vdb.viewModels.user {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class RatedSongsSearchViewModel {
		
		constructor(
			urlMapper: UrlMapper,
			private userRepo: rep.UserRepository, private artistRepo: rep.ArtistRepository,
			private songRepo: rep.SongRepository,
			private resourceRepo: rep.ResourceRepository,
			private languageSelection: string, private loggedUserId: number, private cultureCode: string,
			sort: string, groupByRating: boolean,
			pvPlayersFactory: pvs.PVPlayersFactory,
			initialize = true) {

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			if (sort)
				this.sort(sort);

			if (groupByRating != null)
				this.groupByRating(groupByRating);

			this.artistSearchParams = {
				allowCreateNew: false,
				acceptSelection: this.selectArtist,
				height: 300
			};

			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.childVoicebanks.subscribe(this.updateResultsWithTotalCount);
			this.groupByRating.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);
			this.rating.subscribe(this.updateResultsWithTotalCount);
			this.searchTerm.subscribe(this.updateResultsWithTotalCount);
			this.showTags.subscribe(this.updateResultsWithoutTotalCount);
			this.songListId.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithoutTotalCount);
			this.tag.subscribe(this.updateResultsWithTotalCount);
			this.viewMode.subscribe(this.updateResultsWithTotalCount);

			this.showChildVoicebanks = ko.computed(() => this.artistId() != null && helpers.ArtistHelper.canHaveChildVoicebanks(this.artistType()));

			this.pvPlayerViewModel = new pvs.PVPlayerViewModel(urlMapper, songRepo, userRepo, pvPlayersFactory);
			var songsRepoAdapter = new vdb.viewModels.songs.PlayListRepositoryForRatedSongsAdapter(userRepo, loggedUserId, this.searchTerm, this.sort,
				this.tag, this.artistId, this.childVoicebanks,
				this.rating, this.songListId, this.groupByRating, ko.observable("AdditionalNames,ThumbUrl"));
			this.playListViewModel = new vdb.viewModels.songs.PlayListViewModel(urlMapper, songsRepoAdapter, songRepo, userRepo, this.pvPlayerViewModel,
				cls.globalization.ContentLanguagePreference[languageSelection]);

			if (initialize)
				this.init();

		}

		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public artistType = ko.observable<cls.artists.ArtistType>(null);
		public childVoicebanks = ko.observable(false);
		public groupByRating = ko.observable(true);
		public isInit = false;
		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<IRatedSongSearchItem>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public playListViewModel: vdb.viewModels.songs.PlayListViewModel;
		public pvPlayerViewModel: pvs.PVPlayerViewModel;
		public pvServiceIcons: vdb.models.PVServiceIcons;
		public rating = ko.observable("Nothing");
		public resources = ko.observable<any>();
		public searchTerm = ko.observable("").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
		public showChildVoicebanks: KnockoutComputed<boolean>;
		public showTags = ko.observable(false);
		public songListId = ko.observable<number>(undefined);
		public songLists = ko.observableArray<dc.SongListBaseContract>([]);
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.resources() != null ? this.resources().songSortRuleNames[this.sort()] : "");
		public tag = ko.observable("");
		public viewMode = ko.observable("Details");

		public fields = ko.computed(() => {
			return "AdditionalNames,ThumbUrl" + (this.showTags() ? ",Tags" : "");
		});

		public getPVServiceIcons = (services: string) => {
			return this.pvServiceIcons.getIconUrls(services);
		}

		public init = () => {

			if (this.isInit)
				return;

			this.userRepo.getSongLists(this.loggedUserId, songLists => this.songLists(songLists));

			this.resourceRepo.getList(this.cultureCode, ['songSortRuleNames', 'songTypeNames'], resources => {
				this.resources(resources);
				this.updateResultsWithTotalCount();
				this.isInit = true;
			});

		};

		public selectArtist = (selectedArtistId: number) => {
			this.artistId(selectedArtistId);
			this.artistType(null);
			this.artistRepo.getOne(selectedArtistId, artist => {
				this.artistName(artist.name);
				this.artistType(cls.artists.ArtistType[artist.artistType]);
			});
		};

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean = true) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;
			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			if (this.viewMode() === "PlayList") {
				this.playListViewModel.updateResultsWithTotalCount(() => {
					this.pauseNotifications = false;
					this.loading(false);					
				});
				return;
			}

			this.userRepo.getRatedSongsList(this.loggedUserId, pagingProperties, this.languageSelection, this.searchTerm(),
				this.tag(),
				this.artistId(),
				this.childVoicebanks(),
				this.rating(), this.songListId(),
				this.groupByRating(),
				null,
				this.fields(),
				this.sort(),
				(result: dc.PartialFindResultContract<dc.RatedSongForUserForApiContract>) => {

					var songs: IRatedSongSearchItem[] = [];

					_.each(result.items, (item) => {

						var song: IRatedSongSearchItem = item.song;

						song.rating = item.rating;

						if (song.pvServices && song.pvServices !== 'Nothing') {
							song.previewViewModel = new SongWithPreviewViewModel(this.songRepo, this.userRepo, song.id, song.pvServices);
							song.previewViewModel.ratingComplete = vdb.ui.showThankYouForRatingMessage;
						} else {
							song.previewViewModel = null;
						}

						songs.push(song);

					});

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					this.page(songs);
					this.loading(false);

				});

		}

	}

	export interface IRatedSongSearchItem extends dc.SongApiContract {

		previewViewModel?: SongWithPreviewViewModel;

		rating?: string;

	}

}