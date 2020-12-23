#nullable disable

using Microsoft.AspNetCore.Mvc;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	[Route("api/mikus")]
	[ApiController]
	public class MikuApiController : ApiController
	{
		[HttpGet("")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public RedirectResult GetMikus() => Redirect("http://www.pixiv.net/search.php?s_mode=s_tag&word=%E3%81%BE%E3%81%BE%E3%81%BE%E5%BC%8F%E3%81%82%E3%81%B4%E3%83%9F%E3%82%AF");
	}
}