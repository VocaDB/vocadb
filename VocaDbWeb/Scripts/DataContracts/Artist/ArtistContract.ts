import EntryThumbContract from '@/DataContracts/EntryThumbContract';
import TagUsageForApiContract from '@/DataContracts/Tag/TagUsageForApiContract';
import ArtistType from '@/Models/Artists/ArtistType';

import CommonEntryContract from '../CommonEntryContract';

export default interface ArtistContract extends CommonEntryContract {
	additionalNames?: string;

	artistType: ArtistType;

	mainPicture?: EntryThumbContract;

	releaseDate?: string;

	tags?: TagUsageForApiContract[];
}
