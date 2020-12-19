#nullable disable

using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NLog;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Utils.Search;
using VocaDb.Web.Code.Exceptions;
using System.Drawing;
using System.Globalization;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Utils;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Models.Artist;
using VocaDb.Web.Helpers;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Web.Code.Security;
using VocaDb.Model.Domain.Images;
using System.Threading.Tasks;

namespace VocaDb.Web.Controllers
{
	public class ArtistController : ControllerBase
	{
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		private readonly ArtistQueries _queries;
		private readonly ArtistService _service;
		private readonly MarkdownParser _markdownParser;

		private ArtistEditViewModel CreateArtistEditViewModel(int id, ArtistForEditContract editedArtist)
		{
			return _queries.Get(id, album => new ArtistEditViewModel(new ArtistContract(album, PermissionContext.LanguagePreference), PermissionContext,
				EntryPermissionManager.CanDelete(PermissionContext, album), editedArtist));
		}

		public ArtistController(ArtistService service, ArtistQueries queries, MarkdownParser markdownParser)
		{
			_service = service;
			_queries = queries;
			_markdownParser = markdownParser;
		}

		private ArtistService Service => _service;

		public ActionResult ArchivedVersionPicture(int id)
		{
			var contract = Service.GetArchivedArtistPicture(id);

			return Picture(contract);
		}

		public ActionResult ArchivedVersionXml(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var doc = _queries.GetVersionXml<ArchivedArtistVersion>(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);
		}

		[HttpPost]
		[RestrictBannedIP]
		public void CreateReport(int artistId, ArtistReportType reportType, string notes, int? versionNumber)
		{
			_queries.CreateReport(artistId, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);
		}

		[Obsolete]
		public ActionResult Index(IndexRouteParams routeParams)
		{
			return RedirectToAction("Index", "Search", new SearchRouteParams
			{
				searchType = EntryType.Artist,
				filter = routeParams.filter,
				sort = routeParams.sort,
				artistType = routeParams.artistType
			});
		}

		[Authorize]
		public ActionResult RemoveTagUsage(long id)
		{
			var artistId = _queries.RemoveTagUsage(id);
			TempData.SetStatusMessage("Tag usage removed");

			return RedirectToAction("ManageTagUsages", new { id = artistId });
		}

