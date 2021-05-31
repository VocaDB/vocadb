import ArtistContract from '@DataContracts/Artist/ArtistContract';
import { ArtistAutoCompleteParams } from '@KnockoutExtensions/AutoCompleteParams';
import ArtistRepository from '@Repositories/ArtistRepository';
import vdb from '@Shared/VdbStatic';
import ko, { Observable } from 'knockout';

export default class RequestVerificationViewModel {
  constructor(private readonly artistRepository: ArtistRepository) {}

  public clearArtist = (): void => {
    this.selectedArtist(null!);
  };

  public privateMessage = ko.observable(false);

  public selectedArtist: Observable<ArtistContract | null> = ko.observable(
    null!,
  );

  public setArtist = (targetArtistId?: number): void => {
    this.artistRepository
      .getOne(targetArtistId!, vdb.values.languagePreference)
      .then((artist) => {
        this.selectedArtist(artist);
      });
  };

  public artistSearchParams: ArtistAutoCompleteParams = {
    acceptSelection: this.setArtist,
  };
}
