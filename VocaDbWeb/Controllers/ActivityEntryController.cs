using System;
using System.Web.Mvc;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
    public class ActivityEntryController : ControllerBase
    {

		private new const int entriesPerPage = 50;

		private readonly ActivityFeedService service;

		private ActivityFeedService Service {
			get { return service; }
		}

		public ActivityEntryController(ActivityFeedService service) {
			this.service = service;
		}

		public ActionResult FollowedArtistActivity() {

			var result = Service.GetFollowedArtistActivity(entriesPerPage, Request.IsSSL());
			return View(result.Items);

		}

        //
        // GET: /ActivityEntry/

        public ActionResult Index(DateTime? before)
        {

			ViewBag.Before = before;

			return View("Index");

        }

    }

	public class DetailedPageResult {

		public DateTime? LastEntryDate { get; set; }

		public string ViewHtml { get; set; }

	}

}
