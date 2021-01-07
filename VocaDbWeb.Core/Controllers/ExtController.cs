#nullable disable

using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
using VocaDb.Model.Utils;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Ext;

namespace VocaDb.Web.Controllers
{
	public class ExtController : ControllerBase
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		private readonly AlbumService _albumService;
		private readonly ArtistService _artistService;
		private readonly IAggregatedEntryImageUrlFactory _entryThumbPersister;
		private readonly IEntryUrlParser _entryUrlParser;
		private readonly EventQueries _eventQueries;
		private readonly SongQueries _songService;
		private readonly TagQueries _tagQueries;
		private readonly PVHelper _pvHelper;

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

		public ExtController(
			IEntryUrlParser entryUrlParser,
			IAggregatedEntryImageUrlFactory entryThumbPersister,
			AlbumService albumService,
			ArtistService artistService,
			EventQueries eventQueries,
			SongQueries songService,
			TagQueries tagQueries,
			PVHelper pvHelper)
		{
			_entryUrlParser = entryUrlParser;
			_entryThumbPersister = entryThumbPersister;
			_albumService = albumService;
			_artistService = artistService;
			_eventQueries = eventQueries;
			_songService = songService;
			_tagQueries = tagQueries;
			_pvHelper = pvHelper;
		}

#if !DEBUG
		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang;w;h", VaryByHeader = "Accept-Language")]
#endif
		public ActionResult EmbedSong(int songId = InvalidId, int pvId = InvalidId, int? w = null, int? h = null,
			ContentLanguagePreference lang = ContentLanguagePreference.Default)
		{
			if (songId == InvalidId)
				return NoId();

			var song = _songService.GetSongForApi(songId, SongOptionalFields.AdditionalNames | SongOptionalFields.PVs, lang);

			PVContract current = null;

			if (pvId != InvalidId)
			{
				current = song.PVs.FirstOrDefault(p => p.Id == pvId);
			}

			if (current == null)
			{
				current = _pvHelper.PrimaryPV(song.PVs);
			}

			var viewModel = new EmbedSongViewModel(_pvHelper)
			{
				Song = song,
				CurrentPV = current,
				Width = w,
				Height = h
			};

			return PartialView(viewModel);
		}

		public async Task<ActionResult> EntryToolTip(string url, string callback)
		{
			if (string.IsNullOrWhiteSpace(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var entryId = _entryUrlParser.Parse(url, allowRelative: true);

			if (entryId.IsEmpty)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid URL");
			}

			var data = string.Empty;
			var id = entryId.Id;

			switch (entryId.EntryType)
			{
				case EntryType.Album:
					data = await RenderPartialViewToStringAsync("AlbumWithCoverPopupContent", _albumService.GetAlbum(id));
					break;
				case EntryType.Artist:
					data = await RenderPartialViewToStringAsync("ArtistPopupContent", _artistService.GetArtist(id));
					break;
				case EntryType.ReleaseEvent:
					data = await RenderPartialViewToStringAsync("_EventPopupContent", _eventQueries.GetOne(id, ContentLanguagePreference.Default, ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Series));
					break;
				case EntryType.Song:
					data = await RenderPartialViewToStringAsync("SongPopupContent", _songService.GetSong(id));
					break;
				case EntryType.Tag:
					data = await RenderPartialViewToStringAsync("_TagPopupContent", _tagQueries.LoadTag(id, t =>
						new TagForApiContract(t, _entryThumbPersister, ContentLanguagePreference.Default, TagOptionalFields.AdditionalNames | TagOptionalFields.MainPicture)));
					break;
			}

			return Json(data, callback);
		}

		public async Task<ActionResult> OEmbed(string url, int maxwidth = 570, int maxheight = 400, DataFormat format = DataFormat.Json, bool responsiveWrapper = false)
		{
			if (string.IsNullOrEmpty(url))
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "URL must be specified");

			var entryId = _entryUrlParser.Parse(url);

			if (entryId.IsEmpty)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid URL");
			}

			if (entryId.EntryType != EntryType.Song)
			{
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Only song embeds are supported");
			}

			var id = entryId.Id;

			var song = _songService.GetSongForApi(entryId.Id, SongOptionalFields.ThumbUrl);
			var src = VocaUriBuilder.CreateAbsolute(Url.Action("EmbedSong", new { songId = id })).ToString();
			string html;

			if (responsiveWrapper)
			{
				html = await RenderPartialViewToStringAsync("OEmbedResponsive", new OEmbedParams { Width = maxwidth, Height = maxheight, Src = src });
			}
			else
			{
				html = $"<iframe src=\"{src}\" width=\"{maxwidth}\" height=\"{maxheight}\"></iframe>";
			}

			return Object(new SongOEmbedResponse(song, maxwidth, maxheight, html), format);
		}

		public Task<ActionResult> OEmbedResponsive(string url, int maxwidth = 570, int maxheight = 400, DataFormat format = DataFormat.Json)
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