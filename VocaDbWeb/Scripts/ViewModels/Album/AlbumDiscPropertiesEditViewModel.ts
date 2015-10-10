
module vdb.viewModels.albums {
	
	import dc = vdb.dataContracts;

	export class AlbumDiscPropertiesEditViewModel {
		
		constructor(contract: dc.albums.AlbumDiscPropertiesContract) {

			if (contract) {
				this.id = contract.id;
				this.mediaType = ko.observable(contract.mediaType);
				this.name = ko.observable(contract.name);
			} else {
				this.mediaType = ko.observable("Audio");
				this.name = ko.observable("");	
			}

		}

		id: number;

		mediaType: KnockoutObservable<string>;

		name: KnockoutObservable<string>;

	}

	export class AlbumDiscPropertiesListEditViewModel extends BasicListEditViewModel<AlbumDiscPropertiesEditViewModel, dc.albums.AlbumDiscPropertiesContract> {
		
		constructor(contracts: dc.albums.AlbumDiscPropertiesContract[]) {
			super(AlbumDiscPropertiesEditViewModel, contracts);
		}

	}

}