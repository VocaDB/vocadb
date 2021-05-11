import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';
import CommonEntryContract from '../CommonEntryContract';
import EntryThumbContract from '../EntryThumbContract';

export default interface ArtistApiContract
  extends CommonEntryContract,
    EntryWithTagUsagesContract {
  additionalNames: string;

  artistType: string;

  mainPicture: EntryThumbContract;
}
