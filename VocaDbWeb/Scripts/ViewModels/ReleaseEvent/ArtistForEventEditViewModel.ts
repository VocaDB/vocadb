import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ArtistForEventContract from '@DataContracts/ReleaseEvents/ArtistForEventContract';
import ko, { Computed, ObservableArray } from 'knockout';
import _ from 'lodash';

// View model for editing artist for album link.
export default class ArtistForEventEditViewModel
  implements IEditableArtistWithSupport {
  public artist: ArtistContract;

  // Unique link Id.
  public id: number;

  public name: string;

  // Roles as comma-separated string (for serializing to and from .NET enum for the server)
  public roles: Computed<string>;

  // List of roles for this artist.
  public rolesArray: ObservableArray<string>;

  public toContract: () => ArtistForEventContract = () => {
    return {
      artist: this.artist,
      id: this.id,
      name: this.name,
      roles: this.roles(),
    };
  };

  constructor(data: ArtistForEventContract) {
    this.artist = data.artist!;
    this.id = data.id!;

    this.name = data.name!;
    this.rolesArray = ko.observableArray<string>([]);

    this.roles = ko.computed({
      read: () => {
        return this.rolesArray().join();
      },
      write: (value: string) => {
        this.rolesArray(_.map(value.split(','), (val) => val.trim()));
      },
    });

    this.roles(data.roles);
  }
}

export interface IEditableArtistWithSupport {
  rolesArray: ObservableArray<string>;
}
