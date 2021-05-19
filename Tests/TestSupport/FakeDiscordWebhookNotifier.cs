using System.Threading.Tasks;
using Discord;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.TestSupport
{
	public sealed class FakeDiscordWebhookNotifier : IDiscordWebhookNotifier
	{
		public Task SendMessageAsync(WebhookEvents webhookEvent, IUserWithEmail? user, string? title = null, string? url = null, string? description = null, Color? color = null)
		{
			return Task.CompletedTask;
		}
	}
}
