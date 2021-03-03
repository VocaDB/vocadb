#nullable disable

using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("api/admin")]
	[ApiController]
	public class AdminApiController : ApiController
	{
		private readonly IPRuleManager _ipRuleManager;
		private readonly IUserPermissionContext _userContext;

		public AdminApiController(IUserPermissionContext userContext, IPRuleManager ipRuleManager)
		{
			_userContext = userContext;
			_ipRuleManager = ipRuleManager;
		}

		[HttpGet("tempBannedIPs")]
		public string[] GetTempBannedIPs()
		{
			_userContext.VerifyPermission(PermissionToken.ManageIPRules);

			var hosts = _ipRuleManager.TempBannedIPs.Hosts;
			return hosts.ToArray();
		}
	}
}