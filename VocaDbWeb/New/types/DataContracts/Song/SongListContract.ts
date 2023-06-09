import { EntryWithTagUsagesContract } from '@/types/DataContracts/Base/EntryWithTagUsagesContract';
import { CommentContract } from '@/types/DataContracts/CommentContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongListBaseContract } from '@/types/DataContracts/SongListBaseContract';
import { TagUsageForApiContract } from '@/types/DataContracts/Tag/TagUsageForApiContract';
import { UserBaseContract } from '@/types/DataContracts/User/UserBaseContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { SongListFeaturedCategory } from '@/types/Models/SongLists/SongListFeaturedCategory';

export interface SongListContract extends SongListBaseContract, EntryWithTagUsagesContract {
	author: UserBaseContract;
	deleted?: boolean;
	description: string;
	eventDate?: string;
	events?: ReleaseEventContract[];
	featuredCategory: SongListFeaturedCategory;
	latestComments?: CommentContract[];
	mainPicture?: EntryThumbContract;
	status: EntryStatus;
	tags?: TagUsageForApiContract[];
	version?: number;
}
