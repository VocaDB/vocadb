import WebhookContract, { WebhookEvents } from '@DataContracts/WebhookContract';
import ui from '@Shared/MessagesTyped';

enum WebhookContentType {
  Form,
  Json,
}

interface WebhookEventSelection {
  // Webhook event Id, for example "EntryReport"
  id: string;

  // User-visible webhook event name, for example "Entry report"
  name: string;

  selected: KnockoutObservable<boolean>;
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

    this.webhookEventSelections = _.sortBy(
      webhookEventSelections,
      (s) => s.name,
    );
  }
}

class WebhookEditViewModel {
  public url: string;
  public contentType: string;
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
    this.contentType = webhook.contentType;
    this.webhookEvents = webhook.webhookEvents;
    this.webhookEventsArray = webhook.webhookEvents.split(',');
    this.isNew = isNew;
  }

  public deleteWebhook = (): void => this.isDeleted(true);
}

export default class ManageWebhooksViewModel {
  public newUrl = ko.observable('');
  public newContentType = ko.observable(
    WebhookContentType[WebhookContentType.Form],
  );

  public webhookEventsEditViewModel: WebhookEventsEditViewModel;

  public newWebhookEvents: KnockoutObservable<string>;

  private getEnumValues = <TEnum>(
    Enum: any,
    selected?: Array<TEnum>,
  ): string[] =>
    Object.keys(Enum).filter(
      (k) =>
        (!selected || _.includes(selected, Enum[k])) &&
        typeof Enum[k as any] === 'number',
    );

  public contentTypes = this.getEnumValues<WebhookContentType>(
    WebhookContentType,
    [WebhookContentType.Form, WebhookContentType.Json],
  );

  public translateWebhookEvent: (webhookEvent: string) => string;

  constructor(private readonly webhookEventNames: { [key: string]: string }) {
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

    this.translateWebhookEvent = (webhookEvent: string): string =>
      webhookEventNames[webhookEvent];
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
          contentType: this.newContentType(),
          webhookEvents: this.newWebhookEvents(),
        },
        true,
        this.webhookEventNames,
      ),
    );

    this.newUrl('');
  };

  public save = (): Promise<void> => {
    return Promise.resolve();
  };
}
