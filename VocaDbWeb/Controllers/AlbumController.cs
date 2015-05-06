using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using NLog;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Resources;
using VocaDb.Model.Service;
using VocaDb.Model.Service.TagFormatting;
using VocaDb.Model.Utils.Search;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using System.Drawing;
using System.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Web.Models.Album;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;

namespace VocaDb.Web.Controllers
{
    public class AlbumController : ControllerBase
    {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly AlbumDescriptionGenerator albumDescriptionGenerator;
		private readonly MarkdownParser markdownParser;
	    private readonly AlbumQueries queries;
	    private readonly UserQueries userQueries;

		private AlbumService Service { get; set; }

		public AlbumController(AlbumService service, AlbumQueries queries, UserQueries userQueries, AlbumDescriptionGenerator albumDescriptionGenerator,
			MarkdownParser markdownParser) {

			Service = service;
			this.queries = queries;
			this.userQueries = userQueries;
			this.albumDescriptionGenerator = albumDescriptionGenerator;
			this.markdownParser = markdownParser;

		}

		public ActionResult ArchivedVersionCoverPicture(int id) {

			var contract = Service.GetArchivedAlbumPicture(id);

			return Picture(contract);

		}

		public ActionResult ArchivedVersionXml(int id) {

			var doc = Service.GetVersionXml(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);

		}

		[HttpPost]
		public void CreateReport(int albumId, AlbumReportType reportType, string notes, int? versionNumber) {

			queries.CreateReport(albumId, reportType, CfHelper.GetRealIp(Request), notes ?? string.Empty, versionNumber);

		}

        //
        // GET: /Album/

		public ActionResult Index(IndexRouteParams routeParams) {

			return RedirectToAction("Index", "Search", new SearchRouteParams {
				searchType = EntryType.Album, filter = routeParams.filter, sort = routeParams.sort,
				discType = routeParams.discType
			});

        }

		public ActionResult FindDuplicate(string term1, string term2, string term3) {

			var result = Service.FindDuplicates(new[] { term1, term2, term3 });
			var contracts = result.Select(f => new DuplicateEntryResultContract<AlbumMatchProperty>(f, AlbumMatchProperty.Title)).ToArray();

			return LowercaseJson(contracts);

		}

		public ActionResult PopupContent(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			var album = Service.GetAlbum(id);
			return PartialView("AlbumPopupContent", album);

		}

		public ActionResult PopupWithCoverContent(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			var album = Service.GetAlbum(id);
			return PartialView("AlbumWithCoverPopupContent", album);

		}

        //
        // GET: /Album/Details/5

        public ActionResult Details(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			WebHelper.VerifyUserAgent(Request);

			var model = Service.GetAlbumDetails(id, WebHelper.IsValidHit(Request) ? WebHelper.GetRealHost(Request) : string.Empty);

			var prop = PageProperties;
			prop.Title = model.Name;
			prop.CanonicalUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Details", new { id })).ToString();
			prop.GlobalSearchType = EntryType.Album;
			prop.OpenGraph.Image = VocaUriBuilder.CreateAbsolute(Url.Action("CoverPicture", new { id })).ToString();
			prop.OpenGraph.Type = OpenGraphTypes.Album;

			string titleAndArtist;
			if (!string.IsNullOrEmpty(model.ArtistString)) {
				titleAndArtist = string.Format("{0} - {1}", model.Name, model.ArtistString);
			} else {
				titleAndArtist = model.Name;
			}

			PageProperties.OpenGraph.Title =  string.Format("{0} ({1})", titleAndArtist, Translate.DiscTypeName(model.DiscType));

			PageProperties.PageTitle = titleAndArtist;
			PageProperties.Subtitle = string.Format("{0} ({1})", model.ArtistString, Translate.DiscTypeName(model.DiscType));

			prop.Description = !model.Description.IsEmpty ? 
				markdownParser.GetPlainText(model.Description.EnglishOrOriginal) :
				albumDescriptionGenerator.GenerateDescription(model, d => Translate.DiscTypeNames.GetName(d, CultureInfo.InvariantCulture));

            return View(new AlbumDetails(model, PermissionContext));

        }

		public ActionResult DownloadTags(int id = invalidId, string formatString = "", bool setFormatString = false, bool includeHeader = false) {

			if (id == invalidId)
				return NoId();

			if (setFormatString) {
				userQueries.SetAlbumFormatString(formatString);
			} else if (string.IsNullOrEmpty(formatString) && PermissionContext.IsLoggedIn) {
				formatString = PermissionContext.LoggedUser.AlbumFormatString;
			}

			if (string.IsNullOrEmpty(formatString))
				formatString = TagFormatter.TagFormatStrings[0];

			var album = Service.GetAlbum(id);
			var tagString = Service.GetAlbumTagString(id, formatString, includeHeader);

			var enc = new UTF8Encoding(true);
			var data = enc.GetPreamble().Concat(enc.GetBytes(tagString)).ToArray();

			return File(data, "text/csv", album.Name + ".csv");

		}

