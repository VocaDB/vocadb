using System.Threading.Tasks;
using Discord.Webhook;
using Microsoft.Extensions.Options;
using VocaDb.Model.Service.BrandableStrings;

namespace VocaDb.Model.Service.Helpers
{
	public sealed record DiscordWebhookSettings
	{
		public const string DiscordWebhook = nameof(DiscordWebhook);

		/// <summary>
		/// Sends a notification to Discord whenever a new user is registered.
		/// </summary>
		public string WebhookUrl { get; init; } = string.Empty;

		public string AvatarUrl { get; init; } = string.Empty;
	}

	public interface IDiscordWebhookNotifier
	{
		Task SendMessageAsync(string text);
	}

	public sealed class DiscordWebhookNotifier : IDiscordWebhookNotifier
	{
		private readonly DiscordWebhookSettings _discordWebhookSettings;
		private readonly BrandableStringsManager _brandableStrings;

		public DiscordWebhookNotifier(IOptions<DiscordWebhookSettings> discordWebhookSettings, BrandableStringsManager brandableStrings)
		{
			_discordWebhookSettings = discordWebhookSettings.Value;
			_brandableStrings = brandableStrings;
		}

		public async Task SendMessageAsync(string text)
		{
			var webhookUrl = _discordWebhookSettings.WebhookUrl;
			if (string.IsNullOrEmpty(webhookUrl))
				return;

			using var client = new DiscordWebhookClient(webhookUrl);
			await client.SendMessageAsync(text: text, username: _brandableStrings.SiteName, avatarUrl: _discordWebhookSettings.AvatarUrl);
		}
	}
}
