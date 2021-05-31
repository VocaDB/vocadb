import WebhookContract, { WebhookEvents } from '@DataContracts/WebhookContract';
import AdminRepository from '@Repositories/AdminRepository';
import ui from '@Shared/MessagesTyped';
import ko, { Computed, Observable } from 'knockout';
import _ from 'lodash';

interface WebhookEventSelection {
	// Webhook event Id, for example "EntryReport"
	id: string;

	// User-visible webhook event name, for example "Entry report"
	name: string;

	selected: Observable<boolean>;
}

class WebhookEventsEditViewModel {
	public webhookEventSelections: WebhookEventSelection[];

	constructor(
		webhookEventNames: { [key: string]: string },
		defaultWebhookEventName: string,
	) {
		var webhookEventSelections: WebhookEventSelection[] = [];
		for (var webhookEvent in webhookEventNames) {
			if (
				webhookEvent !== defaultWebhookEventName &&
				webhookEventNames.hasOwnProperty(webhookEvent)
			) {
				webhookEventSelections.push({
					id: webhookEvent,
					name: webhookEventNames[webhookEvent],
					selected: ko.observable(false),
				});
			}
		}

		this.webhookEventSelections = webhookEventSelections;
	}
}

class WebhookEditViewModel {
	public url: string;
	public webhookEvents: string;
	public webhookEventsArray: string[];

	public isNew: boolean;
	public isDeleted = ko.observable(false);

	constructor(
		webhook: WebhookContract,
		isNew: boolean,
		webhookEventNames: { [key: string]: string },
	) {
		this.url = webhook.url;
		this.webhookEvents = webhook.webhookEvents;
		this.webhookEventsArray = _.map(webhook.webhookEvents.split(','), (val) =>
			val.trim(),
		);
		this.isNew = isNew;
	}

	public deleteWebhook = (): void => this.isDeleted(true);
}

export default class ManageWebhooksViewModel {
	public newUrl = ko.observable('');

	public webhookEventsEditViewModel: WebhookEventsEditViewModel;

	public newWebhookEvents: Computed<string>;

	private getEnumValues = <TEnum>(
		Enum: any,
		selected?: Array<TEnum>,
	): string[] =>
		Object.keys(Enum).filter(
			(k) =>
				(!selected || _.includes(selected, Enum[k])) &&
				typeof Enum[k as any] === 'number',
		);

	public loadWebhooks: () => Promise<void>;

	public translateWebhookEvent: (webhookEvent: string) => string;

	constructor(
		private readonly webhookEventNames: { [key: string]: string },
		private readonly adminRepo: AdminRepository,
	) {
		this.webhookEventsEditViewModel = new WebhookEventsEditViewModel(
			webhookEventNames,
			WebhookEvents[WebhookEvents.Default],
		);

		this.newWebhookEvents = ko.computed(() => {
			return _.chain(this.webhookEventsEditViewModel.webhookEventSelections)
				.filter((e) => e.selected())
				.map((e) => e.id)
				.value()
				.join(',');
		});

		this.loadWebhooks = async (): Promise<void> => {
			const result = await this.adminRepo.getWebhooks();
			this.webhooks(
				_.map(
					result,
					(t) => new WebhookEditViewModel(t, false, webhookEventNames),
				),
			);
		};

		this.translateWebhookEvent = (webhookEvent: string): string =>
			webhookEventNames[webhookEvent];

		this.loadWebhooks();
	}

	public webhooks = ko.observableArray<WebhookEditViewModel>([]);

	public addWebhook = (): void => {
		if (!this.newUrl() || !this.newWebhookEvents()) return;

		if (_.some(this.webhooks(), (w) => w.url === this.newUrl())) {
			ui.showErrorMessage('Hook already exists');
			return;
		}

		this.webhooks.push(
			new WebhookEditViewModel(
				{
					url: this.newUrl(),
					webhookEvents: this.newWebhookEvents(),
				},
				true,
				this.webhookEventNames,
			),
		);

		this.newUrl('');
	};

	public activeWebhooks = ko.computed(() =>
		_.filter(this.webhooks(), (m) => !m.isDeleted()),
	);

	public save = async (): Promise<void> => {
		const webhooks = this.activeWebhooks();
		await this.adminRepo.saveWebhooks(webhooks);
		ui.showSuccessMessage('Saved');
		await this.loadWebhooks();
	};
}
