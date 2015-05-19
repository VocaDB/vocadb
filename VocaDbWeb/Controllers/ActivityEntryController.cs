using System;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.Service;
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

		public ActionResult DetailedPage(DateTime before) {
			
			var entries = entryQueries.GetRecentVersions(100, before);
			var lastEntryDate = (entries.Any() ? (DateTime?)entries.Last().CreateDate.ToUniversalTime() : null);
			var view = RenderPartialViewToString("_DetailedPage", entries);
			return LowercaseJson(new DetailedPageResult { ViewHtml = view, LastEntryDate = lastEntryDate });

		}

		public ActionResult FollowedArtistActivity() {

			var result = Service.GetFollowedArtistActivity(entriesPerPage);
			return View(result.Items);

		}

        //
        // GET: /ActivityEntry/

        public ActionResult Index(DateTime? before)
        {

			var entries = entryQueries.GetRecentVersions(100, before);
			var lastEntryDate = (entries.Any() ? (DateTime?)entries.Last().CreateDate.ToUniversalTime() : null);
			ViewBag.LastEntryDate = lastEntryDate;

			return View("Index", entries);

        }

    }

	public class DetailedPageResult {

		public DateTime? LastEntryDate { get; set; }

		public string ViewHtml { get; set; }

	}

}
