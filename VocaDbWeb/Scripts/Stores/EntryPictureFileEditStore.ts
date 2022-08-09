import { EntryPictureFileContract } from '@/DataContracts/EntryPictureFileContract';
import { makeObservable, observable } from 'mobx';

export class EntryPictureFileEditStore {
	public readonly entryType!: string /* TODO: enum */;
	public readonly id!: number;
	public readonly mime!: string;
	@observable public name: string;
	public readonly thumbUrl!: string;

	public constructor(data?: EntryPictureFileContract) {
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
