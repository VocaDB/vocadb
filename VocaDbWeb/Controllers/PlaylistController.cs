using Microsoft.AspNetCore.Mvc;

namespace VocaDb.Web.Controllers;

public class PlaylistController : Controller
{
	public IActionResult Index()
	{
		return View("React/Index");
	}
}
