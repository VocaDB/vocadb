import AlbumDiscPropertiesContract from '@DataContracts/Album/AlbumDiscPropertiesContract';
import ko, { Observable } from 'knockout';

import BasicListEditViewModel from '../BasicListEditViewModel';

export default class AlbumDiscPropertiesEditViewModel {
	public constructor(contract?: AlbumDiscPropertiesContract) {
		if (contract) {
			this.id = contract.id;
			this.mediaType = ko.observable(contract.mediaType);
			this.name = ko.observable(contract.name);
		} else {
			this.mediaType = ko.observable('Audio');
			this.name = ko.observable('');
		}
	}

	public id!: number;

	public mediaType: Observable<string>;

	public name: Observable<string>;
}

export class AlbumDiscPropertiesListEditViewModel extends BasicListEditViewModel<
	AlbumDiscPropertiesEditViewModel,
	AlbumDiscPropertiesContract
> {
	public constructor(contracts: AlbumDiscPropertiesContract[]) {
		super(AlbumDiscPropertiesEditViewModel, contracts);
	}
}
