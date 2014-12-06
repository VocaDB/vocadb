using System;
using System.Web.Http;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// Controller for managing base class for common entries.
	/// </summary>
	[RoutePrefix("api/entries")]
	public class EntryApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;

		private readonly EntryQueries queries;
		private readonly OtherService otherService;

		private int GetMaxResults(int max) {
			return Math.Min(max, absoluteMax);	
		}

		public EntryApiController(EntryQueries queries, OtherService otherService) {
			this.queries = queries;
			this.otherService = otherService;
		}

		/// <summary>
		/// Find entries.
		/// </summary>
		/// <param name="query">Entry name query (optional).</param>
		/// <param name="tag">Filter by tag (optional).</param>
		/// <param name="status">Filter by entry status (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="nameMatchMode">Match mode for entry name (optional, defaults to Exact).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, MainPicture, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of entries.</returns>
		/// <example>http://vocadb.net/api/entries?query=164&amp;fields=MainPicture</example>
		[Route("")]
		public PartialFindResult<EntryForApiContract> GetList(
			string query, 
			string tag = null,
			EntryStatus? status = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			EntryOptionalFields fields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
			) {
			
			var ssl = WebHelper.IsSSL(Request);
			maxResults = GetMaxResults(maxResults);

			return queries.GetList(query, tag, status, start, maxResults, getTotalCount, nameMatchMode, fields, lang, ssl);

		}

		[Route("names")]
		public string[] GetNames(string query = "", int maxResults = 10) {
			
			return otherService.FindNames(query, maxResults);

		}

	}

}