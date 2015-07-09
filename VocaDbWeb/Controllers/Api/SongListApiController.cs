using System;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for song lists.
	/// </summary>
	[RoutePrefix("api/songLists")]
	public class SongListApiController : ApiController { 

		private const int absoluteMax = 100;
		private const int defaultMax = 10;
		private readonly SongListQueries queries;

		public SongListApiController(SongListQueries queries) {
			this.queries = queries;
		}

		[Route("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public SongListForEditContract GetForEdit(int id) {
			
			return queries.GetSongListForEdit(id);

		}

		/// <summary>
		/// Gets a list of songs in a song list.
		/// </summary>
		/// <param name="listId">ID of the song list.</param>
		/// <param name="pvServices">Filter by one or more PV services (separated by commas). The song will pass the filter if it has a PV for any of the matched services.</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Song sort rule (optional, by default songs are sorted by song list order).</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of songs.</returns>
		[Route("{listId:int}/songs")]
		public PartialFindResult<SongInListForApiContract> GetSongs(int listId, 
			[FromUri] PVServices? pvServices = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongSortRule? sort = null,
			SongOptionalFields fields = SongOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default
			) {
			
			maxResults = Math.Min(maxResults, absoluteMax);

			return queries.GetSongsInList(
				new SongListQueryParams {
					ListId = listId, 
					Paging = new PagingProperties(start, maxResults, getTotalCount),
					PVServices = pvServices,
					SortRule = sort
				}, 
				songInList => new SongInListForApiContract(songInList, lang, fields));

		}

		[ApiExplorerSettings(IgnoreApi=true)]
		[Route("import")]
		public ImportedSongListContract Import(string url, bool parseAll = true) {
			
			return queries.Import(url, parseAll);

		}

		[ApiExplorerSettings(IgnoreApi=true)]
		[Route("import-songs")]
		public PartialImportedSongs ImportSongs(string url, string pageToken, int maxResults = 20, bool parseAll = true) {
			
			return queries.ImportSongs(url, pageToken, maxResults, parseAll);

		}

	}

}