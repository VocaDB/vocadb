
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
			private loggedUserId: number,
			sort: string,
			artistId: number,
			songType: string, onlyWithPVs: boolean) {

			super(searchViewModel);

			this.pvServiceIcons = new vdb.models.PVServiceIcons(urlMapper);

			this.artistSearchParams = {
				allowCreateNew: false,
				acceptSelection: this.selectArtist,
				height: 300
			};

			if (sort)
				this.sort(sort);

			if (artistId)
				this.selectArtist(artistId);

			if (songType)
				this.songType(songType);

			if (onlyWithPVs)
				this.pvsOnly(onlyWithPVs);

			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);
			this.childVoicebanks.subscribe(this.updateResultsWithTotalCount);
			this.onlyRatedSongs.subscribe(this.updateResultsWithTotalCount);
			this.pvsOnly.subscribe(this.updateResultsWithTotalCount);
			this.since.subscribe(this.updateResultsWithTotalCount);
			this.songType.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.showChildVoicebanks = ko.computed(() => this.artistId() != null && helpers.ArtistHelper.canHaveChildVoicebanks(this.artistType()));

			this.loadResults = (pagingProperties, searchTerm, tag, status, callback) => {

				this.songRepo.getList(pagingProperties, lang, searchTerm, this.sort(), this.songType(), tag, this.artistId(),
					this.artistParticipationStatus(),
					this.childVoicebanks(),
					this.pvsOnly(),
					this.since(),
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

		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistParticipationStatus = ko.observable("Everything");
		public artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public artistType = ko.observable<cls.artists.ArtistType>(null);
		public childVoicebanks = ko.observable(false);
		public onlyRatedSongs = ko.observable(false);
		public pvsOnly = ko.observable(false);
		private pvServiceIcons: vdb.models.PVServiceIcons;
		public showChildVoicebanks: KnockoutComputed<boolean>;
		public since = ko.observable<number>(null);
		public songType = ko.observable("Unspecified");
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.searchViewModel.resources() != null ? this.searchViewModel.resources().songSortRuleNames[this.sort()] : "");

		public fields = ko.computed(() => this.searchViewModel.showTags() ? "ThumbUrl,Tags" : "ThumbUrl");

		public getPVServiceIcons = (services: string) => {
			return this.pvServiceIcons.getIconUrls(services);
		}

		public selectArtist = (selectedArtistId: number) => {
			this.artistId(selectedArtistId);
			this.artistType(null);
			this.artistRepo.getOne(selectedArtistId, artist => {
				this.artistName(artist.name);
				this.artistType(cls.artists.ArtistType[artist.artistType]);
			});
		};

	}

	export interface ISongSearchItem extends dc.SongApiContract {

		previewViewModel?: SongWithPreviewViewModel;

	}

}