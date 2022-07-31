import EntryPictureFileContract from '@DataContracts/EntryPictureFileContract';
import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import EntryPictureFileEditStore from './EntryPictureFileEditStore';

export default class EntryPictureFileListEditStore {
	@observable public readonly pictures: EntryPictureFileEditStore[];

	public constructor(pictures: EntryPictureFileContract[]) {
		makeObservable(this);

		this.pictures = pictures.map(
			(picture) => new EntryPictureFileEditStore(picture),
		);
	}

	@action public add = (): void => {
		this.pictures.push(new EntryPictureFileEditStore());
	};

	@action public remove = (picture: EntryPictureFileEditStore): void => {
		_.pull(this.pictures, picture);
	};

	public toContracts = (): EntryPictureFileContract[] => {
		return this.pictures as EntryPictureFileContract[];
	};
}
