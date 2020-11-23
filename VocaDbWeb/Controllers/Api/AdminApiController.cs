using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Controllers.Api
{

	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	[RoutePrefix("api/admin")]
	public class AdminApiController : ApiController
	{

		private readonly IPRuleManager ipRuleManager;
		private readonly IUserPermissionContext userContext;

		public AdminApiController(IUserPermissionContext userContext, IPRuleManager ipRuleManager)
		{
			this.userContext = userContext;
			this.ipRuleManager = ipRuleManager;
		}

		[Route("tempBannedIPs")]
		public string[] GetTempBannedIPs()
		{

			userContext.VerifyPermission(PermissionToken.ManageIPRules);

			var hosts = ipRuleManager.TempBannedIPs.Hosts;
			return hosts.ToArray();

		}

	}

}