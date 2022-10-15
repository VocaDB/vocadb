#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Model.Utils.Search;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Artist;

namespace VocaDb.Web.Controllers
{
	public class ArtistController : ControllerBase
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		private readonly ArtistQueries _queries;
		private readonly ArtistService _service;
		private readonly MarkdownParser _markdownParser;

		private ArtistEditViewModel CreateArtistEditViewModel(int id, ArtistForEditContract editedArtist)
		{
			return _queries.Get(id, artist => new ArtistEditViewModel(new ArtistContract(artist, PermissionContext.LanguagePreference), PermissionContext,
				EntryPermissionManager.CanDelete(PermissionContext, artist), editedArtist));
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
		public async Task<IActionResult> CreateReport(int artistId, ArtistReportType reportType, string notes, int? versionNumber)
		{
			var (created, _) = await _queries.CreateReport(artistId, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);

			return created ? NoContent() : BadRequest();
		}

		[Obsolete]
		public ActionResult Index(IndexRouteParams routeParams)
		{
			return RedirectToAction("Index", "Search", new SearchRouteParams
			{
				searchType = SearchType.Artist,
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
				return NotFound();

			WebHelper.VerifyUserAgent(Request);

			var model = _queries.GetDetails(id, GetHostnameForValidHit());

			var hasDescription = !model.Description.IsEmpty;
			var prop = PageProperties;
			prop.GlobalSearchType = EntryType.Artist;
			prop.Title = model.Name;
			prop.Subtitle = $"({Translate.ArtistTypeName(model.ArtistType)})";
			prop.Description = new ArtistDescriptionGenerator().GenerateDescription(model, _markdownParser.GetPlainText(model.Description.EnglishOrOriginal), Translate.ArtistTypeNames);
			//prop.CanonicalUrl = UrlMapper.FullAbsolute(Url.Action("Details", new { id }));
			prop.OpenGraph.Image = Url.ImageThumb(model, Model.Domain.Images.ImageSize.Original, fullUrl: true);
			prop.OpenGraph.Title = hasDescription ? $"{model.Name} ({Translate.ArtistTypeName(model.ArtistType)})" : model.Name;
			prop.OpenGraph.ShowTwitterCard = true;
			prop.Robots = model.Deleted ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

			return View("React/Index");
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

		[Authorize]
		public ActionResult Create()
		{
			return View("React/Index");
		}

#nullable enable
		//
		// GET: /Artist/Edit/5
		[Authorize]
		public ActionResult Edit(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			return View("React/Index");
		}
#nullable disable

		[Authorize]
		public ActionResult ManageTagUsages(int id)
		{
			var artist = Service.GetEntryWithTagUsages(id);

			PageProperties.Title = "Manage tag usages - " + artist.DefaultName;

			return View("React/Index");
		}

		public ActionResult Merge()
		{
			return View("React/Index");
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

			PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View("React/Index");
		}

		public ActionResult ViewVersion(int id = InvalidId, int? ComparedVersionId = null)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			PageProperties.Title = "Revision " + contract.ArchivedVersion.Version + " for " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View("React/Index");
		}
	}
}
