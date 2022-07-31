import EntryWithTagUsagesContract from '@DataContracts/Base/EntryWithTagUsagesContract';
import CommentContract from '@DataContracts/CommentContract';
import EntryThumbContract from '@DataContracts/EntryThumbContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import SongListFeaturedCategory from '@Models/SongLists/SongListFeaturedCategory';

import SongListBaseContract from '../SongListBaseContract';
import UserBaseContract from '../User/UserBaseContract';

export default interface SongListContract
	extends SongListBaseContract,
		EntryWithTagUsagesContract {
	author: UserBaseContract;

	deleted?: boolean;

	description: string;

	eventDate?: string;

	events?: ReleaseEventContract[];

	featuredCategory: SongListFeaturedCategory;

	latestComments?: CommentContract[];

	mainPicture?: EntryThumbContract;

	status: string;

	tags?: TagUsageForApiContract[];

	version?: number;
}
