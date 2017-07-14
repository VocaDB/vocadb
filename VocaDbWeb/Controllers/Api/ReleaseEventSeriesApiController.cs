using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for album release event series.
	/// </summary>
	[RoutePrefix("api/releaseEventSeries")]
	public class ReleaseEventSeriesApiController : ApiController {

		private const int defaultMax = 10;
		private readonly EventQueries queries;

		public ReleaseEventSeriesApiController(EventQueries queries) {
			this.queries = queries;
		}

		/// <summary>
		/// Deletes an event series.
		/// </summary>
		/// <param name="id">ID of the series to be deleted.</param>
		/// <param name="notes">Notes.</param>
		[Route("{id:int}")]
		[Authorize]
		public void Delete(int id, string notes = "") {

			queries.DeleteSeries(id, notes ?? string.Empty);

		}

		/// <summary>
		/// Gets a page of event series.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="start">First item to be retrieved (optional).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional).</param>
		/// <param name="nameMatchMode">Match mode for event name (optional).</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of event series.</returns>
		[Route("")]
		public PartialFindResult<ReleaseEventSeriesContract> GetList(
			string query = "", 
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return queries.FindSeries(s => new ReleaseEventSeriesContract(s, lang), SearchTextQuery.Create(query, nameMatchMode), 
				new PagingProperties(start, maxResults, getTotalCount));

		}

	}

}