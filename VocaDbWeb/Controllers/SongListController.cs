using System;
using System.Web.Mvc;
using MvcPaging;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Rankings;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.SongLists;

namespace VocaDb.Web.Controllers
{
    public class SongListController : ControllerBase
    {

		public const int SongsPerPage = 50;
		private readonly SongListQueries queries;
		private readonly RankingService rankingService;
	    private readonly SongService service;

		private SongService Service {
			get { return service; }
		}

		public SongListController(SongService service, RankingService rankingService, SongListQueries queries) {
			this.service = service;
			this.rankingService = rankingService;
			this.queries = queries;
		}

		[Obsolete("Handled in view model now")]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AddSong(int songId) {

			var songContract = service.GetSongWithAdditionalNames(songId);
			var link = new SongInListEditContract(songContract);

			return Json(link);

		}

		public ActionResult CreateFromWVR() {

			var model = new CreateFromWVR();
			return View(model);

		}

		[HttpPost]
		public ActionResult CreateFromWVR(CreateFromWVR model, bool commit) {

			if (commit) {
				var listId = rankingService.CreateSongListFromWVR(model.Url, model.ParseAll);
				return RedirectToAction("Details", "SongList", new { id = listId });
			}

			WVRListResult parseResult;
			try {
				parseResult = rankingService.ParseWVRList(model.Url, model.ParseAll);
			} catch (InvalidFeedException) {
				ModelState.AddModelError("Url", "Check that the URL is valid and points to a NicoNico MyList or RSS feed.");
				return View(model);
			}
			model.ListResult = parseResult;

			if (parseResult.IsIncomplete)
				ModelState.AddModelError("ListResult", "Some of the songs are missing from VocaDB. You need to add all songs to the database before creating the list.");

			return View(model);

		}

		public ActionResult Data(int id = 0) {

			var list = (id != 0 ? Service.GetSongListForEdit(id) : new SongListForEditContract());
			return LowercaseJson(list);

		}

		public ActionResult Delete(int id) {

			Service.DeleteSongList(id);

			return RedirectToAction("Profile", "User", new { id = LoginManager.LoggedUser.Name });

		}

		public ActionResult Details(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = queries.GetSongList(id);

			return View(contract);

		}

        //
        // GET: /SongList/Edit/
        [Authorize]
        public ActionResult Edit(int? id)
        {

			var contract = id != null ? Service.GetSongListForEdit(id.Value, false) : new SongListForEditContract();
			var model = new SongListEdit(contract);

            return View(model);

        }

		[HttpPost]
        [Authorize]
		public ActionResult Edit(SongListEdit model)
        {

			var coverPicUpload = Request.Files["thumbPicUpload"];
			UploadedFileContract uploadedPicture = null;
			if (coverPicUpload != null && coverPicUpload.ContentLength > 0) {

				CheckUploadedPicture(coverPicUpload, "thumbPicUpload");
				uploadedPicture = new UploadedFileContract {Mime = coverPicUpload.ContentType, Stream = coverPicUpload.InputStream};

			}

			if (!ModelState.IsValid) {
				return View(model);
			}

			var listId = queries.UpdateSongList(model.ToContract(), uploadedPicture);

			return RedirectToAction("Details", new { id = listId });

		}

		public ActionResult Featured() {

			var lists = Service.GetSongListsByCategory();

			return View(lists);

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = queries.GetSongListWithArchivedVersions(id);

			if (contract == null)
				return HttpNotFound();

			return View(contract);

		}
    }
}
