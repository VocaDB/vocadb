using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.TestSupport
{
	public sealed class FakeDiscordWebhookNotifier : IDiscordWebhookNotifier
	{
		public Task SendMessageAsync(WebhookEvents webhookEvent, string? text = null, IEnumerable<Embed>? embeds = null)
		{
			return Task.CompletedTask;
		}
	}
}
