#nullable disable

using System.Net;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Ext;

namespace VocaDb.Web.Controllers
{
	public class ExtController : ControllerBase
	{
		private readonly IEntryUrlParser _entryUrlParser;
		private readonly SongQueries _songService;
		private readonly PVHelper _pvHelper;

		protected ActionResult Object<T>(T obj, DataFormat format) where T : class
		{
			if (format == DataFormat.Xml)
				return Xml(obj);
			else
				return Json(obj);
		}

#nullable enable
		protected ActionResult Xml<T>(T? obj) where T : class
		{
			if (obj == null)
				return new EmptyResult();

			var content = XmlHelper.SerializeToUTF8XmlString(obj);
			return base.Xml(content);
		}
#nullable disable

		public ExtController(
			IEntryUrlParser entryUrlParser,
			SongQueries songService,
			PVHelper pvHelper
		)
		{
			_entryUrlParser = entryUrlParser;
			_songService = songService;
			_pvHelper = pvHelper;
		}

#if !DEBUG
		[ResponseCache(Duration = 600, VaryByQueryKeys = new[] { "*" }, VaryByHeader = "Accept-Language")]
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