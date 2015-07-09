using System.Linq;
using System.Net;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Rankings;
using VocaDb.Model.Utils;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
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

		public ActionResult CreateFromWVR() {

			var model = new CreateFromWVR();
			return View(model);

		}

		[HttpPost]
		public ActionResult CreateFromWVR(CreateFromWVR model, bool commit) {

			if (commit) {
				var listId = rankingService.CreateSongListFromWVR(model.Url, !model.OnlyRanked);
				return RedirectToAction("Details", "SongList", new { id = listId });
			}

			ImportedSongListContract parseResult;
			try {
				parseResult = queries.Import(model.Url, !model.OnlyRanked);
			} catch (InvalidFeedException) {
				ModelState.AddModelError("Url", Resources.Views.SongList.ImportNicoMylistStrings.InvalidUrlError);
				return View(model);
			}
			model.ListResult = parseResult;

			if (parseResult.Songs.Items.Any(s => s.MatchedSong == null))
				ModelState.AddModelError("ListResult", Resources.Views.SongList.ImportNicoMylistStrings.SongsMissingError);

			return View(model);

		}

		public ActionResult Delete(int id) {

			Service.DeleteSongList(id);

			return RedirectToAction("Profile", "User", new { id = PermissionContext.LoggedUser.Name });

		}

		public ActionResult Details(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = queries.GetSongList(id);

			PageProperties.CanonicalUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Details", new { id })).ToString();
			PageProperties.Description = contract.Description;

			if (contract.FeaturedCategory == SongListFeaturedCategory.Nothing) {
				PageProperties.PageTitle = string.Format("{0} - {1}", ViewRes.SongList.DetailsStrings.SongList, contract.Name);
				PageProperties.Title = contract.Name;
				PageProperties.Subtitle = ViewRes.SongList.DetailsStrings.SongList;
			} else {

				var categoryName = Translate.SongListFeaturedCategoryNames[contract.FeaturedCategory];

				PageProperties.PageTitle = string.Format("{0} - {1}", categoryName, contract.Name);
				PageProperties.Title = contract.Name;
				PageProperties.Subtitle = categoryName;

			}

			var viewModel = new SongListDetailsViewModel(contract);

			viewModel.SmallThumbUrl = Url.EntryImageOld(contract.Thumb, ImageSize.SmallThumb);
			var thumbUrl = viewModel.ThumbUrl = Url.EntryImageOld(contract.Thumb, ImageSize.Original) ?? Url.EntryImageOld(contract.Thumb, ImageSize.Thumb);
			if (!string.IsNullOrEmpty(thumbUrl)) {
				PageProperties.OpenGraph.Image = thumbUrl;
			}

			PageProperties.OpenGraph.ShowTwitterCard = true;

			return View(viewModel);

		}

        //
        // GET: /SongList/Edit/
        [Authorize]
        public ActionResult Edit(int? id)
        {

			var contract = id != null ? queries.GetSongList(id.Value) : new SongListContract();
			var model = new SongListEditViewModel(contract, PermissionContext);

            return View(model);

        }

		[HttpPost]
        [Authorize]
		public ActionResult Edit(SongListEditViewModel model)
        {

			if (model == null) {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "View model was null - probably JavaScript is disabled");				
			}

			var coverPicUpload = Request.Files["thumbPicUpload"];
			UploadedFileContract uploadedPicture = null;
			if (coverPicUpload != null && coverPicUpload.ContentLength > 0) {

				CheckUploadedPicture(coverPicUpload, "thumbPicUpload");
				uploadedPicture = new UploadedFileContract {Mime = coverPicUpload.ContentType, Stream = coverPicUpload.InputStream};

			}

			if (!ModelState.IsValid) {
				return View(new SongListEditViewModel(model.ToContract(), PermissionContext));
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
