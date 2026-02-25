import { EntryWithTagUsagesContract } from '@/DataContracts/Base/EntryWithTagUsagesContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';

export interface SongListContract
	extends SongListBaseContract,
		EntryWithTagUsagesContract {
	author: UserBaseContract;
	deleted?: boolean;
	description: string;
	eventDate?: string;
	events?: ReleaseEventContract[];
	featuredCategory: SongListFeaturedCategory;
	commentsLocked?: boolean;
	latestComments?: CommentContract[];
	mainPicture?: EntryThumbContract;
	status: EntryStatus;
	tags?: TagUsageForApiContract[];
	version?: number;
}
