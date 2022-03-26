import EntryThumbContract from '@DataContracts/EntryThumbContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';

import CommonEntryContract from '../CommonEntryContract';

export default interface ArtistContract extends CommonEntryContract {
	additionalNames?: string;

	artistType?: string;

	mainPicture?: EntryThumbContract;

	releaseDate?: string;

	tags?: TagUsageForApiContract[];
}
