import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import UserMessageSummaryContract from '@DataContracts/User/UserMessageSummaryContract';
import EntryType from '@Models/EntryType';
import SongVoteRating from '@Models/SongVoteRating';
import { UserInboxType } from '@Repositories/UserRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';

import FakePromise from './FakePromise';

export default class FakeUserRepository extends UserRepository {
	public message!: UserMessageSummaryContract;
	public messages!: UserMessageSummaryContract[];
	public songId!: number;
	public rating!: SongVoteRating;

	constructor() {
		super(new HttpClient());

		this.getMessage = (messageId): Promise<UserMessageSummaryContract> => {
			return FakePromise.resolve(this.message);
		};

		this.getMessageSummaries = (
			userId: number,
			inbox: UserInboxType,
			maxCount?,
			unread?,
			anotherUserId?,
			iconSize?,
		): Promise<PartialFindResultContract<UserMessageSummaryContract>> => {
			return FakePromise.resolve<
				PartialFindResultContract<UserMessageSummaryContract>
			>({
				items: this.messages,
				totalCount: this.messages ? this.messages.length : 0,
			});
		};

		this.refreshEntryEdit = (
			entryType: EntryType,
			entryId: number,
		): Promise<void> => {
			return Promise.resolve();
		};

		this.updateSongRating = (
			songId: number,
			rating: SongVoteRating,
		): Promise<void> => {
			this.songId = songId;
			this.rating = rating;

			return Promise.resolve();
		};
	}
}
