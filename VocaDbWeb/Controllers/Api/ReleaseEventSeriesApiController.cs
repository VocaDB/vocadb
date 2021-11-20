#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for album release event series.
	/// </summary>
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Route("api/releaseEventSeries")]
	[ApiController]
	public class ReleaseEventSeriesApiController : ApiController
	{
		private const int DefaultMax = 10;
		private readonly EventQueries _queries;

		public ReleaseEventSeriesApiController(EventQueries queries)
		{
			_queries = queries;
		}

		/// <summary>
		/// Deletes an event series.
		/// </summary>
		/// <param name="id">ID of the series to be deleted.</param>
		/// <param name="notes">Notes.</param>
		/// <param name="hardDelete">
		/// If true, the entry is hard deleted. Hard deleted entries cannot be restored normally, but they will be moved to trash.
		/// If false, the entry is soft deleted, meaning it can still be restored.
		/// </param>
		[HttpDelete("{id:int}")]
		[Authorize]
		public void Delete(int id, string notes = "", bool hardDelete = false)
		{
			notes ??= string.Empty;

			if (hardDelete)
			{
				_queries.MoveSeriesToTrash(id, notes);
			}
			else
			{
				_queries.DeleteSeries(id, notes);
			}
		}

		/// <summary>
		/// Gets a page of event series.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="fields">Optional fields to include.</param>
		/// <param name="start">First item to be retrieved (optional).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional).</param>
		/// <param name="nameMatchMode">Match mode for event name (optional).</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of event series.</returns>
		[HttpGet("")]
		public PartialFindResult<ReleaseEventSeriesForApiContract> GetList(
			string query = "",
			ReleaseEventSeriesOptionalFields fields = ReleaseEventSeriesOptionalFields.None,
			int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
		) => _queries.FindSeries(SearchTextQuery.Create(query, nameMatchMode), new PagingProperties(start, maxResults, getTotalCount), lang, fields);

		/// <summary>
		/// Gets single event series by ID.
		/// </summary>
		[HttpGet("{id:int}")]
		public ReleaseEventSeriesForApiContract GetOne(
			int id,
			ReleaseEventSeriesOptionalFields fields = ReleaseEventSeriesOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
		) => _queries.GetOneSeries(id, lang, fields);

#nullable enable
		[HttpGet("{id:int}/details")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ReleaseEventSeriesDetailsForApiContract GetReleaseEventSeriesDetails(int id) => _queries.GetReleaseEventSeriesDetails(id);
#nullable disable
	}
}