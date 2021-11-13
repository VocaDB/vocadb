#nullable disable

using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources;
using VocaDb.Model.Service;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Search;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Feeds;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.Song;

namespace VocaDb.Web.Controllers
{
	public class SongController : ControllerBase
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private readonly MarkdownParser _markdownParser;
		private readonly SongQueries _queries;
		private readonly SongService _service;
		private readonly SongListQueries _songListQueries;
		private readonly PVHelper _pvHelper;

		private SongService Service => _service;

		public SongController(
			SongService service,
			SongQueries queries,
			SongListQueries songListQueries,
			MarkdownParser markdownParser,
			PVHelper pvHelper
		)
		{
			_service = service;
			_queries = queries;
			_songListQueries = songListQueries;
			_markdownParser = markdownParser;
			_pvHelper = pvHelper;
		}

		// Used from the song page
		[HttpPost]
		public void AddSongToList(int listId, int songId, string notes = null, string newListName = null)
		{
			if (listId != 0)
			{
				Service.AddSongToList(listId, songId, notes ?? string.Empty);
			}
			else if (!string.IsNullOrWhiteSpace(newListName))
			{
				var contract = new SongListForEditContract
				{
					Name = newListName,
					SongLinks = new[] {new SongInListEditContract {
						Song = new SongForApiContract { Id  = songId },
						Order = 1,
						Notes = notes ?? string.Empty
					}}
				};

				_songListQueries.UpdateSongList(contract, null);
			}
		}

		public ActionResult ArchivedVersionXml(int id)
		{
			var doc = _queries.GetVersionXml<ArchivedSongVersion>(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);
		}

		[HttpPost]
		[RestrictBannedIP]
		public void CreateReport(int songId, SongReportType reportType, string notes, int? versionNumber)
		{
			_queries.CreateReport(songId, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);
		}

		public ActionResult Index(IndexRouteParams indexParams)
		{
			return RedirectToAction("Index", "Search", new SearchRouteParams
			{
				searchType = SearchType.Song,
				filter = indexParams.filter,
				sort = indexParams.sort,
				songType = indexParams.songType,
				onlyWithPVs = indexParams.onlyWithPVs,
				artistId = indexParams.artistId
			});
		}

		public ActionResult SongListsForSong(int songId = InvalidId)
		{
			if (songId == InvalidId)
				return NoId();

			var lists = Service.GetPublicSongListsForSong(songId);
			return PartialView("Partials/_SongInListsDialogContent", lists);
		}

		public ActionResult SongListsForUser(int ignoreSongId = InvalidId)
		{
			if (ignoreSongId == InvalidId)
				return NoId();

			var result = Service.GetSongListsForCurrentUser(ignoreSongId);
			return LowercaseJson(result);
		}

		//
		// GET: /Song/Details/5

		public ActionResult Details(int id = InvalidId, int albumId = 0)
		{
			if (id == InvalidId)
				return NoId();

			WebHelper.VerifyUserAgent(Request);

			var contract = _queries.GetSongDetails(id, albumId, GetHostnameForValidHit(), null, WebHelper.GetUserLanguageCodes(Request));
			var model = new SongDetails(contract, PermissionContext, _pvHelper);

			var hasDescription = !model.Notes.IsEmpty;
			var prop = PageProperties;
			prop.GlobalSearchType = EntryType.Song;
			prop.Title = model.Name;
			prop.Description = hasDescription ?
				_markdownParser.GetPlainText(model.Notes.EnglishOrOriginal) :
				new SongDescriptionGenerator().GenerateDescription(contract.Song, Translate.SongTypeNames);
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
			prop.PageTitle = titleAndArtist;

			prop.Subtitle = $"{model.ArtistString} ({Translate.SongTypeNames[model.SongType]})";

			if (!string.IsNullOrEmpty(model.ThumbUrlMaxSize))
			{
				prop.OpenGraph.Image = model.ThumbUrlMaxSize;
			}

			prop.CanonicalUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Details", new { id })).ToString();
			prop.OpenGraph.Title = hasDescription ? $"{titleAndArtist} ({Translate.SongTypeNames[model.SongType]})" : model.Name;
			prop.OpenGraph.Type = OpenGraphTypes.Song;

			prop.Robots = model.Deleted ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

			return View(model);
		}

