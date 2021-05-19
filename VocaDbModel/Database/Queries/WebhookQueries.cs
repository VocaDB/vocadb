using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Database.Queries
{
	public sealed class WebhookQueries
	{
		private readonly IUserPermissionContext _permissionContext;
		private readonly IRepository _repo;

		public WebhookQueries(IUserPermissionContext permissionContext, IRepository repo)
		{
			_permissionContext = permissionContext;
			_repo = repo;
		}

		public WebhookContract[] GetWebhooks()
		{
			_permissionContext.VerifyPermission(PermissionToken.ManageWebhooks);

			return _repo.HandleQuery(ctx =>
			{
				return ctx.Query<Webhook>()
					.ToArray()
					.Select(w => new WebhookContract(w))
					.ToArray();
			});
		}

		public void UpdateWebhooks(WebhookContract[] webhooks)
		{
			_permissionContext.VerifyPermission(PermissionToken.ManageWebhooks);

			_repo.HandleTransaction(ctx =>
			{
				ctx.AuditLogger.SysLog("updating webhooks");

				var existing = ctx.Query<Webhook>().ToList();
				var diff = CollectionHelper.Sync(
					existing,
					webhooks,
					equality: (left, right) => left.Url == right.Url && left.WebhookEvents == right.WebhookEvents,
					create: t => new Webhook(t.Url, t.WebhookEvents));

				ctx.OfType<Webhook>().Sync(diff);

				ctx.AuditLogger.AuditLog($"updated webhooks");
			});
		}
	}
}
