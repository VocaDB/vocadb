import { EntryPictureFileContract } from '@/DataContracts/EntryPictureFileContract';
import { EntryPictureFileEditStore } from '@/Stores/EntryPictureFileEditStore';
import { pull } from 'lodash-es';
import { action, makeObservable, observable } from 'mobx';

export class EntryPictureFileListEditStore {
	@observable readonly pictures: EntryPictureFileEditStore[];

	constructor(pictures: EntryPictureFileContract[]) {
		makeObservable(this);

		this.pictures = pictures.map(
			(picture) => new EntryPictureFileEditStore(picture),
		);
	}

	@action add = (): void => {
		this.pictures.push(new EntryPictureFileEditStore());
	};

	@action remove = (picture: EntryPictureFileEditStore): void => {
		pull(this.pictures, picture);
	};

	toContracts = (): EntryPictureFileContract[] => {
		return this.pictures as EntryPictureFileContract[];
	};
}
