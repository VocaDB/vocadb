
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
			private eventRepo: rep.ReleaseEventRepository,
			resourceRep: rep.ResourceRepository,
			cultureCode: string,
			private loggedUserId: number,
			sort: string,
			artistId: number[],
			childVoicebanks: boolean,
			songType: string,
			eventId: number,
			onlyWithPVs: boolean,
			onlyRatedSongs: boolean,
			since: number,
			minScore: number,
			viewMode: string,
			autoplay: boolean,
			shuffle: boolean,
			pvPlayersFactory: pvs.PVPlayersFactory) {

			super(searchViewModel);

			if (searchViewModel) {
				this.resourceManager = searchViewModel.resourcesManager;
				this.showTags = this.searchViewModel.showTags;
			} else {
				this.resourceManager = new cls.ResourcesManager(resourceRep, cultureCode);
				this.resourceManager.loadResources(null, "songSortRuleNames");
				this.showTags = ko.observable(false);
			}

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			this.artistFilters = new ArtistFilters(this.artistRepo, childVoicebanks);
			this.artistFilters.selectArtists(artistId);

			this.releaseEvent = new BasicEntryLinkViewModel<cls.IEntryWithIdAndName>({ id: eventId, name: null }, this.eventRepo ? this.eventRepo.getOne : null);

			if (eventId)
				this.releaseEvent.id(eventId);

			if (sort)
				this.sort(sort);

			if (songType)
				this.songType(songType);

			if (onlyWithPVs)
				this.pvsOnly(onlyWithPVs);

			if (onlyRatedSongs)
				this.onlyRatedSongs(onlyRatedSongs);

			this.minScore = ko.observable(minScore || undefined).extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });;
			this.since = ko.observable(since);
			this.viewMode = ko.observable(viewMode || "Details");

			this.parentVersion = new BasicEntryLinkViewModel<cls.IEntryWithIdAndName>(null, this.songRepo.getOne);

			this.advancedFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.afterDate.subscribe(this.updateResultsWithTotalCount);
			this.releaseEvent.subscribe(this.updateResultsWithTotalCount);
			this.minScore.subscribe(this.updateResultsWithTotalCount);
			this.onlyRatedSongs.subscribe(this.updateResultsWithTotalCount);
			this.parentVersion.subscribe(this.updateResultsWithTotalCount);
			this.pvPlayerViewModel = new pvs.PVPlayerViewModel(urlMapper, songRepo, userRepo, pvPlayersFactory, autoplay, shuffle);
			this.pvsOnly.subscribe(this.updateResultsWithTotalCount);
			this.since.subscribe(this.updateResultsWithTotalCount);
			this.songType.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.viewMode.subscribe(this.updateResultsWithTotalCount);

			this.sortName = ko.computed(() => this.resourceManager.resources().songSortRuleNames != null ? this.resourceManager.resources().songSortRuleNames[this.sort()] : "");

			var songsRepoAdapter = new vdb.viewModels.songs.PlayListRepositoryForSongsAdapter(songRepo, this.searchTerm, this.sort, this.songType,
				this.afterDate,
                this.beforeDate,
				this.tagIds, this.childTags,
				this.artistFilters.artistIds, this.artistFilters.artistParticipationStatus,
				this.artistFilters.childVoicebanks,
				this.artistFilters.includeMembers,
				this.releaseEvent.id,
				this.pvsOnly, this.since,
				this.minScore,
				this.onlyRatedSongs,
				this.loggedUserId,
				this.parentVersion.id,
				this.fields, this.draftsOnly, this.advancedFilters.filters);

			this.playListViewModel = new vdb.viewModels.songs.PlayListViewModel(urlMapper, songsRepoAdapter, songRepo, userRepo, this.pvPlayerViewModel,
				cls.globalization.ContentLanguagePreference[lang]);

			this.loadResults = (pagingProperties, searchTerm, tag, childTags, status, callback) => {

				if (this.viewMode() === "PlayList") {
					this.playListViewModel.updateResultsWithTotalCount();		
					callback({ items: [], totalCount: 0 });			
				} else {

					this.songRepo.getList(pagingProperties, lang, searchTerm, this.sort(),
						this.songType() != cls.songs.SongType[cls.songs.SongType.Unspecified] ? this.songType() : null,
						this.afterDate(),
                        this.beforeDate(),
						tag,
						childTags,
						this.artistFilters.artistIds(),
						this.artistFilters.artistParticipationStatus(),
						this.artistFilters.childVoicebanks(),
						this.artistFilters.includeMembers(),
						this.releaseEvent.id(),
						this.pvsOnly(),
						null,
						this.since(),
						this.minScore(),
						this.onlyRatedSongs() ? this.loggedUserId : null,
						this.parentVersion.id(),
						this.fields(),
						status,
						this.advancedFilters.filters(),
						result => {

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

		public artistFilters: ArtistFilters;
		public dateMonth = ko.observable<number>(null);
		public dateYear = ko.observable<number>(null);
		public releaseEvent: BasicEntryLinkViewModel<cls.IEntryWithIdAndName>;
		public minScore: KnockoutObservable<number>;
		public onlyRatedSongs = ko.observable(false);
		public parentVersion: BasicEntryLinkViewModel<cls.IEntryWithIdAndName>;
		public playListViewModel: vdb.viewModels.songs.PlayListViewModel;
		public pvPlayerViewModel: pvs.PVPlayerViewModel;
		public pvsOnly = ko.observable(false);
		private pvServiceIcons: vdb.models.PVServiceIcons;
		private resourceManager: cls.ResourcesManager;
		public since: KnockoutObservable<number>;
		public songType = ko.observable("Unspecified");
		public sort = ko.observable("Name");
		public sortName: KnockoutComputed<string>;
		public viewMode: KnockoutObservable<string>;

        // Remember, JavaScript months start from 0 (who came up with that??)
		private toDateOrNull = (mom: moment.Moment) => mom.isValid() ? mom.toDate() : null;
		private afterDate = ko.computed(() => this.dateYear() ? this.toDateOrNull(moment.utc([this.dateYear(), (this.dateMonth() || 1) - 1, 1])) : null);
		private beforeDate = () => this.dateYear() ? this.toDateOrNull(moment.utc([this.dateYear(), (this.dateMonth() || 12) - 1, 1]).add(1, 'M')) : null;

		public fields = ko.computed(() => this.showTags() ? "AdditionalNames,ThumbUrl,Tags" : "AdditionalNames,ThumbUrl");

		public getPVServiceIcons = (services: string) => {
			return this.pvServiceIcons.getIconUrls(services);
		}

		public showTags: KnockoutObservable<boolean>;

	}

	export interface ISongSearchItem extends dc.SongApiContract {

		previewViewModel?: SongWithPreviewViewModel;

	}

}
