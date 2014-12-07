using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Web.Controllers;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.API.v1.Controllers {

	public class AlbumApiController : Web.Controllers.ControllerBase {

		private const int maxResults = 10;
		private readonly AlbumQueries queries;
		private readonly AlbumService service;

		public AlbumApiController(AlbumService service, AlbumQueries queries) {
			this.service = service;
			this.queries = queries;
		}

		private AlbumService Service {
			get { return service; }
		}

		public ActionResult Details(int id = invalidId, 
			bool includeArtists = true, bool includeNames = true, bool includePVs = false, bool includeTags = true, bool includeWebLinks = false, 
			DataFormat format = DataFormat.Auto, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			if (id == invalidId)
				return NoId();

			var album = Service.GetAlbumWithMergeRecord(id, (a, m) => new AlbumForApiContract(a, m, lang, includeArtists, includeNames, includePVs, includeTags, includeWebLinks));

			return Object(album, format);

		}

		public ActionResult Index(string query, DiscType discType = DiscType.Unknown,
			int start = 0, bool getTotalCount = false, AlbumSortRule sort = AlbumSortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			bool includeArtists = true, bool includeNames = true, bool includePVs = false, bool includeTags = true, bool includeWebLinks = false,
			DataFormat format = DataFormat.Auto, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var queryParams = new AlbumQueryParams(SearchTextQuery.Create(query, nameMatchMode), discType, start, maxResults, false, getTotalCount, sort);

			var entries = Service.Find(a => new AlbumForApiContract(a, null, lang, includeArtists, includeNames, includePVs, includeTags, includeWebLinks), queryParams);

			return Object(entries, format);

		}

		public ActionResult Tracks(int id = invalidId, DataFormat format = DataFormat.Auto, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			if (id == invalidId)
				return NoId();

			var tracks = Service.GetAlbum(id, a => a.Songs.Select(s => new SongInAlbumContract(s, lang)).ToArray());

			return Object(tracks, format);

		}

	}

}