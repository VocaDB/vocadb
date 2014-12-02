
module vdb.viewModels.search {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class AlbumSearchViewModel extends SearchCategoryBaseViewModel<dc.AlbumContract> {

		constructor(searchViewModel: SearchViewModel,
			private unknownPictureUrl: string,
			lang: string, private albumRepo: rep.AlbumRepository,
			private artistRepo: rep.ArtistRepository, sort: string, artistId: number, albumType: string) {

			super(searchViewModel);

			this.artistSearchParams = {
				allowCreateNew: false,
				acceptSelection: this.selectArtist,
				height: 300
			};

			if (sort)
				this.sort(sort);

			if (artistId)
				this.selectArtist(artistId);

			if (albumType)
				this.albumType(albumType);

			this.sort.subscribe(this.updateResultsWithTotalCount);
			this.albumType.subscribe(this.updateResultsWithTotalCount);
			this.artistId.subscribe(this.updateResultsWithTotalCount);
			this.artistParticipationStatus.subscribe(this.updateResultsWithTotalCount);
			this.childVoicebanks.subscribe(this.updateResultsWithTotalCount);

			this.showChildVoicebanks = ko.computed(() => this.artistId() != null && helpers.ArtistHelper.canHaveChildVoicebanks(this.artistType()));

			this.loadResults = (pagingProperties, searchTerm, tag, status, callback) => {

				this.albumRepo.getList(pagingProperties, lang, searchTerm, this.sort(), this.albumType(), tag, this.artistId(),
					this.artistParticipationStatus(), this.childVoicebanks(), this.fields(), status, callback);

			}

		}

		public albumType = ko.observable("Unknown");
		public artistId = ko.observable<number>(null);
		public artistName = ko.observable("");
		public artistParticipationStatus = ko.observable("Everything");
		public artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams;
		public artistType = ko.observable<cls.artists.ArtistType>(null);
		public childVoicebanks = ko.observable(false);
		public showChildVoicebanks: KnockoutComputed<boolean>;
		public sort = ko.observable("Name");
		public sortName = ko.computed(() => this.searchViewModel.resources() != null ? this.searchViewModel.resources().albumSortRuleNames[this.sort()] : "");
		public viewMode = ko.observable("Details");

		public fields = ko.computed(() => this.searchViewModel.showTags() ? "MainPicture,Tags" : "MainPicture");

		public ratingStars = (album: dc.AlbumContract) => {

			if (!album)
				return [];

			var ratings = _.map([1, 2, 3, 4, 5], rating => { return { enabled: (Math.round(album.ratingAverage) >= rating) } });
			return ratings;

		};

		public selectArtist = (selectedArtistId: number) => {
			this.artistId(selectedArtistId);
			this.artistType(null);
			this.artistRepo.getOne(selectedArtistId, artist => {
				this.artistName(artist.name);
				this.artistType(cls.artists.ArtistType[artist.artistType]);
			});
		};

	}

}