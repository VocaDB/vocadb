import { EntryPictureFileContract } from '@/DataContracts/EntryPictureFileContract';
import { makeObservable, observable } from 'mobx';

export class EntryPictureFileEditStore {
	readonly entryType!: string /* TODO: enum */;
	readonly id!: number;
	readonly mime!: string;
	@observable name: string;
	readonly thumbUrl!: string;

	constructor(data?: EntryPictureFileContract) {
		makeObservable(this);

		if (data) {
			this.entryType = data.entryType;
			this.id = data.id;
			this.mime = data.mime;
			this.thumbUrl = data.thumbUrl;
			this.name = data.name;
		} else {
			this.name = '';
		}
	}
}
