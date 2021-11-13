#nullable disable

using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources;
using VocaDb.Model.Service;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Model.Service.TagFormatting;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Search;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Album;

namespace VocaDb.Web.Controllers
{
	public class AlbumController : ControllerBase
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly AlbumDescriptionGenerator _albumDescriptionGenerator;
		private readonly MarkdownParser _markdownParser;
		private readonly AlbumQueries _queries;
		private readonly UserQueries _userQueries;
		private readonly PVHelper _pvHelper;

		private AlbumService Service { get; set; }

		private AlbumEditViewModel CreateAlbumEditViewModel(int id, AlbumForEditContract editedAlbum)
		{
			return Service.GetAlbum(id, album => new AlbumEditViewModel(new AlbumContract(album, PermissionContext.LanguagePreference), PermissionContext,
				EntryPermissionManager.CanDelete(PermissionContext, album), editedAlbum));
		}

		public AlbumController(
			AlbumService service,
			AlbumQueries queries,
			UserQueries userQueries,
			AlbumDescriptionGenerator albumDescriptionGenerator,
			MarkdownParser markdownParser,
			PVHelper pvHelper
		)
		{
			Service = service;
			_queries = queries;
			_userQueries = userQueries;
			_albumDescriptionGenerator = albumDescriptionGenerator;
			_markdownParser = markdownParser;
			_pvHelper = pvHelper;
		}

		public ActionResult ArchivedVersionCoverPicture(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetArchivedAlbumPicture(id);

			return Picture(contract);
		}

		public ActionResult ArchivedVersionXml(int id)
		{
			var doc = _queries.GetVersionXml<ArchivedAlbumVersion>(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);
		}

		[HttpPost]
		[RestrictBannedIP]
		public void CreateReport(int albumId, AlbumReportType reportType, string notes, int? versionNumber)
		{
			_queries.CreateReport(albumId, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);
		}

		//
		// GET: /Album/

		public ActionResult Index(IndexRouteParams routeParams)
		{
			return RedirectToAction("Index", "Search", new SearchRouteParams
			{
				searchType = SearchType.Album,
				filter = routeParams.filter,
				sort = routeParams.sort,
				discType = routeParams.discType
			});
		}

		public ActionResult FindDuplicate(string term1, string term2, string term3)
		{
			var result = Service.FindDuplicates(new[] { term1, term2, term3 });
			var contracts = result.Select(f => new DuplicateEntryResultContract<AlbumMatchProperty>(f, AlbumMatchProperty.Title)).ToArray();

			return LowercaseJson(contracts);
		}

		public ActionResult PopupContent(int id = InvalidId)
		{
			if (id == InvalidId)
				return NotFound();

			var album = Service.GetAlbum(id);
			return PartialView("AlbumPopupContent", album);
		}

		public ActionResult PopupWithCoverContent(int id = InvalidId)
		{
			if (id == InvalidId)
				return NotFound();

			var album = Service.GetAlbum(id);
			return PartialView("AlbumWithCoverPopupContent", album);
		}

		//
		// GET: /Album/Details/5

		public ActionResult Details(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			WebHelper.VerifyUserAgent(Request);

			var model = _queries.GetAlbumDetails(id, WebHelper.IsValidHit(Request) ? WebHelper.GetRealHost(Request) : string.Empty);

			var prop = PageProperties;
			prop.Title = model.Name;
			prop.CanonicalUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Details", new { id })).ToString();
			prop.GlobalSearchType = EntryType.Album;
			prop.OpenGraph.Image = Url.ImageThumb(model, Model.Domain.Images.ImageSize.Original, fullUrl: true);
			prop.OpenGraph.Type = OpenGraphTypes.Album;
			prop.OpenGraph.ShowTwitterCard = true;

			string titleAndArtist;
			if (!string.IsNullOrEmpty(model.ArtistString))
			{
				titleAndArtist = $"{model.Name} - {model.ArtistString}";
			}
			else
			{
				titleAndArtist = model.Name;
			}

			PageProperties.OpenGraph.Title = $"{titleAndArtist} ({Translate.DiscTypeName(model.DiscType)})";

			PageProperties.PageTitle = titleAndArtist;
			PageProperties.Subtitle = $"{model.ArtistString} ({Translate.DiscTypeName(model.DiscType)})";

			prop.Description = !model.Description.IsEmpty ?
				_markdownParser.GetPlainText(model.Description.EnglishOrOriginal) :
				_albumDescriptionGenerator.GenerateDescription(model, d => Translate.DiscTypeNames.GetName(d, CultureInfo.InvariantCulture));

			prop.Robots = model.Deleted ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

			return View(new AlbumDetails(model, PermissionContext, _pvHelper));
		}

		public ActionResult DownloadTags(int id = InvalidId, string formatString = "", int? discNumber = null, bool setFormatString = false, bool includeHeader = false)
		{
			if (id == InvalidId)
				return NoId();

			if (setFormatString)
			{
				_userQueries.SetAlbumFormatString(formatString);
			}
			else if (string.IsNullOrEmpty(formatString) && PermissionContext.IsLoggedIn)
			{
				formatString = PermissionContext.LoggedUser.AlbumFormatString;
			}

			if (string.IsNullOrEmpty(formatString))
				formatString = AlbumSongFormatter.TagFormatStrings[0];

			var album = Service.GetAlbum(id);
			var tagString = Service.GetAlbumTagString(id, formatString, discNumber, includeHeader);

			var enc = new UTF8Encoding(true);
			var data = enc.GetPreamble().Concat(enc.GetBytes(tagString)).ToArray();

			return File(data, "text/csv", album.Name + ".csv");
		}

