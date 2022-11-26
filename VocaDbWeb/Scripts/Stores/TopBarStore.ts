import { UserMessageSummaryContract } from '@/DataContracts/User/UserMessageSummaryContract';
import { EntryType } from '@/Models/EntryType';
import { LoginManager } from '@/Models/LoginManager';
import { EntryReportRepository } from '@/Repositories/EntryReportRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { computed, makeObservable, observable, runInAction } from 'mobx';

// Store for the top bar.
export class TopBarStore {
	static readonly entryTypes = [
		EntryType.Undefined,
		EntryType.Album,
		EntryType.Artist,
		EntryType.ReleaseEvent,
		EntryType.Song,
		EntryType.SongList,
		EntryType.Tag,
		EntryType.User,
	] as const;

	// entryType: currently selected entry type (for search).
	@observable entryType: typeof TopBarStore.entryTypes[number] =
		EntryType.Undefined;
	@observable isLoaded = false;
	@observable reportCount = 0;
	@observable searchTerm = '';
	@observable unreadMessages: UserMessageSummaryContract[] = [];
	// unreadMessagesCount: number of unread received messages (includes notifications).
	@observable unreadMessagesCount = 0;

	// Initializes store.
	// entryReportRepo: entry reports repository.
	// userRepo: user repository.
	constructor(
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

	@computed get hasNotifications(): boolean {
		return this.reportCount > 0;
	}

	ensureMessagesLoaded = async (): Promise<void> => {
		if (this.isLoaded) return;

		const messages = await this.userRepo.getMessageSummaries({
			userId: this.loginManager.loggedUserId,
			inbox: undefined,
			paging: { maxEntries: 3, start: 0, getTotalCount: false },
			unread: true,
			anotherUserId: undefined,
			iconSize: 40,
		});

		runInAction(() => {
			this.unreadMessages = messages.items;
			this.isLoaded = true;
		});
	};
}
