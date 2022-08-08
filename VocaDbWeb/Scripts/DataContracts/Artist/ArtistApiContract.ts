import ArtistType from '@/Models/Artists/ArtistType';

import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';
import CommonEntryContract from '../CommonEntryContract';
import EntryThumbContract from '../EntryThumbContract';

export default interface ArtistApiContract
	extends CommonEntryContract,
		EntryWithTagUsagesContract {
	additionalNames: string;

	artistType: ArtistType;

	mainPicture: EntryThumbContract;
}
