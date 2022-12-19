#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Venues;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for venues.
	/// </summary>
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Route("api/venues")]
	[ApiController]
	public class VenueApiController : ApiController
	{
		private const int DefaultMax = 10;
		private readonly VenueQueries _queries;

		public VenueApiController(VenueQueries queries)
		{
			_queries = queries;
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
		[HttpDelete("{id:int}")]
		[Authorize]
		[ValidateAntiForgeryToken]
		public void Delete(int id, string notes = "", bool hardDelete = false)
		{
			notes ??= string.Empty;

			if (hardDelete)
			{
				_queries.MoveToTrash(id, notes);
			}
			else
			{
				_queries.Delete(id, notes);
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
		[HttpGet("")]
		public PartialFindResult<VenueForApiContract> GetList(
			string query = "",
			VenueOptionalFields fields = VenueOptionalFields.None,
			int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Auto,
			ContentLanguagePreference lang = ContentLanguagePreference.Default,
			VenueSortRule sortRule = VenueSortRule.Name,
			double? latitude = null, double? longitude = null, double? radius = null, DistanceUnit distanceUnit = DistanceUnit.Kilometers
		)
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

			return _queries.Find(v => new VenueForApiContract(v, lang, fields), queryParams);
		}

		/// <summary>
		/// Creates a new report.
		/// </summary>
		/// <param name="id">Venue to be reported.</param>
		/// <param name="reportType">Report type.</param>
		/// <param name="notes">Notes. Optional.</param>
		/// <param name="versionNumber">Version to be reported. Optional.</param>
		[HttpPost("{id:int}/reports")]
		[RestrictBannedIP]
		public async Task<IActionResult> PostReport(int id, VenueReportType reportType, string notes, int? versionNumber)
		{
			var (created, _) = await _queries.CreateReport(id, reportType, WebHelper.GetRealHost(Request), notes ?? string.Empty, versionNumber);

			return created ? NoContent() : BadRequest();
		}

#nullable enable
		[HttpGet("{id:int}/details")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public VenueForApiContract GetDetails(int id) => _queries.GetDetails(id);

		[HttpGet("{id:int}/versions")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public EntryWithArchivedVersionsForApiContract<VenueForApiContract> GetWithArchivedVersions(int id) =>
			_queries.GetVenueWithArchivedVersionsForApi(id);

		[HttpGet("versions/{id:int}")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ArchivedVenueVersionDetailsForApiContract GetVersionDetails(int id, int comparedVersionId = 0) =>
			_queries.GetVersionDetailsForApi(id, comparedVersionId);

		[HttpGet("{id:int}")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public VenueForApiContract GetOne(int id) => _queries.GetOne(id);

		[HttpGet("{id:int}/for-edit")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public VenueForEditForApiContract GetForEdit(int id) => _queries.GetForEdit(id);

		[HttpPost("{id:int}")]
		[Authorize]
		[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
		[ValidateAntiForgeryToken]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ActionResult<int> Edit(VenueForEditForApiContract contract)
		{
			// Note: name is allowed to be whitespace, but not empty.
			if (contract.Names is null || contract.Names.All(n => string.IsNullOrEmpty(n.Value)))
			{
				ModelState.AddModelError("Names", "Name cannot be empty");
			}

			if ((contract.Coordinates is not null) && !OptionalGeoPoint.IsValid(contract.Coordinates.Latitude, contract.Coordinates.Longitude))
			{
				ModelState.AddModelError("Coordinates", "Invalid coordinates");
			}

			if (!ModelState.IsValid)
			{
				return ValidationProblem(ModelState);
			}

			var id = _queries.Update(contract);

			return id;
		}

		[HttpPost("versions/{archivedVersionId:int}/update-visibility")]
		[Authorize]
		[EnableCors(AuthenticationConstants.AuthenticatedCorsApiPolicy)]
		[ValidateAntiForgeryToken]
		[ApiExplorerSettings(IgnoreApi = true)]
		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedVenueVersion>(archivedVersionId, hidden);

			return NoContent();
		}
#nullable disable
	}
}
