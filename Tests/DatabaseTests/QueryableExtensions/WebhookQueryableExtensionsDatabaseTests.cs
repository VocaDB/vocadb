using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.QueryableExtensions
{
	/// <summary>
	/// Database tests for <see cref="WebhookQueryableExtensions"/>.
	/// </summary>
	[TestClass]
	public class WebhookQueryableExtensionsDatabaseTests
	{
		private readonly DatabaseTestContext<IDatabaseContext> _context = new();

		private Webhook[] WhereHasWebhookEvent(WebhookEvents webhookEvent)
		{
			return _context.RunTest(ctx =>
			{
				return ctx.Query<Webhook>().WhereHasWebhookEvent(webhookEvent).ToArray();
			});
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void WhereHasWebhookEvent()
		{
			var webhooks = WhereHasWebhookEvent(WebhookEvents.User);

			webhooks.Length.Should().Be(2, "Number of webhooks returned");
			webhooks.Any(w => w.Url == "https://discord.com/api/webhooks/39").Should().BeTrue("Found 'https://discord.com/api/webhooks/39' webhook");
			webhooks.Any(w => w.Url == "https://discord.com/api/webhooks/3939").Should().BeTrue("Found 'https://discord.com/api/webhooks/3939' webhook");
		}


		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void WhereHasName_Empty()
		{
			var webhooks = WhereHasWebhookEvent(WebhookEvents.Default);

			webhooks.Length.Should().Be(0, "Number of webhooks returned");
		}
	}
}