		//[OutputCache(Duration = pictureCacheDurationSec, Location = OutputCacheLocation.Any, VaryByParam = "id,v")]
		public ActionResult CoverPicture(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			var album = Service.GetCoverPicture(id, Size.Empty);

			return Picture(album);

		}

		public ActionResult CoverPictureThumb(int id = invalidId) {

			if (id == invalidId) 
				return HttpNotFound();

			var data = queries.GetCoverPictureThumb(id);
			return Picture(data);

		}

		[Authorize]
		public ActionResult Create() {

			return View(new Create());

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) 
				&& string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", ViewRes.Album.CreateStrings.NeedArtist);

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			var album = queries.Create(contract);
			return RedirectToAction("Edit", new { id = album.Id });

		}

        //
        // GET: /Album/Edit/5
        [Authorize]
        public ActionResult Edit(int id) {

			CheckConcurrentEdit(EntryType.Album, id);

        	var album = Service.GetAlbum(id);
			return View(new AlbumEditViewModel(album, PermissionContext));

        }

        //
        // POST: /Album/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(AlbumEditViewModel viewModel)
        {

			if (viewModel == null) {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Viewmodel was null - probably JavaScript is disabled");				
			}

			try {
				viewModel.CheckModel();
			} catch (InvalidFormException x) {
				AddFormSubmissionError(x.Message);
			}

			var model = viewModel.EditedAlbum;

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names != null && model.Names.All(n => n == null || string.IsNullOrEmpty(n.Value))) {
				ModelState.AddModelError("Names", AlbumValidationErrors.UnspecifiedNames);
			}

			if (model.OriginalRelease != null && model.OriginalRelease.ReleaseDate != null && !OptionalDateTime.IsValid(model.OriginalRelease.ReleaseDate.Year, model.OriginalRelease.ReleaseDate.Day, model.OriginalRelease.ReleaseDate.Month))
				ModelState.AddModelError("ReleaseYear", "Invalid date");

			var coverPicUpload = Request.Files["coverPicUpload"];
			var pictureData = ParsePicture(coverPicUpload, "CoverPicture");

			if (coverPicUpload == null) {
				AddFormSubmissionError("Cover picture was null");
			}

			if (model.Pictures == null) {
				AddFormSubmissionError("List of pictures was null");
			}

			if (coverPicUpload != null && model.Pictures != null) {
				ParseAdditionalPictures(coverPicUpload, model.Pictures);				
			}

			if (!ModelState.IsValid) {
				return View(new AlbumEditViewModel(Service.GetAlbum(model.Id), PermissionContext, model));
			}

			try {
				queries.UpdateBasicProperties(model, pictureData);				
			} catch (InvalidPictureException) {
				ModelState.AddModelError("ImageError", "The uploaded image could not processed, it might be broken. Please check the file and try again.");
				return View(new AlbumEditViewModel(Service.GetAlbum(model.Id), PermissionContext, model));				
			}

        	return RedirectToAction("Details", new { id = model.Id });

        }

		public ActionResult Related(int id) {

			var related = queries.GetRelatedAlbums(id);
			return PartialView("RelatedAlbums", related);

		}

		[Authorize]
		public ActionResult RemoveTagUsage(long id) {

			var albumId = Service.RemoveTagUsage(id);
			TempData.SetStatusMessage("Tag usage removed");

			return RedirectToAction("ManageTagUsages", new { id = albumId });

		}

		public ActionResult Restore(int id) {

			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });

		}

		public ActionResult RevertToVersion(int archivedAlbumVersionId) {

			var result = Service.RevertToVersion(archivedAlbumVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });

		}

		[Authorize]
		public ActionResult Deleted() {

			return View();

		}

		[Authorize]
		public ActionResult ManageTagUsages(int id) {

			var album = Service.GetEntryWithTagUsages(id);
			return View(album);

		}

		public ActionResult Merge(int id) {

			var album = Service.GetAlbum(id);
			return View(album);

		}

		[HttpPost]
		public ActionResult Merge(int id, int targetAlbumId) {

			queries.Merge(id, targetAlbumId);

			return RedirectToAction("Edit", new { id = targetAlbumId });

		}

		[Authorize]
		public ActionResult MoveToTrash(int id) {

			Service.MoveToTrash(id);

			TempData.SetStatusMessage("Entry moved to trash");

			return RedirectToAction("Deleted");

		}

		public ActionResult UsersWithAlbumInCollection(int albumId = invalidId) {

			if (albumId == invalidId)
				return NoId();

			var users = Service.GetUsersWithAlbumInCollection(albumId);
			return PartialView(users);
			//return Json(users);

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetAlbumWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id = invalidId, int? ComparedVersionId = 0) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);

		}

    }

	public enum AlbumMatchProperty {

		Title,

	}

}
