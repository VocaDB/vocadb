import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import EntryPictureFileContract from '../DataContracts/EntryPictureFileContract';

export class EntryPictureFileEditStore {
	public readonly entryType: string;
	public readonly id: number;
	public readonly mime: string;
	@observable public name: string;
	public readonly thumbUrl: string;

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

export default class EntryPictureFileListEditStore {
	@observable public pictures: EntryPictureFileEditStore[];

	public constructor(pictures: EntryPictureFileContract[]) {
		makeObservable(this);

		this.pictures = _.map(
			pictures,
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
		return this.pictures;
	};
}
