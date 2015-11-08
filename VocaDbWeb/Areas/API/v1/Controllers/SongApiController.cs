using System;
using System.Net;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Web.Controllers;

namespace VocaDb.Web.API.v1.Controllers
{
	public class SongApiController : Web.Controllers.ControllerBase
    {

		private const int defaultMax = 10;
		private readonly SongService songService;
		private readonly SongQueries songQueries;

		public SongApiController(SongService songService, SongQueries songQueries) {
			this.songService = songService;
			this.songQueries = songQueries;
		}

		public ActionResult ByPV(PVService? service, string pvId, ContentLanguagePreference? lang, string callback,
			bool includeAlbums = true, bool includeArtists = true, bool includeNames = true, bool includePVs = false,
			bool includeTags = true, bool includeWebLinks = false,
			DataFormat format = DataFormat.Auto) {

			if (service == null)
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Service not specified or invalid");

			var song = songService.GetSongWithPV(s => new SongForApiContract(s, null, lang ?? ContentLanguagePreference.Default, 
				includeAlbums, includeArtists, includeNames, includePVs, includeTags, true, includeWebLinks), service.Value, pvId);

			return Object(song, format, callback);

		}

		public ActionResult ByPVBase(PVService? service, string pvId, string callback,
			DataFormat format = DataFormat.Auto) {

			if (service == null)
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Service not specified or invalid");

			var song = songService.GetSongWithPV(s => new EntryBaseContract(s), service.Value, pvId);

			return Object(song, format, callback);

		}

		public ActionResult ByName(string query, ContentLanguagePreference? lang, int? start, int? maxResults, NameMatchMode? nameMatchMode,
			bool includeAlbums = true, bool includeArtists = true, bool includeNames = true, bool includePVs = false, bool includeTags = true, bool includeWebLinks = false,
			string callback = null, DataFormat format = DataFormat.Auto) {

			var textQuery = SearchTextQuery.Create(query, nameMatchMode ?? NameMatchMode.Exact);
			var param = new SongQueryParams(textQuery, new SongType[] {}, 0, defaultMax, false, true, SongSortRule.Name, true, false, new int[] {});

			if (start.HasValue)
				param.Paging.Start = start.Value;

			if (maxResults.HasValue)
				param.Paging.MaxEntries = Math.Min(maxResults.Value, defaultMax);

			var songs = songService.Find(s => new SongForApiContract(s, null, lang ?? ContentLanguagePreference.Default, includeAlbums, includeArtists, includeNames, includePVs, includeTags, true, includeWebLinks), param);

			return Object(songs, format, callback);

		}

		public ActionResult Details(int id = invalidId,
			bool includeAlbums = true, bool includeArtists = true, bool includeNames = true, bool includePVs = false, bool includeTags = true, bool includeWebLinks = false,
			string callback = null, DataFormat format = DataFormat.Auto,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			if (id == invalidId)
				return NoId();

			var song = songQueries.GetSongWithMergeRecord(id, (s, m) => new SongForApiContract(s, m, lang, includeAlbums, includeArtists, includeNames, includePVs, includeTags, true, includeWebLinks));

			return Object(song, format, callback);

		}

    }
}
