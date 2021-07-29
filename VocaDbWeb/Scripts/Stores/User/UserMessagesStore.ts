import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import UserApiContract from '@DataContracts/User/UserApiContract';
import UserMessageSummaryContract from '@DataContracts/User/UserMessageSummaryContract';
import { UserInboxType } from '@Repositories/UserRepository';
import UserRepository from '@Repositories/UserRepository';
import GlobalValues from '@Shared/GlobalValues';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import PagedItemsStore from '@Stores/PagedItemsStore';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class UserMessageStore {
	@observable public checked = false;
	public readonly created: string;
	public readonly highPriority: boolean;
	public readonly id: number;
	public readonly inbox: string;
	@observable public read: boolean;
	public readonly receiver: UserApiContract;
	@observable public selected = false;

	public readonly sender: UserApiContract;
	public subject: string;

	public constructor(data: UserMessageSummaryContract) {
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
	@observable public body = '';
	@observable public highPriority = false;
	@observable public isReceiverInvalid = false;
	@observable public isSending = false;
	public readonly receiver = new BasicEntryLinkStore<UserApiContract>();
	@observable public subject = '';

	public constructor() {
		makeObservable(this);

		reaction(
			() => this.receiver.id,
			() => {
				this.isReceiverInvalid = false;
			},
		);
	}

	@action public clear = (): void => {
		this.body = '';
		this.highPriority = false;
		this.receiver.clear();
		this.subject = '';
	};

	public toContract = (senderId: number): UserApiContract => {
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
	public readonly anotherUser: BasicEntryLinkStore<UserApiContract>;
	@observable public selectAll = false;
	@observable public unreadOnServer?: number;

	public constructor(
		private readonly values: GlobalValues,
		private readonly userRepo: UserRepository,
		public readonly inbox: UserInboxType,
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
			(selected) =>
				_.forEach(this.items, (m) => {
					m.checked = selected;
				}),
		);

		this.anotherUser = new BasicEntryLinkStore();
		reaction(() => this.anotherUser.id, this.clear);
	}

	@computed public get canFilterByUser(): boolean {
		return (
			this.inbox === UserInboxType.Received || this.inbox === UserInboxType.Sent
		);
	}

	@computed public get unread(): number | undefined {
		return this.items.length
			? _.size(_.filter(this.items, (msg) => !msg.read))
			: this.unreadOnServer;
	}

	public loadMoreItems = (): Promise<
		PartialFindResultContract<UserMessageStore>
	> => {
		return this.userRepo
			.getMessageSummaries({
				userId: this.values.loggedUserId,
				inbox: this.inbox,
				paging: { start: this.start, maxEntries: 100, getTotalCount: true },
				unread: false,
				anotherUserId: this.anotherUser.id,
				iconSize: 40,
			})
			.then((result) => {
				const messageStores = _.map(
					result.items,
					(msg) => new UserMessageStore(msg),
				);
				return { items: messageStores, totalCount: result.totalCount };
			});
	};

	@action public deleteMessage = (message: UserMessageStore): void => {
		this.userRepo.deleteMessage({ messageId: message.id });
		_.pull(this.items, message);
	};

	@action public deleteSelected = (): void => {
		const selected = _.chain(this.items).filter((m) => m.checked);
		const selectedIds = selected.map((m) => m.id).value();

		if (selectedIds.length === 0) return;

		this.userRepo.deleteMessages({
			userId: this.values.loggedUserId,
			messageIds: selectedIds,
		});
		// TODO
	};

	@action public selectMessage = (message: UserMessageStore): void => {
		_.each(this.items, (msg) => {
			if (msg !== message) msg.selected = false;
		});
	};
}

export default class UserMessagesStore {
	private readonly inboxes: UserMessageFolderStore[];
	public newMessageStore = new NewMessageStore();
	public readonly notifications: UserMessageFolderStore;
	public readonly receivedMessages: UserMessageFolderStore;
	public readonly sentMessages: UserMessageFolderStore;
	@observable public selectedMessage?: UserMessageStore;
	@observable public selectedMessageBody = '';

	public constructor(
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

		const inbox = _.find(this.inboxes, (i) => i.inbox === inboxType)!;

		inbox.init(() => {
			if (selectedMessageId) {
				this.selectMessageById(selectedMessageId, inbox);
			}
		});

		if (receiverName) {
			userRepo.getOneByName({ username: receiverName }).then((result) =>
				runInAction(() => {
					this.newMessageStore.receiver.entry = result;
				}),
			);
		}
	}

	@action public selectMessage = (message: UserMessageStore): void => {
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

	@action public reply = (): void => {
		if (!this.selectedMessage) throw Error('No message selected');

		const msg = this.selectedMessage;
		this.newMessageStore.receiver.entry = msg.sender;
		this.newMessageStore.subject =
			msg.subject && msg.subject.indexOf('Re:') === 0
				? msg.subject
				: `Re: ${msg.subject}`;

		// TODO
	};

	public selectMessageById = (
		messageId: number,
		inbox: UserMessageFolderStore,
	): void => {
		const message = _.find(inbox.items, (msg) => msg.id === messageId);

		if (message) {
			// TODO
			this.selectMessage(message);
		}
	};

	@action public sendMessage = (): void => {
		if (this.newMessageStore.receiver.isEmpty) {
			this.newMessageStore.isReceiverInvalid = true;
			return;
		}

		this.newMessageStore.isSending = true;
		const message = this.newMessageStore.toContract(this.userId);
		this.userRepo
			.createMessage({
				userId: this.values.loggedUserId,
				contract: message,
			})
			.then(() => {
				this.newMessageStore.clear();
				this.sentMessages.clear();
				// TODO
				// TODO
			})
			.finally(() =>
				runInAction(() => {
					this.newMessageStore.isSending = false;
				}),
			);
	};
}
