using System;

namespace VocaDb.Model.Domain
{
	/// <remarks>
	/// Do not persist the numeric values anywhere - they may change.
	/// </remarks>
	public enum WebhookContentType
	{
		Form,
		Json,
	}

	/// <remarks>
	/// Saved into database as a bitarray - do not change the number values.
	/// Also remember to update the TypeScript file.
	/// </remarks>
	[Flags]
	public enum WebhookEvents
	{
		Default = 0,
		User = 1 << 0,
		EntryReport = 1 << 1,
	}

	public class Webhook : IEntryWithIntId
	{
		public virtual int Id { get; set; }

		public virtual string Url { get; init; } = string.Empty;

		public virtual WebhookContentType ContentType { get; init; } = WebhookContentType.Form;

		public virtual WebhookEvents WebhookEvents { get; init; } = WebhookEvents.Default;

		public Webhook() { }
	}
}
