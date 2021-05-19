using System.Linq;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class WebhookQueryableExtensions
	{
		public static IQueryable<Webhook> WhereHasWebhookEvent(this IQueryable<Webhook> query, WebhookEvents webhookEvent)
			=> query.Where(w => (w.WebhookEvents & webhookEvent) != 0);
	}
}
