import CommonEntryContract from '../CommonEntryContract';

export default interface ArtistContract extends CommonEntryContract {
  additionalNames?: string;

  artistType?: string;
}
