using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

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

			var comments = otherService.GetRecentComments(WebHelper.IsSSL(Request));
			return View(comments);

        }

    }
}
