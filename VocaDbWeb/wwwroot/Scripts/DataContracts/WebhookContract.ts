export enum WebhookEvents {
  Default = 0,
  User = 1 << 0,
  EntryReport = 1 << 1,
}

export default interface WebhookContract {
  url: string;
  contentType: string;
  webhookEvents: string;
}
