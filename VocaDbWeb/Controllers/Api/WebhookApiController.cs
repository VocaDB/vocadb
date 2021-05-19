using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Authorize]
	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("api/webhooks")]
	[ApiController]
	public class WebhookApiController : ApiController
	{
		private readonly IUserPermissionContext _userContext;
		private readonly WebhookQueries _queries;

		public WebhookApiController(IUserPermissionContext userContext, WebhookQueries queries)
		{
			_userContext = userContext;
			_queries = queries;
		}

		[HttpGet("")]
		public WebhookContract[] GetWebhooks()
		{
			_userContext.VerifyPermission(PermissionToken.ManageWebhooks);

			return _queries.GetWebhooks();
		}

		[HttpPut("")]
		public IActionResult PutWebhooks(IEnumerable<WebhookContract> webhooks)
		{
			_userContext.VerifyPermission(PermissionToken.ManageWebhooks);

			_queries.UpdateWebhooks(webhooks.ToArray());

			return NoContent();
		}
	}
}
