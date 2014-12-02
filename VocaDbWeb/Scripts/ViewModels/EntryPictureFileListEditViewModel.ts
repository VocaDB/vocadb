
module vdb.viewModels {

	import dc = vdb.dataContracts;
	
	export class EntryPictureFileListEditViewModel {

		constructor(pictures: dc.EntryPictureFileContract[]) {

			this.pictures = ko.observableArray(_.map(pictures, picture => new EntryPictureFileEditViewModel(picture)));

		}
		
		public add = () => {
			this.pictures.push(new EntryPictureFileEditViewModel());
		}

		public pictures: KnockoutObservableArray<EntryPictureFileEditViewModel>;

		public remove = (picture: EntryPictureFileEditViewModel) => {
			this.pictures.remove(picture);
		}

		public toContracts: () => dc.EntryPictureFileContract[] = () => {

			return ko.toJS(this.pictures());

		}

	}

}