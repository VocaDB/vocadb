using Microsoft.AspNetCore.Mvc;

namespace VocaDb.Web.Controllers
{
	public class ReactController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
