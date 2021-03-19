import CommonEntryContract from '../CommonEntryContract';
import EntryThumbContract from '../EntryThumbContract';
import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';
import OptionalDateTimeContract from '../OptionalDateTimeContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';

	export default interface AlbumContract extends CommonEntryContract, EntryWithTagUsagesContract {

		additionalNames: string;

		artistString: string;

		discType: string;

		mainPicture: EntryThumbContract;

		ratingAverage: number;

		ratingCount: number;

		releaseDate: OptionalDateTimeContract;

		releaseEvent?: ReleaseEventContract;

	}