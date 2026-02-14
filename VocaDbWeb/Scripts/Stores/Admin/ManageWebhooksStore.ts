import { WebhookContract } from '@/DataContracts/WebhookContract';
import { AdminRepository } from '@/Repositories/AdminRepository';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

// TODO: Remove.
enum WebhookEvent {
	User = 'User',
	EntryReport = 'EntryReport',
	DuplicateAccount = 'DuplicateAccount',
}

class WebhookEventSelection {
	@observable selected: boolean;

	constructor(
		// Webhook event Id, for example "EntryReport"
		readonly id: WebhookEvent,
		selected: boolean,
	) {
		makeObservable(this);

		this.selected = selected;
	}
}

class WebhookEventsEditStore {
	readonly webhookEventSelections: WebhookEventSelection[];

	constructor() {
		this.webhookEventSelections = Object.values(WebhookEvent).map(
			(webhookEvent) => new WebhookEventSelection(webhookEvent, false),
		);
	}
}

class WebhookEditStore {
	readonly url: string;
	readonly webhookEvents: string;
	readonly webhookEventsArray: string[];

	readonly isNew: boolean;
	@observable isDeleted = false;

	constructor(webhook: WebhookContract, isNew: boolean) {
		makeObservable(this);

		this.url = webhook.url;
		this.webhookEvents = webhook.webhookEvents;
		this.webhookEventsArray = webhook.webhookEvents
			.split(',')
			.map((val) => val.trim());
		this.isNew = isNew;
	}

	@action deleteWebhook = (): void => {
		this.isDeleted = true;
	};
}

export class ManageWebhooksStore {
	@observable newUrl = '';
	@observable submitting = false;
	readonly webhookEventsEditStore: WebhookEventsEditStore;
	@observable webhooks: WebhookEditStore[] = [];

	constructor(private readonly adminRepo: AdminRepository) {
		makeObservable(this);

		this.webhookEventsEditStore = new WebhookEventsEditStore();

		this.loadWebhooks();
	}

	@computed get newWebhookEvents(): string {
		return this.webhookEventsEditStore.webhookEventSelections
			.filter((e) => e.selected)
			.map((e) => e.id)
			.join(',');
	}

	@computed get activeWebhooks(): WebhookEditStore[] {
		return this.webhooks.filter((m) => !m.isDeleted);
	}

	loadWebhooks = async (): Promise<void> => {
		const result = await this.adminRepo.getWebhooks({});

		runInAction(() => {
			this.webhooks = result.map((t) => new WebhookEditStore(t, false));
		});
	};

	@action addWebhook = (): void => {
		if (!this.newUrl || !this.newWebhookEvents) return;

		this.webhooks.push(
			new WebhookEditStore(
				{
					url: this.newUrl,
					webhookEvents: this.newWebhookEvents,
				},
				true,
			),
		);

		this.newUrl = '';
	};

	@action save = async (): Promise<void> => {
		try {
			this.submitting = true;

			const webhooks = this.activeWebhooks;
			await this.adminRepo.saveWebhooks({ webhooks: webhooks });
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
