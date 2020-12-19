#nullable disable

using System.Threading.Tasks;
using System.Web.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
	public class CommentController : ControllerBase
	{
		private readonly OtherService _otherService;

		public CommentController(OtherService otherService)
		{
			_otherService = otherService;
		}

		//
		// GET: /Comment/

		public async Task<ActionResult> Index()
		{
			var comments = await _otherService.GetRecentComments();
			return View(comments);
		}
	}
}
