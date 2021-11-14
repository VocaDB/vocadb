#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.Search;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
	public class HomeController : ControllerBase
	{
		private readonly BrandableStringsManager _brandableStringsManager;
		private readonly OtherService _otherService;
		private readonly SongQueries _songService;

		public HomeController(SongQueries songService, OtherService otherService, BrandableStringsManager brandableStringsManager)
		{
			_songService = songService;
			_otherService = otherService;
			_brandableStringsManager = brandableStringsManager;
		}

		public ActionResult Chat()
		{
			PageProperties.Title = "IRC chat";

			return View();
		}

		// Might still be used by some clients with opensearch
		[Obsolete("Moved to web api")]
		public ActionResult FindNames(string term)
		{
			var result = _otherService.FindNames(SearchTextQuery.Create(term), 10);

			return Json(result);
		}

		//
		// GET: /Home/

		public async Task<ActionResult> Index()
		{
			PageProperties.Description = _brandableStringsManager.Home.SiteDescription;
			PageProperties.AddMainScripts = false;
			PageProperties.CanonicalUrl = UrlMapper.HostAddress;

			var contract = await _otherService.GetFrontPageContent();

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

		public ActionResult PVContent(int songId = InvalidId)
		{
			if (songId == InvalidId)
				return NoId();

			var song = _songService.GetSongWithPVAndVote(songId, false);

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
