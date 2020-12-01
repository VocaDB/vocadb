using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Venues;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for venues.
	/// </summary>
	[RoutePrefix("api/venues")]
	public class VenueApiController : ApiController
	{
		private const int defaultMax = 10;
		private readonly VenueQueries queries;

		public VenueApiController(VenueQueries queries)
		{
			this.queries = queries;
		}

		/// <summary>
		/// Deletes a venue.
		/// </summary>
		/// <param name="id">ID of the venue to be deleted.</param>
		/// <param name="notes">Notes.</param>
		/// <param name="hardDelete">
		/// If true, the entry is hard deleted. Hard deleted entries cannot be restored normally, but they will be moved to trash.
		/// If false, the entry is soft deleted, meaning it can still be restored.
		/// </param>
		[Route("{id:int}")]
		[Authorize]
		public void Delete(int id, string notes = "", bool hardDelete = false)
		{
			notes = notes ?? string.Empty;

			if (hardDelete)
			{
				queries.MoveToTrash(id, notes);
			}
			else
			{
				queries.Delete(id, notes);
			}
		}

		/// <summary>
		/// Gets a page of event venue.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="fields">Optional fields to include.</param>
		/// <param name="start">First item to be retrieved (optional).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional).</param>
		/// <param name="nameMatchMode">Match mode for event name (optional).</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <param name="sortRule">Sort rule (optional, defaults to Name). Possible values are None, Name, Distance.</param>
		/// <param name="latitude">Latitude (optional).</param>
		/// <param name="longitude">Longitude (optional).</param>
		/// <param name="radius">Radius (optional).</param>
		/// <param name="distanceUnit">Unit of length (optional). Possible values are Kilometers, Miles.</param>
		/// <returns>Page of venue.</returns>
		[Route("")]
		public PartialFindResult<VenueForApiContract> GetList(
			string query = "",
			VenueOptionalFields fields = VenueOptionalFields.None,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			ContentLanguagePreference lang = ContentLanguagePreference.Default,
			VenueSortRule sortRule = VenueSortRule.Name,
			double? latitude = null, double? longitude = null, double? radius = null, DistanceUnit distanceUnit = DistanceUnit.Kilometers)
		{
			var textQuery = SearchTextQuery.Create(query, nameMatchMode);
			var queryParams = new VenueQueryParams
			{
				Coordinates = new GeoPointQueryParams(latitude, longitude),
				DistanceUnit = distanceUnit,
				Paging = new PagingProperties(start, maxResults, getTotalCount),
				Radius = radius,
				SortRule = sortRule,
				TextQuery = textQuery
			};

			return queries.Find(v => new VenueForApiContract(v, lang, fields), queryParams);
		}

		/// <summary>
		/// Creates a new report.
		/// </summary>
		/// <param name="id">Venue to be reported.</param>
		/// <param name="reportType">Report type.</param>
		/// <param name="notes">Notes. Optional.</param>
		/// <param name="versionNumber">Version to be reported. Optional.</param>
		[Route("{id:int}/reports")]
		[RestrictBannedIP]
		public void PostReport(int id, VenueReportType reportType, string notes, int? versionNumber) => queries.CreateReport(id, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);
	}
}
