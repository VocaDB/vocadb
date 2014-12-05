
module vdb.viewModels.songs {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class SongMergeViewModel {
		
		constructor(songRepo: rep.SongRepository, id: number) {

			this.target = new BasicEntryLinkViewModel(null, songRepo.getOne);

			this.targetSearchParams = {
				acceptSelection: this.target.id,
				allowCreateNew: false,
				ignoreId: id
			};

		}

		public target: BasicEntryLinkViewModel<dc.SongContract>;
		public targetSearchParams: vdb.knockoutExtensions.SongAutoCompleteParams;

	}

}