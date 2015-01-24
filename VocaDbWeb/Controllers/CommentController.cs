using System.Web.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers {

    public class CommentController : ControllerBase {

		private readonly OtherService otherService;

		public CommentController(OtherService otherService) {
			this.otherService = otherService;
		}

        //
        // GET: /Comment/

        public ActionResult Index()
        {

			var comments = otherService.GetRecentComments();
			return View(comments);

        }

    }
}
