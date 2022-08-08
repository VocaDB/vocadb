import EntryPictureFileContract from '@/DataContracts/EntryPictureFileContract';
import EntryPictureFileEditViewModel from '@/ViewModels/EntryPictureFileEditViewModel';
import ko, { ObservableArray } from 'knockout';

export default class EntryPictureFileListEditViewModel {
	public constructor(pictures: EntryPictureFileContract[]) {
		this.pictures = ko.observableArray(
			pictures.map((picture) => new EntryPictureFileEditViewModel(picture)),
		);
	}

	public add = (): void => {
		this.pictures.push(new EntryPictureFileEditViewModel());
	};

	public pictures: ObservableArray<EntryPictureFileEditViewModel>;

	public remove = (picture: EntryPictureFileEditViewModel): void => {
		this.pictures.remove(picture);
	};

	public toContracts: () => EntryPictureFileContract[] = () => {
		return ko.toJS(this.pictures()) as EntryPictureFileContract[];
	};
}
