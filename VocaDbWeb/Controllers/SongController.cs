using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using NLog;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Search;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Feeds;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Models;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.Song;
using System;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
    public class SongController : ControllerBase
    {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly SongQueries queries;
	    private readonly SongService service;
	    private readonly SongListQueries songListQueries;

		private SongService Service {
			get { return service; }
		}

		public SongController(SongService service, SongQueries queries, SongListQueries songListQueries) {
			this.service = service;
			this.queries = queries;
			this.songListQueries = songListQueries;
		}

		// Used from the song page
		[HttpPost]
		public void AddSongToList(int listId, int songId, string newListName = null) {

			if (listId != 0) {

				Service.AddSongToList(listId, songId);

			} else if (!string.IsNullOrWhiteSpace(newListName)) {

				var contract = new SongListForEditContract {
					Name = newListName,
					SongLinks = new[] {new SongInListEditContract { Song = new SongForApiContract { Id  = songId }, Order = 1 }}
				};

				songListQueries.UpdateSongList(contract, null);

			}

		}

		public ActionResult ArchivedVersionXml(int id) {

			var doc = Service.GetVersionXml(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);

		}

		public PartialViewResult Comments(int id = invalidId) {

			var comments = Service.GetComments(id);
			return PartialView("DiscussionContent", comments);

		}

		[HttpPost]
		public PartialViewResult CreateComment(int songId, string message) {

			var comment = queries.CreateComment(songId, message);

			return PartialView("Comment", comment);

		}

		[HttpPost]
		public void CreateReport(int songId, SongReportType reportType, string notes) {

			Service.CreateReport(songId, reportType, CfHelper.GetRealIp(Request), notes ?? string.Empty);

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

		public ActionResult Index(IndexRouteParams indexParams) {

			return RedirectToAction("Index", "Search", new SearchRouteParams {
				searchType = EntryType.Song, filter = indexParams.filter, sort = indexParams.sort,
				songType = indexParams.songType, onlyWithPVs = indexParams.onlyWithPVs, artistId = indexParams.artistId
			});

        }

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3, string pv1, string pv2, bool getPVInfo = false) {

			var result = queries.FindDuplicates(new[] { term1, term2, term3 }, new[] { pv1, pv2 }, getPVInfo);

			return LowercaseJson(result);

		}

		public ActionResult SongListsForSong(int songId = invalidId) {

			if (songId == invalidId)
				return NoId();

			var lists = Service.GetPublicSongListsForSong(songId);
			return PartialView("Partials/_SongInListsDialogContent", lists);
			//return LowercaseJson(lists);

		}

		public ActionResult SongListsForUser(int ignoreSongId) {

			var result = Service.GetSongListsForCurrentUser(ignoreSongId);
			return LowercaseJson(result);

		}

		// TODO: migrate to API
		public ActionResult DataById(int id = invalidId, bool includeArtists = false) {

			if (id == invalidId)
				return NoId();

			var song = Service.GetSong(id, s => new SongWithComponentsContract(s, PermissionContext.LanguagePreference, includeArtists));
			return new JsonNetResult { Data = song };

		}

        //
        // GET: /Song/Details/5

        public ActionResult Details(int id = invalidId, int albumId = 0) {

			if (id == invalidId)
				return NoId();

			WebHelper.VerifyUserAgent(Request);
			SetSearchEntryType(EntryType.Song);

			var model = new SongDetails(Service.GetSongDetails(id, albumId, WebHelper.IsValidHit(Request) ? WebHelper.GetRealHost(Request) : string.Empty));
			PageProperties.Description = model.Notes;

            return View(model);

        }

		[Authorize]
		public ActionResult Create(string pvUrl) {

			var model = new Create { PVUrl = pvUrl };

			return View(model);

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) && string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", ViewRes.Song.CreateStrings.NeedArtist);

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			try {
				var song = queries.Create(contract);
				return RedirectToAction("Edit", new { id = song.Id });
			} catch (VideoParseException x) {
				ModelState.AddModelError("PVUrl", x.Message);
				return View(model);
			}

		}
       
        //
        // GET: /Song/Edit/5 
        [Authorize]
        public ActionResult Edit(int id)
        {

			CheckConcurrentEdit(EntryType.Song, id);

			var model = new SongEditViewModel(Service.GetSong(id), PermissionContext);
			return View(model);

		}

        //
        // POST: /Song/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(SongEditViewModel viewModel)
        {

			var model = viewModel.EditedSong;

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names.All(n => string.IsNullOrEmpty(n.Value))) {
				ModelState.AddModelError("Names", SongValidationErrors.UnspecifiedNames);
			}

			if (model.Lyrics.Any(n => string.IsNullOrEmpty(n.Value))) {
				ModelState.AddModelError("Lyrics", "Lyrics cannot be empty");				
			}

			try {
				viewModel.CheckModel();
			} catch (InvalidFormException x) {
				AddFormSubmissionError(x.Message);
			}

			if (!ModelState.IsValid) {
				return View(new SongEditViewModel(Service.GetSong(model.Id), PermissionContext, model));				
			}

			queries.UpdateBasicProperties(model);

			return RedirectToAction("Details", new { id = model.Id });

        }

        //
        // GET: /Song/Delete/5

        public ActionResult Delete(int id)
        {
            
			Service.Delete(id);
			return RedirectToAction("Details", new { id });

        }

		public FeedResult Feed(IndexRouteParams indexParams) {

			WebHelper.VerifyUserAgent(Request);

			var pageSize = (indexParams.pageSize.HasValue ? Math.Min(indexParams.pageSize.Value, 30) : 30);
			var sortRule = indexParams.sort ?? SongSortRule.Name;
			var timeFilter = DateTimeUtils.ParseFromSimpleString(indexParams.since);
			var filter = indexParams.filter;
			var songType = indexParams.songType ?? SongType.Unspecified;
			var draftsOnly = indexParams.draftsOnly ?? false;
			var matchMode = indexParams.matchMode ?? NameMatchMode.Auto;
			var onlyWithPVs = indexParams.onlyWithPVs ?? false;
			var minScore = indexParams.minScore ?? 0;

			var textQuery = SearchTextQuery.Create(filter, matchMode);
			var queryParams = new SongQueryParams(textQuery,
				songType != SongType.Unspecified ? new[] { songType } : new SongType[] { },
				0, pageSize, draftsOnly, false, sortRule, false, false, null) {

					TimeFilter = timeFilter,
					OnlyWithPVs = onlyWithPVs,
					ArtistId = indexParams.artistId ?? 0,
					MinScore = minScore,
				};

			var result = Service.FindWithThumbPreferNotNico(queryParams);

			var fac = new SongFeedFactory();
			var feed = fac.Create(result.Items, 
				VocaUriBuilder.CreateAbsolute(Url.Action("Index", indexParams)), 
				song => RenderPartialViewToString("SongItem", song), 
				song => Url.Action("Details", new { id = song.Id }));

			return new FeedResult(new Atom10FeedFormatter(feed));

		}

		public FeedResult LatestVideos() {

			return Feed(new IndexRouteParams { onlyWithPVs = true, pageSize = 20, sort = SongSortRule.AdditionDate });

		}

        [Authorize]
        public ActionResult ManageTagUsages(int id) {

            var song = Service.GetEntryWithTagUsages(id);
            return View(song);

        }

		public ActionResult Merge(int id) {

			var song = Service.GetSong(id);
			return View(song);

		}

		[HttpPost]
		public ActionResult Merge(int id, int? targetSongId) {

			if (targetSongId == null) {
				ModelState.AddModelError("targetSongId", "Song must be selected");
				return Merge(id);
			}

			queries.Merge(id, targetSongId.Value);

			return RedirectToAction("Edit", new { id = targetSongId.Value });

		}

		/// <summary>
		/// Returns a PV player HTML by PV id (unique)
		/// </summary>
		public ActionResult PVForSong(int pvId = invalidId) {

			if (pvId == invalidId)
				return NoId();

			var pv = queries.PVForSong(pvId);
			return PartialView("PVs/_PVEmbedDynamic", pv);

		}

		/// <summary>
		/// Returns PV player HTML by song Id and PV service. Primary PV of that type will be selected.
		/// </summary>
		public ActionResult PVForSongAndService(int songId = invalidId, PVService? service = null) {

			if (songId == invalidId)
				return NoId();

			if (service == null)
				return NoId();

			var pv = queries.PVForSongAndService(songId, service.Value);
			return PartialView("PVs/_PVEmbedDynamic", pv);

		}

		public ActionResult PVEmbedNicoIFrame(int pvId = invalidId) {

		    if (pvId == invalidId)
		        return NoId();

			var pv = queries.PVForSong(pvId);
			return PartialView("PVs/_PVEmbedNicoIFrame", pv);

		}

		/// <summary>
		/// Returns a PV player with song rating by song Id. Primary PV will be chosen.
		/// </summary>
		public ActionResult PVPlayer(int id = invalidId, bool enableScriptAccess = false, string elementId = null,
			PVServices? pvServices = null) {

			if (id == invalidId)
				return NoId();

			var pv = queries.PrimaryPVForSong(id, pvServices);

			if (pv == null)
				return new EmptyResult();

			var embedParams = new PVEmbedParams { PV = pv, EnableScriptAccess = enableScriptAccess, ElementId = elementId };
			var view = RenderPartialViewToString("PVs/_PVEmbedDynamicCustom", embedParams);

			return LowercaseJson(new SongWithPVPlayerAndVoteContract {
				PlayerHtml = view, PVId = pv.PVId, PVService = pv.Service
			});

		}

		/// <summary>
		/// Returns a PV player with song rating by song Id. Primary PV will be chosen.
		/// </summary>
		public ActionResult PVPlayerWithRating(int songId = invalidId) {

			if (songId == invalidId)
				return NoId();

			var song = Service.GetSongWithPVAndVote(songId);
			var pv = PVHelper.PrimaryPV(song.PVs);

			if (pv == null)
				return new EmptyResult();

			var view = RenderPartialViewToString("PVs/_PVEmbedDynamic", pv);

			return LowercaseJson(new SongWithPVPlayerAndVoteContract { Song = song, PlayerHtml = view, PVService = pv.Service });

		}

		public ActionResult PVRedirect(PVService service, string pvId) {

			var song = Service.GetSongWithPV(service, pvId);

			if (song == null) {

				TempData.SetWarnMessage("Sorry, song not found! Maybe it hasn't been added yet.");
				return RedirectToAction("Index", "Home");

			} else {

				return RedirectToAction("Details", new { id = song.Id });

			}

		}

		public ActionResult Related(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var related = queries.GetRelatedSongs(id);
			return PartialView("RelatedSongs", related);

		}

        [Authorize]
        public ActionResult RemoveTagUsage(long id) {

            var songId = Service.RemoveTagUsage(id);
            TempData.SetStatusMessage("Tag usage removed");

            return RedirectToAction("ManageTagUsages", new { id = songId });

        }

		public ActionResult Restore(int id) {

			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });

		}

		public ActionResult RevertToVersion(int archivedSongVersionId) {

			var result = Service.RevertToVersion(archivedSongVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });

		}

		public PartialViewResult TagSelections(int songId = invalidId) {

			var contract = Service.GetTagSelections(songId, PermissionContext.LoggedUserId);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int songId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(songId, tagNameParts);

			return PartialView("TagList", tagUsages);

		}


		public string ThumbUrl(int id) {

			var songWithPVs = Service.GetSongWithPVAndVote(id);
			return (songWithPVs.ThumbUrl);

		}

		public ActionResult UsersWithSongRating(int songId = invalidId) {

			if (songId == invalidId)
				return NoId();

			var users = Service.GetUsersWithSongRating(songId);
			return PartialView(users);
			//return Json(users);

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetSongWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId) {

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);

		}

    }
}
