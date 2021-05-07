import EntryPictureFileContract from '../DataContracts/EntryPictureFileContract';
import EntryPictureFileEditViewModel from './EntryPictureFileEditViewModel';

export default class EntryPictureFileListEditViewModel {
  constructor(pictures: EntryPictureFileContract[]) {
    this.pictures = ko.observableArray(
      _.map(pictures, (picture) => new EntryPictureFileEditViewModel(picture)),
    );
  }

  public add = (): void => {
    this.pictures.push(new EntryPictureFileEditViewModel());
  };

  public pictures: KnockoutObservableArray<EntryPictureFileEditViewModel>;

  public remove = (picture: EntryPictureFileEditViewModel): void => {
    this.pictures.remove(picture);
  };

  public toContracts: () => EntryPictureFileContract[] = () => {
    return ko.toJS(this.pictures());
  };
}