		public ActionResult Restore(int id)
		{
			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });
		}

		public async Task<ActionResult> RevertToVersion(int archivedArtistVersionId)
		{
			var result = await _queries.RevertToVersion(archivedArtistVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });
		}

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3, string linkUrl)
		{
			var result = _queries.FindDuplicates(new[] { term1, term2, term3 }, linkUrl).Select(e => new DuplicateEntryResultContract<ArtistEditableFields>(e, ArtistEditableFields.Names));
			return LowercaseJson(result);
		}

		//
		// GET: /Artist/Details/5

		public ActionResult Details(int id = InvalidId)
		{
			if (id == InvalidId)
				return HttpNotFound();

			WebHelper.VerifyUserAgent(Request);

			var model = _queries.GetDetails(id, GetHostnameForValidHit());

			var hasDescription = !model.Description.IsEmpty;
			var prop = PageProperties;
			prop.GlobalSearchType = EntryType.Artist;
			prop.Title = model.Name;
			prop.Subtitle = $"({Translate.ArtistTypeName(model.ArtistType)})";
			prop.Description = new ArtistDescriptionGenerator().GenerateDescription(model, _markdownParser.GetPlainText(model.Description.EnglishOrOriginal), Translate.ArtistTypeNames);
			prop.CanonicalUrl = UrlMapper.FullAbsolute(Url.Action("Details", new { id }));
			prop.OpenGraph.Image = Url.ImageThumb(model, Model.Domain.Images.ImageSize.Original, fullUrl: true);
			prop.OpenGraph.Title = hasDescription ? $"{model.Name} ({Translate.ArtistTypeName(model.ArtistType)})" : model.Name;
			prop.OpenGraph.ShowTwitterCard = true;

			return View(model);
		}

		public ActionResult Picture(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var artist = Service.GetArtistPicture(id);

			return Picture(artist);
		}

		public ActionResult PictureThumb(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var artist = _queries.GetPictureThumb(id);
			return Picture(artist);
		}

		public ActionResult PopupContent(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var artist = Service.GetArtist(id);
			return PartialView("ArtistPopupContent", artist);
		}

		[Authorize]
		public ActionResult Create()
		{
			return View(new Create());
		}

		[HttpPost]
		public async Task<ActionResult> Create(Create model)
		{
			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) && string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (string.IsNullOrWhiteSpace(model.Description) && string.IsNullOrWhiteSpace(model.WebLinkUrl))
				ModelState.AddModelError("Description", ViewRes.Artist.CreateStrings.NeedWebLinkOrDescription);

			var coverPicUpload = Request.Files["pictureUpload"];
			var pictureData = ParsePicture(coverPicUpload, "Picture", ImagePurpose.Main);

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();
			contract.PictureData = pictureData;

			ArtistContract artist;
			try
			{
				artist = await _queries.Create(contract);
			}
			catch (InvalidPictureException)
			{
				ModelState.AddModelError("Picture", "The uploaded image could not processed, it might be broken. Please check the file and try again.");
				return View(model);
			}

			return RedirectToAction("Edit", new { id = artist.Id });
		}

		//
		// GET: /Artist/Edit/5
		[Authorize]
		public ActionResult Edit(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			CheckConcurrentEdit(EntryType.Artist, id);

			var model = CreateArtistEditViewModel(id, null);
			return View(model);
		}

		//
		// POST: /Artist/Edit/5
		[HttpPost]
		[Authorize]
		public async Task<ActionResult> Edit(ArtistEditViewModel viewModel)
		{
			// Unable to continue if viewmodel is null because we need the ID at least
			if (viewModel == null || viewModel.EditedArtist == null)
			{
				_log.Warn("Viewmodel was null");
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Viewmodel was null - probably JavaScript is disabled");
			}

			try
			{
				viewModel.CheckModel();
			}
			catch (InvalidFormException x)
			{
				AddFormSubmissionError(x.Message);
			}

			var model = viewModel.EditedArtist;

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names.All(n => n == null || string.IsNullOrEmpty(n.Value)))
			{
				ModelState.AddModelError("Names", Model.Resources.ArtistValidationErrors.UnspecifiedNames);
			}

			var coverPicUpload = Request.Files["pictureUpload"];
			var pictureData = ParsePicture(coverPicUpload, "Picture", ImagePurpose.Main);

			ParseAdditionalPictures(coverPicUpload, model.Pictures);

			if (!ModelState.IsValid)
			{
				return View("Edit", CreateArtistEditViewModel(model.Id, model));
			}

			try
			{
				await _queries.Update(model, pictureData, PermissionContext);
			}
			catch (InvalidPictureException)
			{
				ModelState.AddModelError("ImageError", "The uploaded image could not processed, it might be broken. Please check the file and try again.");
				return View("Edit", CreateArtistEditViewModel(model.Id, model));
			}

			return RedirectToAction("Details", new { id = model.Id });
		}

		[Authorize]
		public ActionResult ManageTagUsages(int id)
		{
			var artist = Service.GetEntryWithTagUsages(id);
			return View(artist);
		}

		public ActionResult Merge(int id)
		{
			var artist = Service.GetArtist(id);
			return View(artist);
		}

		[HttpPost]
		public ActionResult Merge(int id, int targetArtistId)
		{
			Service.Merge(id, targetArtistId);

			return RedirectToAction("Edit", new { id = targetArtistId });
		}

		public ActionResult Name(int id)
		{
			var contract = Service.GetArtist(id);
			return Content(contract.Name);
		}

		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedArtistVersion>(archivedVersionId, hidden);

			return RedirectToAction("ViewVersion", new { id = archivedVersionId });
		}

		public ActionResult Versions(int id)
		{
			var contract = Service.GetArtistWithArchivedVersions(id);

			return View(new Versions(contract));
		}

		public ActionResult ViewVersion(int id = InvalidId, int? ComparedVersionId = null)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);
		}
	}
}
