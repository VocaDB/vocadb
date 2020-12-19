#nullable disable

using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.SongLists;

namespace VocaDb.Web.Controllers
{
	public class SongListController : ControllerBase
	{
		public const int SongsPerPage = 50;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly SongListQueries _queries;

		public SongListController(SongListQueries queries, IEntryLinkFactory entryLinkFactory)
		{
			_queries = queries;
			_entryLinkFactory = entryLinkFactory;
		}

		public ActionResult Details(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = _queries.GetDetails(id);

			PageProperties.CanonicalUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Details", new { id })).ToString();
			PageProperties.Description = contract.Description;

			if (contract.FeaturedCategory == SongListFeaturedCategory.Nothing)
			{
				PageProperties.PageTitle = $"{ViewRes.SongList.DetailsStrings.SongList} - {contract.Name}";
				PageProperties.Title = contract.Name;
				PageProperties.Subtitle = ViewRes.SongList.DetailsStrings.SongList;
			}
			else
			{
				var categoryName = Translate.SongListFeaturedCategoryNames[contract.FeaturedCategory];

				PageProperties.PageTitle = $"{categoryName} - {contract.Name}";
				PageProperties.Title = contract.Name;
				PageProperties.Subtitle = categoryName;
			}

			var viewModel = new SongListDetailsViewModel(contract, PermissionContext);

			viewModel.SmallThumbUrl = Url.ImageThumb(contract.MainPicture, ImageSize.SmallThumb);
			var thumbUrl = viewModel.ThumbUrl = Url.ImageThumb(contract.MainPicture, ImageSize.Original) ?? Url.ImageThumb(contract.MainPicture, ImageSize.Thumb);
			if (!string.IsNullOrEmpty(thumbUrl))
			{
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
			var contract = id != null ? _queries.GetSongList(id.Value) : new SongListContract();
			var model = new SongListEditViewModel(contract, PermissionContext);

			return View(model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult Edit(SongListEditViewModel model)
		{
			if (model == null)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "View model was null - probably JavaScript is disabled");
			}

			var coverPicUpload = Request.Files["thumbPicUpload"];
			UploadedFileContract uploadedPicture = null;
			if (coverPicUpload != null && coverPicUpload.ContentLength > 0)
			{
				CheckUploadedPicture(coverPicUpload, "thumbPicUpload");
				uploadedPicture = new UploadedFileContract { Mime = coverPicUpload.ContentType, Stream = coverPicUpload.InputStream };
			}

			if (!ModelState.IsValid)
			{
				return View(new SongListEditViewModel(model.ToContract(), PermissionContext));
			}

			var listId = _queries.UpdateSongList(model.ToContract(), uploadedPicture);

			return RedirectToAction("Details", new { id = listId });
		}

		public ActionResult Export(int id)
		{
			var songList = _queries.GetSongList(id);
			var formatString = "%notes%;%publishdate%;%title%;%url%;%pv.original.niconicodouga%;%pv.original.!niconicodouga%;%pv.reprint%";
			var tagString = _queries.GetTagString(id, formatString);

			var enc = new UTF8Encoding(true);
			var data = enc.GetPreamble().Concat(enc.GetBytes(tagString)).ToArray();

			return File(data, "text/csv", songList.Name + ".csv");
		}

		public ActionResult Featured(FeaturedViewModel viewModel)
		{
			return View(viewModel);
		}

		[Authorize]
		public ActionResult Import()
		{
			return View();
		}

		public ActionResult Versions(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = _queries.GetSongListWithArchivedVersions(id);

			if (contract == null)
				return HttpNotFound();

			return View(contract);
		}
	}
}
