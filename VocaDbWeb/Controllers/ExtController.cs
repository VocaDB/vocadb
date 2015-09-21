using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;
using NLog;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Ext;

namespace VocaDb.Web.Controllers
{

	[SessionState(SessionStateBehavior.Disabled)]
    public class ExtController : ControllerBase {

	    private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly AlbumService albumService;
		private readonly ArtistService artistService;
		private readonly IEntryUrlParser entryUrlParser;
		private readonly SongQueries songService;

		public ExtController(IEntryUrlParser entryUrlParser, AlbumService albumService, ArtistService artistService, SongQueries songService) {
			this.entryUrlParser = entryUrlParser;
			this.albumService = albumService;
			this.artistService = artistService;
			this.songService = songService;
		}

#if !DEBUG
		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang;w;h", VaryByHeader = "Accept-Language")]
#endif
		public ActionResult EmbedSong(int songId = invalidId, int pvId = invalidId, int? w = null, int? h = null) {

			if (songId == invalidId)
				return NoId();

			if (string.IsNullOrEmpty(Request.Params[LoginManager.LangParamName]))
				PermissionContext.LanguagePreferenceSetting.OverrideRequestValue(ContentLanguagePreference.Default);

			var song = songService.GetSongWithPVAndVote(songId, false);

			PVContract current = null;
    
			if (pvId != invalidId) {
				current = song.PVs.FirstOrDefault(p => p.Id == pvId);
			}
        
			if (current == null) {
				current = PVHelper.PrimaryPV(song.PVs);
			}

			var viewModel = new EmbedSongViewModel {
				Song = song,
				CurrentPV = current,
				Width = w,
				Height = h
			};

			return PartialView(viewModel);

		}

		public ActionResult EntryToolTip(string url, string callback) {

			if (string.IsNullOrWhiteSpace(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var entryId = entryUrlParser.Parse(url, allowRelative: true);

			if (entryId.IsEmpty) {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid URL");
			}

			var data = string.Empty;
			var id = entryId.Id;

			switch (entryId.EntryType) {
				case EntryType.Album:
					data = RenderPartialViewToString("AlbumWithCoverPopupContent", albumService.GetAlbum(id));
					break;
				case EntryType.Artist:
					data = RenderPartialViewToString("ArtistPopupContent", artistService.GetArtist(id));
					break;
				case EntryType.Song:
					data = RenderPartialViewToString("SongPopupContent", songService.GetSongWithPVAndVote(id, false));
					break;
			}

			return Json(data, callback);

		}

		public ActionResult OEmbed(string url, int maxwidth = 570, int maxheight = 400, DataFormat format = DataFormat.Json) {

			if (string.IsNullOrEmpty(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var entryId = entryUrlParser.Parse(url);

			if (entryId.IsEmpty) {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid URL");
			}

			if (entryId.EntryType != EntryType.Song) {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Only song embeds are supported");
			}

			var id = entryId.Id;

			var song = songService.GetSong(entryId.Id);
			var html = string.Format("<iframe src=\"{0}\" width=\"{1}\" height=\"{2}\"></iframe>",
				VocaUriBuilder.CreateAbsolute(Url.Action("EmbedSong", new {songId = id})), maxwidth, maxheight);

			return Object(new SongOEmbedResponse(song, maxwidth, maxheight, html), format);

		}

    }
}
