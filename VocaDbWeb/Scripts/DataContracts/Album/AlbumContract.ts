import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import AlbumType from '@Models/Albums/AlbumType';

import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';
import CommonEntryContract from '../CommonEntryContract';
import EntryThumbContract from '../EntryThumbContract';
import OptionalDateTimeContract from '../OptionalDateTimeContract';

export default interface AlbumContract
	extends CommonEntryContract,
		EntryWithTagUsagesContract {
	additionalNames: string;

	artistString: string;

	discType: AlbumType;

	mainPicture: EntryThumbContract;

	ratingAverage: number;

	ratingCount: number;

	releaseDate: OptionalDateTimeContract;

	releaseEvent?: ReleaseEventContract;
}
