
module vdb.viewModels.albums {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class AlbumMergeViewModel {

		constructor(repo: rep.AlbumRepository, id: number) {

			this.target = new BasicEntryLinkViewModel(null, repo.getOne);

			this.targetSearchParams = {
				acceptSelection: this.target.id,
				ignoreId: id
			};

		}

		public target: BasicEntryLinkViewModel<dc.AlbumContract>;
		public targetSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;

	}

} 