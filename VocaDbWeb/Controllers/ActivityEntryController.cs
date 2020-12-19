#nullable disable

using System;
using System.Web.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
	public class ActivityEntryController : ControllerBase
	{
		private new const int EntriesPerPage = 50;

		private readonly ActivityFeedService _service;

		private ActivityFeedService Service => _service;

		public ActivityEntryController(ActivityFeedService service)
		{
			_service = service;
		}

		public ActionResult FollowedArtistActivity()
		{
			var result = Service.GetFollowedArtistActivity(EntriesPerPage);
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

	public class DetailedPageResult
	{
		public DateTime? LastEntryDate { get; set; }

		public string ViewHtml { get; set; }
	}
}
