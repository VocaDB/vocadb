import ArtistType from '../../Models/Artists/ArtistType';

export default class ArtistFilter {
  constructor(public id: number) {}

  artistType = ko.observable<ArtistType>(null);

  name = ko.observable<string>(null);
}
