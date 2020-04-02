import CommonEntryContract from '../CommonEntryContract';
import EntryThumbContract from '../EntryThumbContract';
import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';

	export default interface ArtistApiContract extends CommonEntryContract, EntryWithTagUsagesContract {

		additionalNames: string;

		artistType: string;

		mainPicture: EntryThumbContract;

	}