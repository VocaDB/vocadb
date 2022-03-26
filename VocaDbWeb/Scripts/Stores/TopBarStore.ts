import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import UserMessageSummaryContract from '@DataContracts/User/UserMessageSummaryContract';
import EntryType from '@Models/EntryType';
import LoginManager from '@Models/LoginManager';
import EntryReportRepository from '@Repositories/EntryReportRepository';
import UserRepository from '@Repositories/UserRepository';
import { computed, makeObservable, observable, runInAction } from 'mobx';

// Store for the top bar.
export default class TopBarStore {
	// entryType: currently selected entry type (for search).
	@observable public entryType = EntryType.Undefined;
	@observable public isLoaded = false;
	@observable public reportCount = 0;
	@observable public searchTerm = '';
	@observable public unreadMessages: UserMessageSummaryContract[] = [];
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
				runInAction(() => {
					this.reportCount = count;
				});
			});
		}
	}

	@computed public get hasNotifications(): boolean {
		return this.reportCount > 0;
	}

	public ensureMessagesLoaded = (): void => {
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
					runInAction(() => {
						this.unreadMessages = messages.items;
						this.isLoaded = true;
					});
				},
			);
	};
}
