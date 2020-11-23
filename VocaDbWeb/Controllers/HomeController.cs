using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.Search;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
	[SessionState(SessionStateBehavior.ReadOnly)]
	public class HomeController : ControllerBase
	{
		private readonly BrandableStringsManager brandableStringsManager;
		private readonly OtherService otherService;
		private readonly SongQueries songService;

		public HomeController(SongQueries songService, OtherService otherService, BrandableStringsManager brandableStringsManager)
		{
			this.songService = songService;
			this.otherService = otherService;
			this.brandableStringsManager = brandableStringsManager;
		}

		public ActionResult Chat()
		{
			return View();
		}

		// Might still be used by some clients with opensearch
		[Obsolete("Moved to web api")]
		public ActionResult FindNames(string term)
		{
			var result = otherService.FindNames(SearchTextQuery.Create(term), 10);

			return Json(result);
		}

		//
		// GET: /Home/

		public async Task<ActionResult> Index()
		{
			PageProperties.Description = brandableStringsManager.Home.SiteDescription;
			PageProperties.AddMainScripts = false;

			var contract = await otherService.GetFrontPageContent();

			return View(contract);
		}

		[HttpPost]
		public ActionResult GlobalSearch(GlobalSearchBoxModel model)
		{
			switch (model.ObjectType)
			{
				case EntryType.Undefined:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm });

				case EntryType.Album:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				case EntryType.Artist:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				case EntryType.ReleaseEvent:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				case EntryType.Song:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				case EntryType.SongList:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				default:
					var controller = model.ObjectType.ToString();
					return RedirectToAction("Index", controller, new { filter = model.GlobalSearchTerm });
			}
		}

		public ActionResult PVContent(int songId = invalidId)
		{
			if (songId == invalidId)
				return NoId();

			var song = songService.GetSongWithPVAndVote(songId, false);

			return PartialView("PVs/_PVContent", song);
		}

		public ActionResult Search(string filter)
		{
			return RedirectToAction("Index", "Search", new { filter });
		}

		public ActionResult Wiki()
		{
			return View();
		}
	}
}
