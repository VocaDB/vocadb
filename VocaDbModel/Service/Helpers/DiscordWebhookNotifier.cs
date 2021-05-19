using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using Microsoft.Extensions.Options;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Helpers
{
	public sealed record DiscordWebhookSettings
	{
		public const string DiscordWebhook = nameof(DiscordWebhook);

		public string AvatarUrl { get; init; } = string.Empty;
	}

	public interface IDiscordWebhookNotifier
	{
		Task SendMessageAsync(WebhookEvents webhookEvent, IUserWithEmail? user, string? title = null, string? url = null, string? description = null, Color? color = null);
	}

	public sealed class DiscordWebhookNotifier : IDiscordWebhookNotifier
	{
		private static readonly ILogger s_log = LogManager.GetCurrentClassLogger();

		private readonly DiscordWebhookSettings _discordWebhookSettings;
		private readonly BrandableStringsManager _brandableStrings;
		private readonly IRepository _repository;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly IUserIconFactory _userIconFactory;

		public DiscordWebhookNotifier(
			IOptions<DiscordWebhookSettings> discordWebhookSettings,
			BrandableStringsManager brandableStrings,
			IRepository repository,
			IEntryLinkFactory entryLinkFactory,
			IUserIconFactory userIconFactory)
		{
			_discordWebhookSettings = discordWebhookSettings.Value;
			_brandableStrings = brandableStrings;
			_repository = repository;
			_entryLinkFactory = entryLinkFactory;
			_userIconFactory = userIconFactory;
		}

		public async Task SendMessageAsync(WebhookEvents webhookEvent, IUserWithEmail? user, string? title = null, string? url = null, string? description = null, Color? color = null)
		{
			EmbedBuilder CreateEmbedBuilder(IUserWithEmail? user)
			{
				var builder = new EmbedBuilder
				{
					Color = color,
				};

				if (user is not null)
				{
					var profileUrl = _entryLinkFactory.GetFullEntryUrl(EntryType.User, user.Id);

					builder
						.WithAuthor(name: user.Name, iconUrl: _userIconFactory.GetIcons(user, ImageSizes.Thumb).UrlThumb, url: profileUrl)
						.WithUrl(profileUrl);
				}

				return builder;
			}

			var embeds = new[] { CreateEmbedBuilder(user).WithTitle(title).WithUrl(url).WithDescription(description).Build() };

			await _repository.HandleTransactionAsync(async ctx =>
			{
				var webhooks = await ctx.Query<Webhook>().WhereHasWebhookEvent(webhookEvent).ToListAsync();

				var tasks = webhooks.Select(async w =>
				{
					try
					{
						using var client = new DiscordWebhookClient(w.Url);
						await client.SendMessageAsync(embeds: embeds, username: _brandableStrings.SiteName, avatarUrl: _discordWebhookSettings.AvatarUrl);
					}
					catch (Exception x)
					{
						s_log.Error(x, "Unable to send message");
					}
				}).ToList();

				await Task.WhenAll(tasks);
			});
		}
	}
}
