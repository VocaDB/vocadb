using Microsoft.AspNetCore.Mvc;

namespace VocaDb.Web.Controllers;

public class PlaylistController : ControllerBase
{
	public IActionResult Index()
	{
		return ReactIndex.File(PageProperties);
	}
}