		[Authorize]
		public ActionResult Create(string pvUrl)
		{
			var model = new Create { PVUrl = pvUrl };

			return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> Create(Create model)
		{
			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) && string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", ViewRes.Song.CreateStrings.NeedArtist);

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			try
			{
				var song = await _queries.Create(contract);
				return RedirectToAction("Edit", new { id = song.Id });
			}
			catch (VideoParseException x)
			{
				ModelState.AddModelError("PVUrl", x.Message);
				return View(model);
			}
		}

		private int InstrumentalTagId => _queries.InstrumentalTagId;

		//
		// GET: /Song/Edit/5 
		[Authorize]
		public ActionResult Edit(int id, int? albumId = null)
		{
			CheckConcurrentEdit(EntryType.Song, id);

			var model = Service.GetSong(id, song => new SongEditViewModel(new SongContract(song, PermissionContext.LanguagePreference, false),
				PermissionContext, EntryPermissionManager.CanDelete(PermissionContext, song), InstrumentalTagId, albumId: albumId));

			return View(model);
		}

		//
		// POST: /Song/Edit/5
		[HttpPost]
		[Authorize]
		public async Task<ActionResult> Edit(SongEditViewModel viewModel)
		{
			// Unable to continue if viewmodel is null because we need the ID at least
			if (viewModel?.EditedSong == null)
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

			var model = viewModel.EditedSong;

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names == null || model.Names.All(n => n == null || string.IsNullOrEmpty(n.Value)))
			{
				ModelState.AddModelError("Names", SongValidationErrors.UnspecifiedNames);
			}

			if (model.Lyrics != null && model.Lyrics.Any(n => string.IsNullOrEmpty(n.Value)))
			{
				ModelState.AddModelError("Lyrics", "Lyrics cannot be empty");
			}

			if (!ModelState.IsValid)
			{
				return View(Service.GetSong(model.Id, song => new SongEditViewModel(new SongContract(song, PermissionContext.LanguagePreference, false),
					PermissionContext, EntryPermissionManager.CanDelete(PermissionContext, song), InstrumentalTagId, model)));
			}

			await _queries.UpdateBasicProperties(model);

