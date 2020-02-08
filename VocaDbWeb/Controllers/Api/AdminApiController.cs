using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Controllers.Api {

	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	[RoutePrefix("api/admin")]
	public class AdminApiController : ApiController {

		private readonly IRepository repo;
		private readonly IPRuleManager ipRuleManager;
		private readonly IUserPermissionContext userContext;

		public AdminApiController(IUserPermissionContext userContext, IRepository repo, IPRuleManager ipRuleManager) {
			this.userContext = userContext;
			this.repo = repo;
			this.ipRuleManager = ipRuleManager;
		}

		[Route("tempBannedIPs")]
		public string[] GetTempBannedIPs() {

			userContext.VerifyPermission(PermissionToken.ManageIPRules);

			var hosts = ipRuleManager.TempBannedIPs.Hosts;
			return hosts.ToArray();

		}

		[Route("permBannedIPs")]
		public bool PostNewPermBannedIp(IPRule rule) {

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