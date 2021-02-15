#nullable disable

using Microsoft.AspNetCore.Mvc;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	[Route("api")]
	[ApiController]
	public class ApiRootController : ApiController
	{
		[HttpGet("")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public RedirectResult Get() => Redirect("~/swagger");
	}
}