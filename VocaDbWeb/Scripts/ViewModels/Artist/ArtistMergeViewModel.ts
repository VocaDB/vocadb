
module vdb.viewModels.artists {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class ArtistMergeViewModel {

		constructor(repo: rep.ArtistRepository, id: number) {

			this.target = new BasicEntryLinkViewModel(null, repo.getOne);

			this.targetSearchParams = {
				acceptSelection: this.target.id,
				allowCreateNew: false,
				ignoreId: id
			};

		}

		public target: BasicEntryLinkViewModel<dc.ArtistContract>;
		public targetSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;

	}

} 