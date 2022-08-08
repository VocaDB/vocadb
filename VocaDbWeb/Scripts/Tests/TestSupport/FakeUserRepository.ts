import PagingProperties from '@/DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@/DataContracts/PartialFindResultContract';
import EntryEditDataContract from '@/DataContracts/User/EntryEditDataContract';
import UserMessageSummaryContract from '@/DataContracts/User/UserMessageSummaryContract';
import EntryType from '@/Models/EntryType';
import SongVoteRating from '@/Models/SongVoteRating';
import { UserInboxType } from '@/Repositories/UserRepository';
import UserRepository from '@/Repositories/UserRepository';
import HttpClient from '@/Shared/HttpClient';
import UrlMapper from '@/Shared/UrlMapper';
import FakePromise from '@/Tests/TestSupport/FakePromise';

export default class FakeUserRepository extends UserRepository {
	public message!: UserMessageSummaryContract;
	public messages!: UserMessageSummaryContract[];
	public songId!: number;
	public rating!: SongVoteRating;

	public constructor() {
		super(new HttpClient(), new UrlMapper(''));

		this.getMessage = ({
			messageId,
		}: {
			messageId: number;
		}): Promise<UserMessageSummaryContract> => {
			return FakePromise.resolve(this.message);
		};

		this.getMessageSummaries = ({
			userId,
			inbox,
			paging,
			unread = false,
			anotherUserId,
			iconSize = 40,
		}: {
			userId: number;
			inbox?: UserInboxType;
			paging: PagingProperties;
			unread: boolean;
			anotherUserId?: number;
			iconSize?: number;
		}): Promise<PartialFindResultContract<UserMessageSummaryContract>> => {
			return FakePromise.resolve<
				PartialFindResultContract<UserMessageSummaryContract>
			>({
				items: this.messages,
				totalCount: this.messages ? this.messages.length : 0,
			});
		};

		this.refreshEntryEdit = ({
			entryType,
			entryId,
		}: {
			entryType: EntryType;
			entryId: number;
		}): Promise<EntryEditDataContract> => {
			return Promise.resolve({ time: '0001-01-01T00:00:00', userId: 0 });
		};

		this.updateSongRating = ({
			songId,
			rating,
		}: {
			songId: number;
			rating: SongVoteRating;
		}): Promise<void> => {
			this.songId = songId;
			this.rating = rating;

			return Promise.resolve();
		};
	}
}
