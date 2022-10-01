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
}

class WebhookEventSelection {
	@observable public selected: boolean;

	public constructor(
		// Webhook event Id, for example "EntryReport"
		public readonly id: WebhookEvent,
		selected: boolean,
	) {
		makeObservable(this);

		this.selected = selected;
	}
}

class WebhookEventsEditStore {
	public readonly webhookEventSelections: WebhookEventSelection[];

	public constructor() {
		this.webhookEventSelections = Object.values(WebhookEvent).map(
			(webhookEvent) => new WebhookEventSelection(webhookEvent, false),
		);
	}
}

class WebhookEditStore {
	public readonly url: string;
	public readonly webhookEvents: string;
	public readonly webhookEventsArray: string[];

	public readonly isNew: boolean;
	@observable public isDeleted = false;

	public constructor(webhook: WebhookContract, isNew: boolean) {
		makeObservable(this);

		this.url = webhook.url;
		this.webhookEvents = webhook.webhookEvents;
		this.webhookEventsArray = webhook.webhookEvents
			.split(',')
			.map((val) => val.trim());
		this.isNew = isNew;
	}

	@action public deleteWebhook = (): void => {
		this.isDeleted = true;
	};
}

export class ManageWebhooksStore {
	@observable public newUrl = '';
	@observable public submitting = false;
	public readonly webhookEventsEditStore: WebhookEventsEditStore;
	@observable public webhooks: WebhookEditStore[] = [];

	public constructor(private readonly adminRepo: AdminRepository) {
		makeObservable(this);

		this.webhookEventsEditStore = new WebhookEventsEditStore();

		this.loadWebhooks();
	}

	@computed public get newWebhookEvents(): string {
		return this.webhookEventsEditStore.webhookEventSelections
			.filter((e) => e.selected)
			.map((e) => e.id)
			.join(',');
	}

	@computed public get activeWebhooks(): WebhookEditStore[] {
		return this.webhooks.filter((m) => !m.isDeleted);
	}

	public loadWebhooks = async (): Promise<void> => {
		const result = await this.adminRepo.getWebhooks({});

		runInAction(() => {
			this.webhooks = result.map((t) => new WebhookEditStore(t, false));
		});
	};

	@action public addWebhook = (): void => {
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

	@action public save = async (): Promise<void> => {
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
