import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import UserMessageSummaryContract from '@DataContracts/User/UserMessageSummaryContract';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import EntryReportRepository from '@Repositories/EntryReportRepository';
import UserRepository from '@Repositories/UserRepository';
import { action, computed, makeObservable, observable } from 'mobx';

// Store for the top bar.
export default class TopBarStore {
	@action public ensureMessagesLoaded = (): void => {
		if (this.isLoaded) return;

		this.userRepo
			.getMessageSummaries({
				userId: this.loginManager.loggedUserId,
				inbox: undefined,
				paging: { maxEntries: 3, start: 0, getTotalCount: false },
				unread: true,
				anotherUserId: undefined,
				iconSize: 40,
			})
			.then(
				(messages: PartialFindResultContract<UserMessageSummaryContract>) => {
					this.setUnreadMessages(messages.items);
					this.setIsLoaded(true);
				},
			);
	};

	// entryType: currently selected entry type (for search).
	@observable public entryType = EntryType.Undefined;
	@action public setEntryType = (value: EntryType): void => {
		this.entryType = value;
	};

	@computed public get hasNotifications(): boolean {
		return this.reportCount > 0;
	}

	@observable public isLoaded = false;
	@action public setIsLoaded = (value: boolean): void => {
		this.isLoaded = value;
	};

	@observable public reportCount = 0;
	@action public setReportCount = (value: number): void => {
		this.reportCount = value;
	};

	@observable public searchTerm = '';
	@action public setSearchTerm = (value: string): void => {
		this.searchTerm = value;
	};

	@observable public unreadMessages: UserMessageSummaryContract[] = [];
	@action public setUnreadMessages = (
		value: UserMessageSummaryContract[],
	): void => {
		this.unreadMessages = value;
	};

	// unreadMessagesCount: number of unread received messages (includes notifications).
	@observable public unreadMessagesCount = 0;

	// Initializes store.
	// entryReportRepo: entry reports repository.
	// userRepo: user repository.
	public constructor(
		private readonly loginManager: LoginManager,
		entryReportRepo: EntryReportRepository,
		private readonly userRepo: UserRepository,
	) {
		makeObservable(this);

		// whether to load new reports count (for mods only).
		if (loginManager.canManageEntryReports) {
			entryReportRepo.getNewReportCount({}).then((count) => {
				this.setReportCount(count);
			});
		}
	}
}
