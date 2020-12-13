using System.Threading.Tasks;
using System.Web.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
	public class CommentController : ControllerBase
	{
		private readonly OtherService otherService;

		public CommentController(OtherService otherService)
		{
			this.otherService = otherService;
		}

		//
		// GET: /Comment/

		public async Task<ActionResult> Index()
		{
			var comments = await otherService.GetRecentComments();
			return View(comments);
		}
	}
}
