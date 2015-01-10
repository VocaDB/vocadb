using System;
using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;
using NLog;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Web.Models.Ext;

namespace VocaDb.Web.Controllers
{

	[SessionState(SessionStateBehavior.Disabled)]
    public class ExtController : ControllerBase {

	    private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly EntryUrlParser entryUrlParser;

		public ExtController(EntryUrlParser entryUrlParser) {
			this.entryUrlParser = entryUrlParser;
		}

		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang")]
        public ActionResult EmbedSong(int songId = invalidId, int pvId = invalidId) {

			if (songId == invalidId)
				return NoId();

			if (string.IsNullOrEmpty(Request.Params[LoginManager.LangParamName]))
				PermissionContext.OverrideLanguage(ContentLanguagePreference.Default);

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

		public ActionResult EntryToolTip(string url, string callback) {

			if (string.IsNullOrWhiteSpace(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var entryId = entryUrlParser.Parse(url, allowRelative: true);

			string data = string.Empty;
			var id = entryId.Id;

			switch (entryId.EntryType) {
				case EntryType.Album:
					data = RenderPartialViewToString("AlbumWithCoverPopupContent", Services.Albums.GetAlbum(id));
					break;
				case EntryType.Artist:
					data = RenderPartialViewToString("ArtistPopupContent", Services.Artists.GetArtist(id));
					break;
				case EntryType.Song:
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

			var entryId = entryUrlParser.Parse(url);

			if (entryId.EntryType != EntryType.Song) {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Only song embeds are supported");
			}

			var id = entryId.Id;

			var song = Services.Songs.GetSong(entryId.Id);
			var html = string.Format("<iframe src=\"{0}\" width=\"{1}\" height=\"{2}\"></iframe>",
				VocaUriBuilder.CreateAbsolute(Url.Action("EmbedSong", new {songId = id})), maxwidth, maxheight);

			return Object(new SongOEmbedResponse(song, maxwidth, maxheight, html), format);

		}

    }
}
