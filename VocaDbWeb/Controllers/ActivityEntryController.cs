using System;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers
{
    public class ActivityEntryController : ControllerBase
    {

		private new const int entriesPerPage = 50;

		private readonly EntryQueries entryQueries;
		private readonly ActivityFeedService service;

		private ActivityFeedService Service {
			get { return service; }
		}

		public ActivityEntryController(ActivityFeedService service, EntryQueries entryQueries) {
			this.service = service;
			this.entryQueries = entryQueries;
		}

		public ActionResult Detailed(DateTime? since) {
			
			var entries = entryQueries.GetRecentVersions(100, since);
			var lastEntryDate = (entries.Any() ? (DateTime?)entries.Last().CreateDate.ToUniversalTime() : null);
			ViewBag.LastEntryDate = lastEntryDate;
			return View("Detailed", entries);

		}

		public ActionResult DetailedPage(DateTime since) {
			
			var entries = entryQueries.GetRecentVersions(100, since);
			var lastEntryDate = (entries.Any() ? (DateTime?)entries.Last().CreateDate : null);
			var view = RenderPartialViewToString("_DetailedPage", entries);
			return LowercaseJson(new DetailedPageResult { ViewHtml = view, LastEntryDate = lastEntryDate });

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

	public class DetailedPageResult {

		public DateTime? LastEntryDate { get; set; }

		public string ViewHtml { get; set; }

	}

}
