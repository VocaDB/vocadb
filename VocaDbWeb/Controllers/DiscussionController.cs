using Microsoft.AspNetCore.Mvc;

namespace VocaDb.Web.Controllers
{
	public class DiscussionController : ControllerBase
	{
		public ActionResult Index()
		{
			PageProperties.Title = "Discussions";

			return View("React/Index");
		}
	}
}