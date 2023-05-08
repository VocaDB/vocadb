#nullable disable

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

/// <summary>
/// API queries for activity feed.
/// </summary>
[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/activityEntries")]
[ApiController]
public class ActivityEntryApiController : ApiController
{
	private const int DefaultMax = 50;
	private readonly ActivityEntryQueries _queries;
	private readonly ActivityFeedService _service;

	public ActivityEntryApiController(ActivityEntryQueries queries, ActivityFeedService service)
	{
		_queries = queries;
		_service = service;
	}

	/// <summary>
	/// Gets a list of recent activity entries.
	/// </summary>
	/// <param name="before">Filter to return activity entries only before this date. Optional, by default no filter.</param>
	/// <param name="since">Filter to return activity entries only after this date. Optional, by default no filter.</param>
	/// <param name="userId">Filter by user Id. Optional, by default no filter.</param>
	/// <param name="editEvent">Filter by entry edit event (either Created or Updated). Optional, by default no filter.</param>
	/// <param name="entryType">Entry type. Optional.</param>
	/// <param name="maxResults">Maximum number of results to return. Default 50. Maximum value 500.</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="fields">Optional fields.</param>
	/// <param name="entryFields">Optional fields for entries.</param>
	/// <param name="lang">Content language preference. Optional.</param>
	/// <param name="sortRule">Sort rule. Optional.</param>
	/// <returns>List of activity entries.</returns>
	/// <remarks>
	/// Entries are always returned sorted from newest to oldest.
	/// Activity for deleted entries is not returned.
	/// </remarks>
	[HttpGet("")]
	public PartialFindResult<ActivityEntryForApiContract> GetList(
		DateTime? before = null,
		DateTime? since = null,
 			int? userId = null,
		EntryEditEvent? editEvent = null,
		EntryType entryType = EntryType.Undefined,
		int maxResults = DefaultMax,
		bool getTotalCount = false,
		ActivityEntryOptionalFields fields = ActivityEntryOptionalFields.None,
		EntryOptionalFields entryFields = EntryOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default,
		ActivityEntrySortRule sortRule = ActivityEntrySortRule.CreateDateDescending
	) => _queries.GetList(before, since, userId, editEvent, entryType, maxResults, getTotalCount, fields, entryFields, lang, sortRule);

	[HttpGet("followedArtistActivity")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public PartialFindResult<ActivityEntryForApiContract> FollowedArtistActivity() => _service.GetFollowedArtistActivity(DefaultMax);
}