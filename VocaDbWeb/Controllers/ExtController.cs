using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.SessionState;
using NLog;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Ext;

namespace VocaDb.Web.Controllers
{

	[SessionState(SessionStateBehavior.Disabled)]
    public class ExtController : ControllerBase {

	    private static readonly Logger log = LogManager.GetCurrentClassLogger();

		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang")]
        public ActionResult EmbedSong(int songId = invalidId, int pvId = invalidId) {

			if (songId == invalidId)
				return NoId();

			if (string.IsNullOrEmpty(Request.Params[Model.Service.Security.LoginManager.LangParamName]))
				PermissionContext.OverrideLanguage(ContentLanguagePreference.Default);

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

		public ActionResult EntryToolTip(string url, string callback) {

			if (string.IsNullOrWhiteSpace(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var route = new RouteInfo(new Uri(url), AppConfig.HostAddress, HttpContext).RouteData;
			var controller = route.Values["controller"].ToString();
			var id = int.Parse(route.Values["id"].ToString());
			string data = string.Empty;

			switch (controller) {
				case "Album":
					data = RenderPartialViewToString("AlbumWithCoverPopupContent", Services.Albums.GetAlbum(id));
					break;
				case "Artist":
					data = RenderPartialViewToString("ArtistPopupContent", Services.Artists.GetArtist(id));
					break;
				case "Song":
					data = RenderPartialViewToString("SongPopupContent", Services.Songs.GetSongWithPVAndVote(id));
					break;
			}

			return Json(data, callback);
		}

		public ActionResult OEmbed(string url, int maxwidth = 570, int maxheight = 400, DataFormat format = DataFormat.Json) {

			if (string.IsNullOrEmpty(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			Uri uri;

			if (!Uri.TryCreate(url, UriKind.Absolute, out uri)) {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid URL");
			}

			var route = new RouteInfo(uri, AppConfig.HostAddress, HttpContext).RouteData;
			var controller = route.Values["controller"].ToString().ToLowerInvariant();

			if (controller != "song" && controller != "s") {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Only song embeds are supported");
			}

			int id;

			if (controller == "s") {
				var match = Regex.Match(route.Values["action"].ToString(), @"(\d+)");
				id = int.Parse(match.Groups[1].Value);
			}
			else {
				id = int.Parse(route.Values["id"].ToString());
			}

			var song = Services.Songs.GetSong(id);
			var html = string.Format("<iframe src=\"{0}\" width=\"{1}\" height=\"{2}\"></iframe>",
				VocaUriBuilder.CreateAbsolute(Url.Action("EmbedSong", new {songId = id})), maxwidth, maxheight);

			return Object(new SongOEmbedResponse(song, maxwidth, maxheight, html), format);

		}

    }
}
