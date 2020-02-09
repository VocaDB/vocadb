using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// Manages <see cref="IPRule"/>s
	/// </summary>
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	[RoutePrefix("api/ip-rules")]
	public class IPRuleApiController : ApiController {

		public IPRuleApiController(IUserPermissionContext userContext, IRepository repo, IPRuleManager ipRuleManager) {
			this.userContext = userContext;
			this.repo = repo;
			this.ipRuleManager = ipRuleManager;
		}

		private readonly IPRuleManager ipRuleManager;
		private readonly IRepository repo;
		private readonly IUserPermissionContext userContext;

		[Route("")]
		public bool PostNewIPRule(IPRule rule) {

			userContext.VerifyPermission(PermissionToken.ManageIPRules);

			if (string.IsNullOrEmpty(rule?.Address)) {
				throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
			}

			bool result = false;

			repo.HandleTransaction(ctx => {
				result = ipRuleManager.AddPermBannedIP(ctx, rule);
				ctx.AuditLogger.SysLog($"added {rule.Address} to banned IPs");
			});

			return result;

		}

	}
}