		//[OutputCache(Duration = pictureCacheDurationSec, Location = OutputCacheLocation.Any, VaryByParam = "id,v")]
		public ActionResult CoverPicture(int id = InvalidId)
		{
			if (id == InvalidId)
				return NotFound();

			var album = Service.GetCoverPicture(id);

			return Picture(album);
		}

		public ActionResult CoverPictureThumb(int id = InvalidId)
		{
			if (id == InvalidId)
				return NotFound();

			var data = _queries.GetCoverPictureThumb(id);
			return Picture(data);
		}

		[Authorize]
		public ActionResult Create()
		{
			return View(new Create());
		}

		[HttpPost]
		public async Task<ActionResult> Create(Create model)
		{
			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji)
				&& string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", ViewRes.Album.CreateStrings.NeedArtist);

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			var album = await _queries.Create(contract);
			return RedirectToAction("Edit", new { id = album.Id });
		}

		//
		// GET: /Album/Edit/5
		[Authorize]
		public ActionResult Edit(int id)
		{
			CheckConcurrentEdit(EntryType.Album, id);

			return View(CreateAlbumEditViewModel(id, null));
		}

#nullable enable
		//
		// POST: /Album/Edit/5

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> Edit(AlbumEditViewModel viewModel)
		{
			// Unable to continue if viewmodel is null because we need the ID at least
			if (viewModel is null || viewModel.EditedAlbum is null)
			{
				s_log.Warn("Viewmodel was null");
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

			var model = viewModel.EditedAlbum;

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names is not null && model.Names.All(n => n is null || string.IsNullOrEmpty(n.Value)))
			{
				ModelState.AddModelError("Names", AlbumValidationErrors.UnspecifiedNames);
			}

			if (model.OriginalRelease is not null && model.OriginalRelease.ReleaseDate is not null && !OptionalDateTime.IsValid(model.OriginalRelease.ReleaseDate.Year, model.OriginalRelease.ReleaseDate.Day, model.OriginalRelease.ReleaseDate.Month))
				ModelState.AddModelError("ReleaseYear", "Invalid date");

			var coverPicUpload = Request.Form.Files["coverPicUpload"];
			var pictureData = ParsePicture(coverPicUpload, "CoverPicture", ImagePurpose.Main);

			if (model.Pictures is null)
			{
				AddFormSubmissionError("List of pictures was null");
			}

			if (model.Pictures is not null)
				ParseAdditionalPictures(coverPicUpload, model.Pictures);

			if (!ModelState.IsValid)
			{
				return View(CreateAlbumEditViewModel(model.Id, model));
			}

			try
			{
				await _queries.UpdateBasicProperties(model, pictureData);
			}
			catch (InvalidPictureException)
			{
				ModelState.AddModelError("ImageError", "The uploaded image could not processed, it might be broken. Please check the file and try again.");
				return View(CreateAlbumEditViewModel(model.Id, model));
			}

			return RedirectToAction("Details", new { id = model.Id });
		}
#nullable disable

		public ActionResult Related(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var related = _queries.GetRelatedAlbums(id);
			return PartialView("RelatedAlbums", related);
		}

		[Authorize]
		public ActionResult RemoveTagUsage(long id)
		{
			var albumId = _queries.RemoveTagUsage(id);
			TempData.SetStatusMessage("Tag usage removed");

			return RedirectToAction("ManageTagUsages", new { id = albumId });
		}

		public ActionResult Restore(int id)
		{
			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });
		}

		public ActionResult RevertToVersion(int archivedAlbumVersionId)
		{
			var result = _queries.RevertToVersion(archivedAlbumVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });
		}

		[Authorize]
		public ActionResult Deleted()
		{
			return View();
		}

		[Authorize]
		public ActionResult ManageTagUsages(int id)
		{
			var album = Service.GetEntryWithTagUsages(id);
			return View(album);
		}

		public ActionResult Merge(int id)
		{
			var album = Service.GetAlbum(id);
			return View(album);
		}

		[HttpPost]
		public ActionResult Merge(int id, int targetAlbumId)
		{
			_queries.Merge(id, targetAlbumId);

			return RedirectToAction("Edit", new { id = targetAlbumId });
		}

		[Authorize]
		public ActionResult MoveToTrash(int id)
		{
			_queries.MoveToTrash(id);

			TempData.SetStatusMessage("Entry moved to trash");

			return RedirectToAction("Deleted");
		}

		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedAlbumVersion>(archivedVersionId, hidden);

			return RedirectToAction("ViewVersion", new { id = archivedVersionId });
		}

		public ActionResult UsersWithAlbumInCollection(int albumId = InvalidId)
		{
			if (albumId == InvalidId)
				return NoId();

			var users = Service.GetUsersWithAlbumInCollection(albumId);
			return PartialView(users);
		}

		public ActionResult Versions(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetAlbumWithArchivedVersions(id);

			PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(new Versions(contract));
		}

		public ActionResult ViewVersion(int id = InvalidId, int? ComparedVersionId = 0)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			PageProperties.Title = "Revision " + contract.ArchivedVersion.Version + " for " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(contract);
		}
	}

	public enum AlbumMatchProperty
	{
		Title,
	}
}
