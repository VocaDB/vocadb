import { DiscMediaType } from '@/DataContracts/Album/AlbumDetailsForApi';
import { AlbumDiscPropertiesContract } from '@/DataContracts/Album/AlbumDiscPropertiesContract';
import { BasicListEditStore } from '@/Stores/BasicListEditStore';
import { makeObservable, observable } from 'mobx';

export class AlbumDiscPropertiesEditStore {
	readonly id!: number;
	@observable mediaType: DiscMediaType;
	@observable name: string;

	constructor(contract?: AlbumDiscPropertiesContract) {
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

export class AlbumDiscPropertiesListEditStore extends BasicListEditStore<
	AlbumDiscPropertiesEditStore,
	AlbumDiscPropertiesContract
> {
	constructor(contracts: AlbumDiscPropertiesContract[]) {
		super(AlbumDiscPropertiesEditStore, contracts);
	}
}
