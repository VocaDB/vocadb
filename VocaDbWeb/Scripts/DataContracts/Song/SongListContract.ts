import EntryWithTagUsagesContract from '@DataContracts/Base/EntryWithTagUsagesContract';
import EntryThumbContract from '@DataContracts/EntryThumbContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';

import SongListBaseContract from '../SongListBaseContract';
import UserBaseContract from '../User/UserBaseContract';

export default interface SongListContract
	extends SongListBaseContract,
		EntryWithTagUsagesContract {
	author: UserBaseContract;

	description: string;

	eventDate?: string;

	featuredCategory: string;

	mainPicture?: EntryThumbContract;

	status: string;

	tags?: TagUsageForApiContract[];
}
