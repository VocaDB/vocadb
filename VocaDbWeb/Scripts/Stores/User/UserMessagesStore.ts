import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { UserMessageSummaryContract } from '@/DataContracts/User/UserMessageSummaryContract';
import { UserInboxType } from '@/Repositories/UserRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { PagedItemsStore } from '@/Stores/PagedItemsStore';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class UserMessageStore {
	@observable checked = false;
	readonly created: string;
	readonly highPriority: boolean;
	readonly id: number;
	readonly inbox: string;
	@observable read: boolean;
	readonly receiver: UserApiContract;
	@observable selected = false;

	readonly sender: UserApiContract;
	subject: string;

	constructor(data: UserMessageSummaryContract) {
		makeObservable(this);

		this.created = data.createdFormatted;
		this.highPriority = data.highPriority;
		this.id = data.id;
		this.inbox = data.inbox;
		this.read = data.read;
		this.receiver = data.receiver;
		this.sender = data.sender!;
		this.subject = data.subject;
	}
}

export class NewMessageStore {
	@observable body = '';
	@observable highPriority = false;
	@observable isReceiverInvalid = false;
	@observable isSending = false;
	readonly receiver: BasicEntryLinkStore<UserApiContract>;
	@observable subject = '';

	constructor(userRepo: UserRepository) {
		makeObservable(this);

		this.receiver = new BasicEntryLinkStore<UserApiContract>((entryId) =>
			userRepo.getOne({ id: entryId }),
		);

		reaction(
			() => this.receiver.id,
			() => {
				this.isReceiverInvalid = false;
			},
		);
	}

	@action clear = (): void => {
		this.body = '';
		this.highPriority = false;
		this.receiver.clear();
		this.subject = '';
	};

	toContract = (senderId: number): UserApiContract => {
		return {
			body: this.body,
			highPriority: this.highPriority,
			receiver: this.receiver.entry,
			sender: { id: senderId } as UserApiContract,
			subject: this.subject,
			id: undefined!,
		} as UserApiContract;
	};
}

export class UserMessageFolderStore extends PagedItemsStore<UserMessageStore> {
	readonly anotherUser: BasicEntryLinkStore<UserApiContract>;
	@observable selectAll = false;
	@observable unreadOnServer?: number;

	constructor(
		private readonly values: GlobalValues,
		private readonly userRepo: UserRepository,
		readonly inbox: UserInboxType,
		getMessageCount: boolean,
	) {
		super();

		makeObservable(this);

		if (getMessageCount) {
			userRepo
				.getMessageSummaries({
					userId: values.loggedUserId,
					inbox: inbox,
					paging: { start: 0, maxEntries: 0, getTotalCount: true },
					unread: true,
					iconSize: undefined,
				})
				.then((result) =>
					runInAction(() => {
						this.unreadOnServer = result.totalCount;
					}),
				);
		}

		reaction(
			() => this.selectAll,
			(selected) => {
				for (const m of this.items) {
					m.checked = selected;
				}
			},
		);

		this.anotherUser = new BasicEntryLinkStore((entryId) =>
			userRepo.getOne({ id: entryId }),
		);
		reaction(() => this.anotherUser.id, this.clear);
	}

	@computed get canFilterByUser(): boolean {
		return (
			this.inbox === UserInboxType.Received || this.inbox === UserInboxType.Sent
		);
	}

	@computed get unread(): number | undefined {
		return this.items.length
			? this.items.filter((msg) => !msg.read).length
			: this.unreadOnServer;
	}

	loadMoreItems = async (): Promise<
		PartialFindResultContract<UserMessageStore>
	> => {
		const result = await this.userRepo.getMessageSummaries({
			userId: this.values.loggedUserId,
			inbox: this.inbox,
			paging: { start: this.start, maxEntries: 100, getTotalCount: true },
			unread: false,
			anotherUserId: this.anotherUser.id,
			iconSize: 40,
		});

		const messageStores = result.items.map((msg) => new UserMessageStore(msg));
		return { items: messageStores, totalCount: result.totalCount };
	};

	@action deleteMessage = (message: UserMessageStore): void => {
		this.userRepo.deleteMessage({ messageId: message.id });
		pull(this.items, message);
	};

	@action deleteSelected = (): void => {
		const selected = this.items.filter((m) => m.checked);
		const selectedIds = selected.map((m) => m.id);

		if (selectedIds.length === 0) return;

		this.userRepo.deleteMessages({
			userId: this.values.loggedUserId,
			messageIds: selectedIds,
		});
		pull(this.items, ...selected);
	};

