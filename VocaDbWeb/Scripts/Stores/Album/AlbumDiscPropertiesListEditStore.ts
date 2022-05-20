import { makeObservable, observable } from 'mobx';

import { DiscMediaType } from '../../DataContracts/Album/AlbumDetailsForApi';
import AlbumDiscPropertiesContract from '../../DataContracts/Album/AlbumDiscPropertiesContract';
import BasicListEditStore from '../BasicListEditStore';

export class AlbumDiscPropertiesEditStore {
	public readonly id: number;
	@observable public mediaType: DiscMediaType;
	@observable public name: string;

	public constructor(contract?: AlbumDiscPropertiesContract) {
		makeObservable(this);

		if (contract) {
			this.id = contract.id;
			this.mediaType = contract.mediaType;
			this.name = contract.name;
		} else {
			this.mediaType = DiscMediaType.Audio;
			this.name = '';
		}
	}
}

export default class AlbumDiscPropertiesListEditStore extends BasicListEditStore<
	AlbumDiscPropertiesEditStore,
	AlbumDiscPropertiesContract
> {
	public constructor(contracts: AlbumDiscPropertiesContract[]) {
		super(AlbumDiscPropertiesEditStore, contracts);
	}
}
