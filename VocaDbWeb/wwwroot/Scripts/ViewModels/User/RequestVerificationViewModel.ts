import { ArtistAutoCompleteParams } from '../../KnockoutExtensions/AutoCompleteParams';
import ArtistContract from '../../DataContracts/Artist/ArtistContract';
import ArtistRepository from '../../Repositories/ArtistRepository';

export default class RequestVerificationViewModel {
  constructor(private readonly artistRepository: ArtistRepository) {}

  public clearArtist = () => {
    this.selectedArtist(null);
  };

  public privateMessage = ko.observable(false);

  public selectedArtist: KnockoutObservable<ArtistContract> = ko.observable(
    null,
  );

  public setArtist = (targetArtistId) => {
    this.artistRepository.getOne(targetArtistId).then((artist) => {
      this.selectedArtist(artist);
    });
  };

  public artistSearchParams: ArtistAutoCompleteParams = {
    acceptSelection: this.setArtist,
  };
}
