
import AlbumDiscPropertiesContract from '../../DataContracts/Album/AlbumDiscPropertiesContract';
import BasicListEditViewModel from '../BasicListEditViewModel';

//module vdb.viewModels.albums {
	
	export default class AlbumDiscPropertiesEditViewModel {
		
		constructor(contract: AlbumDiscPropertiesContract) {

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

	export class AlbumDiscPropertiesListEditViewModel extends BasicListEditViewModel<AlbumDiscPropertiesEditViewModel, AlbumDiscPropertiesContract> {
		
		constructor(contracts: AlbumDiscPropertiesContract[]) {
			super(AlbumDiscPropertiesEditViewModel, contracts);
		}

	}

//}