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
			_userContext = userContext;
			_repo = repo;
			_ipRuleManager = ipRuleManager;
		}

		private readonly IPRuleManager _ipRuleManager;
		private readonly IRepository _repo;
		private readonly IUserPermissionContext _userContext;

		[Route("{id:int}")]
		public async Task DeleteIPRule(int ruleID)
		{
			_userContext.VerifyPermission(PermissionToken.ManageIPRules);

			await _repo.HandleTransactionAsync(async ctx =>
			{
				var rule = await ctx.LoadAsync<IPRule>(ruleID);
				await ctx.DeleteAsync(rule);
				_ipRuleManager.RemovePermBannedIP(rule.Address);
				ctx.AuditLogger.SysLog($"removed {rule.Address} from banned IPs");
			});
		}

		[Route("")]
		public async Task<IEnumerable<IPRule>> GetIPRules()
		{
			_userContext.VerifyPermission(PermissionToken.ManageIPRules);

			return await _repo.HandleQueryAsync(async ctx =>
			{
				return await ctx.Query<IPRule>().ToListAsync();
			});
		}

		[Route("")]
		public bool PostNewIPRule(IPRule rule)
		{
			_userContext.VerifyPermission(PermissionToken.ManageIPRules);

			if (string.IsNullOrEmpty(rule?.Address))
			{
				throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
			}

			bool result = false;

			_repo.HandleTransaction(ctx =>
			{
				result = _ipRuleManager.AddPermBannedIP(ctx, rule);
				ctx.AuditLogger.SysLog($"added {rule.Address} to banned IPs");
			});

			return result;
		}
	}
}