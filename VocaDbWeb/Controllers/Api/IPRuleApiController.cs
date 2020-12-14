#nullable disable

using NHibernate.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// Manages <see cref="IPRule"/>s
	/// </summary>
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	[RoutePrefix("api/ip-rules")]
	public class IPRuleApiController : ApiController
	{
		public IPRuleApiController(IUserPermissionContext userContext, IRepository repo, IPRuleManager ipRuleManager)
		{
			this.userContext = userContext;
			this.repo = repo;
			this.ipRuleManager = ipRuleManager;
		}

		private readonly IPRuleManager ipRuleManager;
		private readonly IRepository repo;
		private readonly IUserPermissionContext userContext;

		[Route("{id:int}")]
		public async Task DeleteIPRule(int ruleID)
		{
			userContext.VerifyPermission(PermissionToken.ManageIPRules);

			await repo.HandleTransactionAsync(async ctx =>
			{
				var rule = await ctx.LoadAsync<IPRule>(ruleID);
				await ctx.DeleteAsync(rule);
				ipRuleManager.RemovePermBannedIP(rule.Address);
				ctx.AuditLogger.SysLog($"removed {rule.Address} from banned IPs");
			});
		}

		[Route("")]
		public async Task<IEnumerable<IPRule>> GetIPRules()
		{
			userContext.VerifyPermission(PermissionToken.ManageIPRules);

			return await repo.HandleQueryAsync(async ctx =>
			{
				return await ctx.Query<IPRule>().ToListAsync();
			});
		}

		[Route("")]
		public bool PostNewIPRule(IPRule rule)
		{
			userContext.VerifyPermission(PermissionToken.ManageIPRules);

			if (string.IsNullOrEmpty(rule?.Address))
			{
				throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
			}

			bool result = false;

			repo.HandleTransaction(ctx =>
			{
				result = ipRuleManager.AddPermBannedIP(ctx, rule);
				ctx.AuditLogger.SysLog($"added {rule.Address} to banned IPs");
			});

			return result;
		}
	}
}