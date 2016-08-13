using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code.Security;

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
			return hosts;

		}

		[Route("permBannedIPs")]
		public bool PostNewPermBannedIp(IPRule rule) {

			userContext.VerifyPermission(PermissionToken.ManageIPRules);

			if (string.IsNullOrEmpty(rule?.Address)) {
				throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest);
			}

			if (ipRuleManager.PermBannedIPs.Contains(rule.Address)) {
				return false;
			}

			ipRuleManager.PermBannedIPs.Add(rule.Address);

			repo.HandleTransaction(ctx => {
				ctx.Save(rule);
			});

			return true;

		}

	}

}