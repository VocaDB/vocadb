
module vdb.viewModels.search {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class AlbumSearchViewModel extends SearchCategoryBaseViewModel<dc.AlbumContract> {

		constructor(searchViewModel: SearchViewModel,
			private unknownPictureUrl: string,
			lang: string, private albumRepo: rep.AlbumRepository,
			private artistRepo: rep.ArtistRepository,
			resourceRep: rep.ResourceRepository,
			cultureCode: string,
			sort: string,
			artistId: number,
			childVoicebanks: boolean,
			albumType: string,
			viewMode: string) {

			super(searchViewModel);

			if (searchViewModel) {
				this.resourceManager = searchViewModel.resourcesManager;
			} else {
				this.resourceManager = new cls.ResourcesManager(resourceRep, cultureCode);
				this.resourceManager.loadResources(null, "albumSortRuleNames", "discTypeNames");
			}

			this.artistSearchParams = { acceptSelection: this.selectArtist };

			this.albumType = ko.observable(albumType || "Unknown");
			this.childVoicebanks = ko.observable(childVoicebanks || false);
			this.sort = ko.observable(sort || "Name");
			this.viewMode = ko.observable(viewMode || "Details");

			if (artistId)
				this.selectArtist(artistId);

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.albumType.subscribe(this.updateResultsWithTotalCount);
			this.artists.subscribe(this.updateResultsWithTotalCount);
			this.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);
			this.childVoicebanks.subscribe(this.updateResultsWithTotalCount);

			this.showChildVoicebanks = ko.computed(() => this.hasSingleArtist() && helpers.ArtistHelper.canHaveChildVoicebanks(this.artists()[0].artistType()));
			this.sortName = ko.computed(() => {
				return this.resourceManager.resources().albumSortRuleNames != null ? this.resourceManager.resources().albumSortRuleNames[this.sort()] : "";
			});

			this.loadResults = (pagingProperties, searchTerm, tags, status, callback) => {

				var artistIds = _.map(this.artists(), a => a.id);

				this.albumRepo.getList(pagingProperties, lang, searchTerm, this.sort(), this.albumType(), tags, artistIds,
					this.artistParticipationStatus(), this.childVoicebanks(), this.fields(), status, false, callback);

			}

		}

		public albumType: KnockoutObservable<string>;
		public artists = ko.observableArray<ArtistFilter>();
		public artistParticipationStatus = ko.observable("Everything");
		public artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public childVoicebanks: KnockoutObservable<boolean>;
		private resourceManager: cls.ResourcesManager;
		public showChildVoicebanks: KnockoutComputed<boolean>;
		public sort: KnockoutObservable<string>;
		public sortName: KnockoutComputed<string>;
		public viewMode: KnockoutObservable<string>;

		public discTypeName = (discTypeStr: string) => this.resourceManager.resources().discTypeNames != null ? this.resourceManager.resources().discTypeNames[discTypeStr] : "";

		public fields = ko.computed(() => this.showTags() ? "MainPicture,Tags" : "MainPicture");

		public hasMultipleArtists = ko.computed(() => this.artists().length > 1);

		public hasSingleArtist = ko.computed(() => this.artists().length === 1);

		public ratingStars = (album: dc.AlbumContract) => {

			if (!album)
				return [];

			var ratings = _.map([1, 2, 3, 4, 5], rating => {
				return {
					enabled: (Math.round(album.ratingAverage) >= rating)
				}
			});
			return ratings;

		};

		public selectArtist = (selectedArtistId: number) => {

			var newArtist = new ArtistFilter(selectedArtistId);
			this.artists.push(newArtist);

			if (this.artistRepo) {
				this.artistRepo.getOne(selectedArtistId, artist => {
					newArtist.name(artist.name);
					newArtist.artistType(cls.artists.ArtistType[artist.artistType]);
				});
			}

		};

	}

}