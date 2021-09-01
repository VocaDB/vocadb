import AlbumDiscPropertiesContract from '@DataContracts/Album/AlbumDiscPropertiesContract';
import BasicListEditStore from '@Stores/BasicListEditStore';
import { makeObservable, observable } from 'mobx';

export default class AlbumDiscPropertiesEditStore {
	public readonly id?: number;
	@observable public mediaType: string;
	@observable public name: string;

	public constructor(contract?: AlbumDiscPropertiesContract) {
		makeObservable(this);

		if (contract) {
			this.id = contract.id;
			this.mediaType = contract.mediaType;
			this.name = contract.name;
		} else {
			this.mediaType = 'Audio';
			this.name = '';
		}
	}
}

export class AlbumDiscPropertiesListEditStore extends BasicListEditStore<
	AlbumDiscPropertiesEditStore,
	AlbumDiscPropertiesContract
> {
	public constructor(contracts: AlbumDiscPropertiesContract[]) {
		super(AlbumDiscPropertiesEditStore, contracts);
	}
}