	@action selectMessage = (message: UserMessageStore): void => {
		for (const msg of this.items) {
			if (msg !== message) msg.selected = false;
		}
	};
}

export class UserMessagesStore {
	private readonly inboxes: UserMessageFolderStore[];
	newMessageStore: NewMessageStore;
	readonly notifications: UserMessageFolderStore;
	readonly receivedMessages: UserMessageFolderStore;
	readonly sentMessages: UserMessageFolderStore;
	@observable selectedMessage?: UserMessageStore;
	@observable selectedMessageBody = '';
	@observable tabName:
		| 'receivedTab'
		| 'notificationsTab'
		| 'sentTab'
		| 'composeTab' = 'receivedTab';

	constructor(
		private readonly values: GlobalValues,
		private readonly userRepo: UserRepository,
		private readonly userId: number,
		inboxType: UserInboxType,
		selectedMessageId?: number,
		receiverName?: string,
	) {
		makeObservable(this);

		this.notifications = new UserMessageFolderStore(
			values,
			userRepo,
			UserInboxType.Notifications,
			inboxType !== UserInboxType.Notifications,
		);
		this.receivedMessages = new UserMessageFolderStore(
			values,
			userRepo,
			UserInboxType.Received,
			inboxType !== UserInboxType.Received,
		);
		this.sentMessages = new UserMessageFolderStore(
			values,
			userRepo,
			UserInboxType.Sent,
			false,
		);

		this.inboxes = [
			this.receivedMessages,
			this.notifications,
			this.sentMessages,
		];

		const inbox = this.inboxes.find((i) => i.inbox === inboxType)!;

		inbox.init(() => {
			if (selectedMessageId) {
				this.selectMessageById(selectedMessageId, inbox);
			}
		});

		this.newMessageStore = new NewMessageStore(userRepo);

		if (receiverName) {
			userRepo.getOneByName({ username: receiverName }).then((result) =>
				runInAction(() => {
					this.newMessageStore.receiver.entry = result;
				}),
			);
		}
	}

	@action selectMessage = (message: UserMessageStore): void => {
		this.userRepo.getMessage({ messageId: message.id }).then((message) =>
			runInAction(() => {
				this.selectedMessageBody = message.body!;
			}),
		);

		this.receivedMessages.selectMessage(message);
		this.sentMessages.selectMessage(message);
		this.notifications.selectMessage(message);

		message.selected = true;
		message.read = true;
		this.selectedMessage = message;
	};

	@action selectTab = (
		tabName: 'receivedTab' | 'notificationsTab' | 'sentTab' | 'composeTab',
	): void => {
		this.tabName = tabName;
	};

	private getInboxTabName = (
		inbox: UserInboxType,
	): 'receivedTab' | 'notificationsTab' | 'sentTab' => {
		switch (inbox) {
			case UserInboxType.Received:
				return 'receivedTab';

			case UserInboxType.Notifications:
				return 'notificationsTab';

			case UserInboxType.Sent:
				return 'sentTab';
		}
	};

	private selectInbox = (inbox: UserInboxType): void => {
		this.selectTab(this.getInboxTabName(inbox));
	};

	@action reply = (): void => {
		if (!this.selectedMessage) throw Error('No message selected');

		const msg = this.selectedMessage;
		this.newMessageStore.receiver.id = msg.sender.id;
		this.newMessageStore.subject =
			msg.subject && msg.subject.indexOf('Re:') === 0
				? msg.subject
				: `Re: ${msg.subject}`;

		this.selectTab('composeTab');
	};

	@action selectMessageById = (
		messageId: number,
		inbox: UserMessageFolderStore,
	): void => {
		const message = inbox.items.find((msg) => msg.id === messageId);

		if (message) {
			this.selectInbox(inbox.inbox);
			this.selectMessage(message);
		}
	};

	@action sendMessage = async (): Promise<void> => {
		this.newMessageStore.isSending = true;
		const message = this.newMessageStore.toContract(this.userId);
		try {
			await this.userRepo.createMessage({
				userId: this.values.loggedUserId,
				contract: message,
			});

			this.newMessageStore.clear();
			this.sentMessages.clear();
			this.selectInbox(UserInboxType.Sent);
		} finally {
			runInAction(() => {
				this.newMessageStore.isSending = false;
			});
		}
	};
}
