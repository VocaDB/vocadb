import { CommonEntryContract } from '@/types/DataContracts/CommonEntryContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { ArtistType } from '@/types/Models/Artists/ArtistType';

export interface ArtistContract extends CommonEntryContract {
	additionalNames?: string;
	artistType: ArtistType;
	mainPicture?: EntryThumbContract;
	releaseDate?: string;
	tags?: TagUsageForApiContract[];
}
