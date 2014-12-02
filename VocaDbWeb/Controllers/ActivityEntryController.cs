using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Web.Controllers
{
    public class ActivityEntryController : Controller
    {

		private const int entriesPerPage = 50;

		private readonly ActivityFeedService service;

		private ActivityFeedService Service {
			get { return service; }
		}

		public ActivityEntryController(ActivityFeedService service) {
			this.service = service;
		}

		public ActionResult Entries(int page = 1) {

			var pageIndex = page - 1;
			var result = Service.GetActivityEntries(PagingProperties.CreateFromPage(pageIndex, entriesPerPage, false));

			return PartialView("ActivityEntryContracts", result.Items);

		}

		public ActionResult EntriesPaged(int? page) {

			return RedirectToActionPermanent("Index", new { page = page ?? 1 });

		}

		public ActionResult FollowedArtistActivity() {

			var result = Service.GetFollowedArtistActivity(entriesPerPage);
			return View(result.Items);

		}

        //
        // GET: /ActivityEntry/

        public ActionResult Index(int page = 1)
        {

			if (Request.IsAjaxRequest())
				return Entries(page);
			else {
				ViewBag.Page = page;
				return View();
			}

        }

    }
}
