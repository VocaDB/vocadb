
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
			sort: string, artistId: number, albumType: string) {

			super(searchViewModel);

			if (searchViewModel) {
				this.resourceManager = searchViewModel.resourcesManager;
			} else {
				this.resourceManager = new cls.ResourcesManager(resourceRep, cultureCode);
				this.resourceManager.loadResources(null, "albumSortRuleNames", "discTypeNames");
			}

			this.artistSearchParams = { acceptSelection: this.selectArtist };

			this.albumType = ko.observable(albumType || "Unknown");
			this.artistId = ko.observable(artistId);
			this.sort = ko.observable(sort || "Name");

			if (artistId)
				this.selectArtist(artistId);

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.albumType.subscribe(this.updateResultsWithTotalCount);
			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);
			this.childVoicebanks.subscribe(this.updateResultsWithTotalCount);

			this.showChildVoicebanks = ko.computed(() => this.artistId() != null && helpers.ArtistHelper.canHaveChildVoicebanks(this.artistType()));
			this.sortName = ko.computed(() => {
				return this.resourceManager.resources().albumSortRuleNames != null ? this.resourceManager.resources().albumSortRuleNames[this.sort()] : "";
			});

			this.loadResults = (pagingProperties, searchTerm, tag, status, callback) => {

				this.albumRepo.getList(pagingProperties, lang, searchTerm, this.sort(), this.albumType(), tag, this.artistId(),
					this.artistParticipationStatus(), this.childVoicebanks(), this.fields(), status, false, callback);

			}

		}

		public albumType: KnockoutObservable<string>;
		public artistId: KnockoutObservable<number>;
		public artistName = ko.observable("");
		public artistParticipationStatus = ko.observable("Everything");
		public artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
		public artistType = ko.observable<cls.artists.ArtistType>(null);
		public childVoicebanks = ko.observable(false);
		private resourceManager: cls.ResourcesManager;
		public showChildVoicebanks: KnockoutComputed<boolean>;
		public sort: KnockoutObservable<string>;
		public sortName: KnockoutComputed<string>;
		public viewMode = ko.observable("Details");

		public discTypeName = (discTypeStr: string) => this.resourceManager.resources().discTypeNames != null ? this.resourceManager.resources().discTypeNames[discTypeStr] : "";

		public fields = ko.computed(() => this.showTags() ? "MainPicture,Tags" : "MainPicture");

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
			this.artistId(selectedArtistId);
			this.artistType(null);
			if (this.artistRepo) {
				this.artistRepo.getOne(selectedArtistId, artist => {
					this.artistName(artist.name);
					this.artistType(cls.artists.ArtistType[artist.artistType]);
				});				
			}
		};

	}

}