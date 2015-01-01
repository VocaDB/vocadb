
module vdb.viewModels.search {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongSearchViewModel extends SearchCategoryBaseViewModel<ISongSearchItem> {

		constructor(
			searchViewModel: SearchViewModel,
			urlMapper: vdb.UrlMapper,
			lang: string,
			private songRepo: rep.SongRepository,
			private artistRepo: rep.ArtistRepository,
			private userRepo: rep.UserRepository,
			resourceRep: rep.ResourceRepository,
			cultureCode: string,
			private loggedUserId: number,
			sort: string,
			artistId: number,
			childVoicebanks: boolean,
			songType: string,
			onlyWithPVs: boolean,
			since: number,
			minScore: number,
			viewMode: string,
			autoplay: boolean,
			shuffle: boolean,
			pvPlayerWrapperElement: HTMLElement) {

			super(searchViewModel);

			if (searchViewModel) {
				this.resourceManager = searchViewModel.resourcesManager;
			} else {
				this.resourceManager = new cls.ResourcesManager(resourceRep, cultureCode);
				this.resourceManager.loadResources(null, "songSortRuleNames");
			}

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			this.artistSearchParams = { acceptSelection: this.selectArtist };

			if (sort)
				this.sort(sort);

			if (artistId)
				this.selectArtist(artistId);

			if (songType)
				this.songType(songType);

			if (onlyWithPVs)
				this.pvsOnly(onlyWithPVs);

			this.childVoicebanks = ko.observable(childVoicebanks || false);
			this.minScore = ko.observable(minScore || undefined).extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });;
			this.since = ko.observable(since);
			this.viewMode = ko.observable(viewMode || "Details");

			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);
			this.childVoicebanks.subscribe(this.updateResultsWithTotalCount);
			this.minScore.subscribe(this.updateResultsWithTotalCount);
			this.onlyRatedSongs.subscribe(this.updateResultsWithTotalCount);
			this.pvPlayerViewModel = new pvs.PVPlayerViewModel(urlMapper, songRepo, userRepo, 'pv-player', pvPlayerWrapperElement, autoplay, shuffle);
			this.pvsOnly.subscribe(this.updateResultsWithTotalCount);
			this.since.subscribe(this.updateResultsWithTotalCount);
			this.songType.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.viewMode.subscribe(this.updateResultsWithTotalCount);

			this.showChildVoicebanks = ko.computed(() => this.artistId() != null && helpers.ArtistHelper.canHaveChildVoicebanks(this.artistType()));
			this.showTags = this.searchViewModel.showTags;
			this.sortName = ko.computed(() => this.resourceManager.resources().songSortRuleNames != null ? this.resourceManager.resources().songSortRuleNames[this.sort()] : "");

			var songsRepoAdapter = new vdb.viewModels.songs.PlayListRepositoryForSongsAdapter(songRepo, this.searchTerm, this.sort, this.songType,
				this.tag, this.artistId, this.artistParticipationStatus, this.childVoicebanks, this.pvsOnly, this.since,
				this.minScore,
				this.onlyRatedSongs, this.loggedUserId, this.fields, this.draftsOnly);
			this.playListViewModel = new vdb.viewModels.songs.PlayListViewModel(urlMapper, songsRepoAdapter, songRepo, userRepo, this.pvPlayerViewModel,
				cls.globalization.ContentLanguagePreference[lang]);

			this.loadResults = (pagingProperties, searchTerm, tag, status, callback) => {

				if (this.viewMode() === "PlayList") {
					this.playListViewModel.updateResultsWithTotalCount();		
					callback({ items: [], totalCount: 0 });			
				} else {
					
					this.songRepo.getList(pagingProperties, lang, searchTerm, this.sort(),
						this.songType() != cls.songs.SongType[cls.songs.SongType.Unspecified] ? this.songType() : null,
						tag, this.artistId(),
						this.artistParticipationStatus(),
						this.childVoicebanks(),
						this.pvsOnly(),
						null,
						this.since(),
						this.minScore(),
						this.onlyRatedSongs() ? this.loggedUserId : null,
						this.fields(),
						status, result => {

							_.each(result.items, (song: ISongSearchItem) => {

								if (song.pvServices && song.pvServices != 'Nothing') {
									song.previewViewModel = new SongWithPreviewViewModel(this.songRepo, this.userRepo, song.id, song.pvServices);
									song.previewViewModel.ratingComplete = vdb.ui.showThankYouForRatingMessage;
								} else {
									song.previewViewModel = null;
								}

							});

							callback(result);

						});

				}

			}

		}

		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistParticipationStatus = ko.observable("Everything");
		public artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public artistType = ko.observable<cls.artists.ArtistType>(null);
		public childVoicebanks: KnockoutObservable<boolean>;
		public minScore: KnockoutObservable<number>;
		public onlyRatedSongs = ko.observable(false);
		public playListViewModel: vdb.viewModels.songs.PlayListViewModel;
		public pvPlayerViewModel: pvs.PVPlayerViewModel;
		public pvsOnly = ko.observable(false);
		private pvServiceIcons: vdb.models.PVServiceIcons;
		private resourceManager: cls.ResourcesManager;
		public showChildVoicebanks: KnockoutComputed<boolean>;
		public since: KnockoutObservable<number>;
		public songType = ko.observable("Unspecified");
		public sort = ko.observable("Name");
		public sortName: KnockoutComputed<string>;
		public viewMode: KnockoutObservable<string>;

		public fields = ko.computed(() => this.showTags() ? "ThumbUrl,Tags" : "ThumbUrl");

		public getPVServiceIcons = (services: string) => {
			return this.pvServiceIcons.getIconUrls(services);
		}

		public selectArtist = (selectedArtistId: number) => {
			this.artistId(selectedArtistId);
			this.artistType(null);
			if (this.artistRepo) {
				this.artistRepo.getOne(selectedArtistId, artist => {
					this.artistName(artist.name);
					this.artistType(cls.artists.ArtistType[artist.artistType]);
				});				
			}
		};

		public showTags: KnockoutObservable<boolean>;

	}

	export interface ISongSearchItem extends dc.SongApiContract {

		previewViewModel?: SongWithPreviewViewModel;

	}

}