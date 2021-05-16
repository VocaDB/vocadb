using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts
{
	public sealed record WebhookContract
	{
		public string Url { get; init; } = string.Empty;

		public WebhookContentType ContentType { get; init; } = WebhookContentType.Form;

		[JsonConverter(typeof(StringEnumConverter))]
		public WebhookEvents WebhookEvents { get; init; } = WebhookEvents.User;

		public WebhookContract() { }

		public WebhookContract(Webhook webhook)
		{
			Url = webhook.Url;
			ContentType = webhook.ContentType;
			WebhookEvents = webhook.WebhookEvents;
		}
	}
}
