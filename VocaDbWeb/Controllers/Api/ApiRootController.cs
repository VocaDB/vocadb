#nullable disable

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Route("api")]
	[ApiController]
	public class ApiRootController : ApiController
	{
		[HttpGet("")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public RedirectResult Get() => Redirect("~/swagger");
	}
}