import AlbumDiscPropertiesContract from '@DataContracts/Album/AlbumDiscPropertiesContract';
import ko, { Observable } from 'knockout';

import BasicListEditViewModel from '../BasicListEditViewModel';

export default class AlbumDiscPropertiesEditViewModel {
  constructor(contract?: AlbumDiscPropertiesContract) {
    if (contract) {
      this.id = contract.id;
      this.mediaType = ko.observable(contract.mediaType);
      this.name = ko.observable(contract.name);
    } else {
      this.mediaType = ko.observable('Audio');
      this.name = ko.observable('');
    }
  }

  id!: number;

  mediaType: Observable<string>;

  name: Observable<string>;
}

export class AlbumDiscPropertiesListEditViewModel extends BasicListEditViewModel<
  AlbumDiscPropertiesEditViewModel,
  AlbumDiscPropertiesContract
> {
  constructor(contracts: AlbumDiscPropertiesContract[]) {
    super(AlbumDiscPropertiesEditViewModel, contracts);
  }
}
