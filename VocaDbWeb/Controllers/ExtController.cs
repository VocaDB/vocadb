#nullable disable

using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;
using NLog;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Ext;

namespace VocaDb.Web.Controllers
{
	[SessionState(SessionStateBehavior.Disabled)]
	public class ExtController : ControllerBase
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly AlbumService albumService;
		private readonly ArtistService artistService;
		private readonly IAggregatedEntryImageUrlFactory entryThumbPersister;
		private readonly IEntryUrlParser entryUrlParser;
		private readonly EventQueries eventQueries;
		private readonly SongQueries songService;
		private readonly TagQueries tagQueries;

		protected ActionResult Object<T>(T obj, DataFormat format) where T : class
		{
			if (format == DataFormat.Xml)
				return Xml(obj);
			else
				return Json(obj);
		}

		protected ActionResult Xml<T>(T obj) where T : class
		{
			if (obj == null)
				return new EmptyResult();

			var content = XmlHelper.SerializeToUTF8XmlString(obj);
			return base.Xml(content);
		}

		public ExtController(IEntryUrlParser entryUrlParser, IAggregatedEntryImageUrlFactory entryThumbPersister,
			AlbumService albumService, ArtistService artistService, EventQueries eventQueries, SongQueries songService, TagQueries tagQueries)
		{
			this.entryUrlParser = entryUrlParser;
			this.entryThumbPersister = entryThumbPersister;
			this.albumService = albumService;
			this.artistService = artistService;
			this.eventQueries = eventQueries;
			this.songService = songService;
			this.tagQueries = tagQueries;
		}

#if !DEBUG
		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang;w;h", VaryByHeader = "Accept-Language")]
#endif
		public ActionResult EmbedSong(int songId = invalidId, int pvId = invalidId, int? w = null, int? h = null,
			ContentLanguagePreference lang = ContentLanguagePreference.Default)
		{
			if (songId == invalidId)
				return NoId();

			var song = songService.GetSongForApi(songId, SongOptionalFields.AdditionalNames | SongOptionalFields.PVs, lang);

			PVContract current = null;

			if (pvId != invalidId)
			{
				current = song.PVs.FirstOrDefault(p => p.Id == pvId);
			}

			if (current == null)
			{
				current = PVHelper.PrimaryPV(song.PVs);
			}

			var viewModel = new EmbedSongViewModel
			{
				Song = song,
				CurrentPV = current,
				Width = w,
				Height = h
			};

			return PartialView(viewModel);
		}

		public ActionResult EntryToolTip(string url, string callback)
		{
			if (string.IsNullOrWhiteSpace(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var entryId = entryUrlParser.Parse(url, allowRelative: true);

			if (entryId.IsEmpty)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid URL");
			}

			var data = string.Empty;
			var id = entryId.Id;

			switch (entryId.EntryType)
			{
				case EntryType.Album:
					data = RenderPartialViewToString("AlbumWithCoverPopupContent", albumService.GetAlbum(id));
					break;
				case EntryType.Artist:
					data = RenderPartialViewToString("ArtistPopupContent", artistService.GetArtist(id));
					break;
				case EntryType.ReleaseEvent:
					data = RenderPartialViewToString("_EventPopupContent", eventQueries.GetOne(id, ContentLanguagePreference.Default, ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series));
					break;
				case EntryType.Song:
					data = RenderPartialViewToString("SongPopupContent", songService.GetSong(id));
					break;
				case EntryType.Tag:
					data = RenderPartialViewToString("_TagPopupContent", tagQueries.LoadTag(id, t =>
						new TagForApiContract(t, entryThumbPersister, ContentLanguagePreference.Default, TagOptionalFields.AdditionalNames | TagOptionalFields.MainPicture)));
					break;
			}

			return Json(data, callback);
		}

		public ActionResult OEmbed(string url, int maxwidth = 570, int maxheight = 400, DataFormat format = DataFormat.Json, bool responsiveWrapper = false)
		{
			if (string.IsNullOrEmpty(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var entryId = entryUrlParser.Parse(url);

			if (entryId.IsEmpty)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid URL");
			}

			if (entryId.EntryType != EntryType.Song)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Only song embeds are supported");
			}

			var id = entryId.Id;

			var song = songService.GetSongForApi(entryId.Id, SongOptionalFields.ThumbUrl);
			var src = VocaUriBuilder.CreateAbsolute(Url.Action("EmbedSong", new { songId = id })).ToString();
			string html;

			if (responsiveWrapper)
			{
				html = RenderPartialViewToString("OEmbedResponsive", new OEmbedParams { Width = maxwidth, Height = maxheight, Src = src });
			}
			else
			{
				html = string.Format("<iframe src=\"{0}\" width=\"{1}\" height=\"{2}\"></iframe>", src, maxwidth, maxheight);
			}

			return Object(new SongOEmbedResponse(song, maxwidth, maxheight, html), format);
		}

		public ActionResult OEmbedResponsive(string url, int maxwidth = 570, int maxheight = 400, DataFormat format = DataFormat.Json)
		{
			return OEmbed(url, maxwidth, maxheight, format, true);
		}
	}

	public class OEmbedParams
	{
		public int Height { get; set; }
		public int Width { get; set; }
		public string Src { get; set; }
	}
}