using System;
using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for activity feed.
	/// </summary>
	[RoutePrefix("api/activityEntries")]
	public class ActivityEntryApiController : ApiController
	{
		private const int defaultMax = 50;
		private readonly ActivityEntryQueries queries;

		public ActivityEntryApiController(ActivityEntryQueries queries)
		{
			this.queries = queries;
		}

		/// <summary>
		/// Gets a list of recent activity entries.
		/// </summary>
		/// <param name="before">Filter to return activity entries only before this date. Optional, by default no filter.</param>
		/// <param name="since">Filter to return activity entries only after this date. Optional, by default no filter.</param>
		/// <param name="userId">Filter by user Id. Optional, by default no filter.</param>
		/// <param name="editEvent">Filter by entry edit event (either Created or Updated). Optional, by default no filter.</param>
		/// <param name="maxResults">Maximum number of results to return. Default 50. Maximum value 500.</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="fields">Optional fields.</param>
		/// <param name="entryFields">Optional fields for entries.</param>
		/// <param name="lang">Content language preference.</param>
		/// <returns>List of activity entries.</returns>
		/// <remarks>
		/// Entries are always returned sorted from newest to oldest.
		/// Activity for deleted entries is not returned.
		/// </remarks>
		[Route("")]
		public PartialFindResult<ActivityEntryForApiContract> GetList(
			DateTime? before = null,
			DateTime? since = null,
 			int? userId = null,
			EntryEditEvent? editEvent = null,
			int maxResults = defaultMax,
			bool getTotalCount = false,
			ActivityEntryOptionalFields fields = ActivityEntryOptionalFields.None,
			EntryOptionalFields entryFields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) => queries.GetList(before, since, userId, editEvent, maxResults, getTotalCount, fields, entryFields, lang);
	}
}