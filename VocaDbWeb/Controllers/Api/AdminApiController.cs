#nullable disable

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
		private readonly IPRuleManager _ipRuleManager;
		private readonly IUserPermissionContext _userContext;

		public AdminApiController(IUserPermissionContext userContext, IPRuleManager ipRuleManager)
		{
			_userContext = userContext;
			_ipRuleManager = ipRuleManager;
		}

		[Route("tempBannedIPs")]
		public string[] GetTempBannedIPs()
		{
			_userContext.VerifyPermission(PermissionToken.ManageIPRules);

			var hosts = _ipRuleManager.TempBannedIPs.Hosts;
			return hosts.ToArray();
		}
	}
}