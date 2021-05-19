using System;

namespace VocaDb.Model.Domain
{
	/// <remarks>
	/// Saved into database as a bitarray - do not change the number values.
	/// Also remember to update the TypeScript file.
	/// </remarks>
	[Flags]
	public enum WebhookEvents
	{
		Default = 0,

		/// <summary>
		/// New user registered.
		/// </summary>
		User = 1 << 0,

		/// <summary>
		/// Entry report created.
		/// </summary>
		EntryReport = 1 << 1,
	}

	public class Webhook : IEntryWithIntId
	{
		public virtual int Id { get; set; }

		public virtual string Url { get; init; } = string.Empty;

		public virtual WebhookEvents WebhookEvents { get; init; } = WebhookEvents.Default;

		public Webhook() { }

		public Webhook(string url, WebhookEvents webhookEvents)
		{
			Url = url;
			WebhookEvents = webhookEvents;
		}
	}
}