			return RedirectToAction("Details", new { id = model.Id, albumId = viewModel.AlbumId });
		}

		[HttpPost]
		[Authorize]
		public ActionResult PostMedia(IFormFile file)
		{
			if (file == null || file.Length == 0)
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "File cannot be empty");

			PermissionContext.VerifyPermission(PermissionToken.UploadMedia);

			if (!LocalFileManager.MimeTypes.Contains(file.ContentType))
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Unsupported file type");
			}

			if (file.Length > LocalFileManager.MaxMediaSizeBytes)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "File too large");
			}

			var pv = new LocalFileManager().CreatePVContract(new AspNetCoreHttpPostedFile(file), User.Identity, PermissionContext.LoggedUser);

			return LowercaseJson(pv);
		}

		[ResponseCache(Location = ResponseCacheLocation.Client, Duration = 3600)]
		public ActionResult PopupContent(int id = InvalidId)
		{
			if (id == InvalidId)
				return NotFound();

			var song = _queries.GetSong(id);
			return PartialView("SongPopupContent", song);
		}

		[ResponseCache(Location = ResponseCacheLocation.Client, Duration = 3600)]
		public async Task<ActionResult> PopupContentWithVote(int id = InvalidId, int? version = null, string callback = null)
		{
			if (id == InvalidId)
				return NotFound();

			var song = _queries.GetSongWithPVAndVote(id, false, includePVs: false);

			if (string.IsNullOrEmpty(callback))
			{
				return PartialView("_SongWithVotePopupContent", song);
			}
			else
			{
				return Json(await RenderPartialViewToStringAsync("_SongWithVotePopupContent", song), callback);
			}
		}

		public async Task<FeedResult> Feed(IndexRouteParams indexParams)
		{
			WebHelper.VerifyUserAgent(Request);

			var pageSize = (indexParams.pageSize.HasValue ? Math.Min(indexParams.pageSize.Value, 30) : 30);
			var sortRule = indexParams.sort ?? SongSortRule.Name;
			var timeFilter = DateTimeUtils.ParseFromSimpleString(indexParams.since);
			var filter = indexParams.filter;
			var songType = indexParams.songType ?? SongType.Unspecified;
			var matchMode = indexParams.matchMode ?? NameMatchMode.Auto;
			var onlyWithPVs = indexParams.onlyWithPVs ?? false;
			var minScore = indexParams.minScore ?? 0;

			var textQuery = SearchTextQuery.Create(filter, matchMode);
			var queryParams = new SongQueryParams(textQuery,
				songType != SongType.Unspecified ? new[] { songType } : new SongType[] { },
				0, pageSize, false, sortRule, false, false, null)
			{
				ArtistParticipation = {
						ArtistIds = !indexParams.artistId.HasValue || indexParams.artistId.Value == 0 ? null : new [] { indexParams.artistId.Value }
					},
				TimeFilter = timeFilter,
				OnlyWithPVs = onlyWithPVs,
				MinScore = minScore,
				UserCollectionId = indexParams.userCollectionId ?? 0,
				FollowedByUserId = indexParams.followedByUserId ?? 0
			};

			var result = Service.FindWithThumbPreferNotNico(queryParams);

			var fac = new SongFeedFactory();
			var feed = await fac.CreateAsync(result.Items,
				VocaUriBuilder.CreateAbsolute(Url.Action("Index", indexParams)),
				song => RenderPartialViewToStringAsync("SongItem", song),
				song => Url.Action("Details", new { id = song.Id }));

			return new FeedResult(new Atom10FeedFormatter(feed));
		}

		public Task<FeedResult> LatestVideos()
		{
			return Feed(new IndexRouteParams { onlyWithPVs = true, pageSize = 20, sort = SongSortRule.AdditionDate });
		}

		[Authorize]
		public ActionResult ManageTagUsages(int id)
		{
			var song = Service.GetEntryWithTagUsages(id);
			return View(song);
		}

		public ActionResult Merge(int id)
		{
			var song = _queries.GetSong(id);
			return View(song);
		}

		[HttpPost]
		public ActionResult Merge(int id, int? targetSongId)
		{
			if (targetSongId == null)
			{
				ModelState.AddModelError("targetSongId", "Song must be selected");
				return Merge(id);
			}

			_queries.Merge(id, targetSongId.Value);

			return RedirectToAction("Edit", new { id = targetSongId.Value });
		}

		/// <summary>
		/// Returns a PV player HTML by PV id (unique)
		/// </summary>
		public ActionResult PVForSong(int pvId = InvalidId)
		{
			if (pvId == InvalidId)
				return NoId();

			var pv = _queries.PVForSong(pvId);
			return PartialView("PVs/_PVEmbedDynamic", pv);
		}

		/// <summary>
		/// Returns PV player HTML by song Id and PV service. Primary PV of that type will be selected.
		/// </summary>
		public ActionResult PVForSongAndService(int songId = InvalidId, PVService? service = null)
		{
			if (songId == InvalidId)
				return NoId();

			if (service == null)
				return NoId();

			var pv = _queries.PVForSongAndService(songId, service.Value);
			return PartialView("PVs/_PVEmbedDynamic", pv);
		}

		/// <summary>
		/// Returns a PV player with song rating by song Id. Primary PV will be chosen.
		/// </summary>
		public async Task<ActionResult> PVPlayer(int id = InvalidId, bool enableScriptAccess = false, string elementId = null,
			PVServices? pvServices = null)
		{
			if (id == InvalidId)
				return NoId();

			var pv = _queries.PrimaryPVForSong(id, pvServices);

			if (pv == null)
				return new EmptyResult();

			var embedParams = new PVEmbedParams { PV = pv, EnableScriptAccess = enableScriptAccess, ElementId = elementId };
			var view = await RenderPartialViewToStringAsync("PVs/_PVEmbedDynamicCustom", embedParams);

			return LowercaseJson(new SongWithPVPlayerAndVoteContract
			{
				PlayerHtml = view,
				PVId = pv.PVId,
				PVService = pv.Service
			});
		}

		/// <summary>
		/// Returns a PV player with song rating by song Id. Primary PV will be chosen.
		/// </summary>
		public async Task<ActionResult> PVPlayerWithRating(int songId = InvalidId)
		{
			if (songId == InvalidId)
				return NoId();

			var song = _queries.GetSongWithPVAndVote(songId, true, GetHostnameForValidHit());
			var pv = _pvHelper.PrimaryPV(song.PVs);

			if (pv == null)
				return new EmptyResult();

			var view = await RenderPartialViewToStringAsync("PVs/_PVEmbedDynamic", pv);

			return LowercaseJson(new SongWithPVPlayerAndVoteContract { Song = song, PlayerHtml = view, PVService = pv.Service });
		}

		[Obsolete("Will be removed")]
		public ActionResult PVRedirect(PVService service, string pvId)
		{
			var song = Service.GetSongWithPV(service, pvId);

			if (song == null)
			{
				TempData.SetWarnMessage("Sorry, song not found! Maybe it hasn't been added yet.");
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return RedirectToAction("Details", new { id = song.Id });
			}
		}

		public ActionResult Related(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var related = _queries.GetRelatedSongs(id, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl, null);
			return PartialView("RelatedSongs", related);
		}

		[Authorize]
		public ActionResult RemoveTagUsage(long id)
		{
			var songId = _queries.RemoveTagUsage(id);
			TempData.SetStatusMessage("Tag usage removed");

			return RedirectToAction("ManageTagUsages", new { id = songId });
		}

		public ActionResult Restore(int id)
		{
			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });
		}

		public ActionResult RevertToVersion(int archivedSongVersionId)
		{
			var result = _queries.RevertToVersion(archivedSongVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });
		}

		public string ThumbUrl(int id)
		{
			return _queries.GetSong(id, s => s.GetThumbUrl());
		}

		public ActionResult TopRated()
		{
			return RedirectToActionPermanent("Rankings");
		}

		[Authorize]
		public ActionResult UpdateArtistString(int id)
		{
			_queries.UpdateArtistString(id);

			TempData.SetSuccessMessage("Artist string refreshed");

			return RedirectToAction("Details", new { id });
		}

		[Authorize]
		public ActionResult UpdateThumbUrl(int id)
		{
			_queries.UpdateThumbUrl(id);

			TempData.SetSuccessMessage("Thumbnail refreshed");

			return RedirectToAction("Details", new { id });
		}

		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedSongVersion>(archivedVersionId, hidden);

			return RedirectToAction("ViewVersion", new { id = archivedVersionId });
		}

		/// <summary>
		/// Refresh PV metadata.
		/// </summary>
		[Authorize]
		public async Task<ActionResult> RefreshPVMetadatas(int id)
		{
			await _queries.RefreshPVMetadatas(id);

			TempData.SetSuccessMessage("PV metadata refreshed");

			return RedirectToAction("Details", new { id });
		}

		public ActionResult Versions(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var contract = Service.GetSongWithArchivedVersions(id);

			PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(new Versions(contract));
		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId)
		{
			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			PageProperties.Title = "Revision " + contract.ArchivedVersion.Version + " for " + contract.Name;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View(contract);
		}
	}
}
