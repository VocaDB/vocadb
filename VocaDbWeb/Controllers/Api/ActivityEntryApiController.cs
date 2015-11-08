using System;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for activity feed.
	/// </summary>
	[RoutePrefix("api/activityEntries")]
	public class ActivityEntryApiController : ApiController {

		private const int absoluteMax = 500;
		private const int defaultMax = 50;
		private readonly IRepository repository;
		private readonly IUserIconFactory userIconFactory;
		private readonly IUserPermissionContext permissionContext;
		private readonly EntryForApiContractFactory entryForApiContractFactory;

		public ActivityEntryApiController(IRepository repository, IUserIconFactory userIconFactory, 
			IUserPermissionContext permissionContext, EntryForApiContractFactory entryForApiContractFactory) {

			this.repository = repository;
			this.userIconFactory = userIconFactory;
			this.permissionContext = permissionContext;
			this.entryForApiContractFactory = entryForApiContractFactory;

		}

		/// <summary>
		/// Gets a list of recent activity entries.
		/// Entries are always returned sorted from newest to oldest.
		/// Activity for deleted entries is not returned.
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
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			bool ssl = WebHelper.IsSSL(Request);

			return repository.HandleQuery(ctx => {
				
				var query = ctx.Query<ActivityEntry>();

				if (before.HasValue && !since.HasValue) {
					query = query.Where(a => a.CreateDate < before.Value);
				}

				if (!before.HasValue && since.HasValue) {
					query = query.Where(a => a.CreateDate > since.Value);
				}

				if (before.HasValue && since.HasValue) {
					query = query.Where(a => a.CreateDate > since.Value && a.CreateDate < before.Value);
				}

				if (userId.HasValue) {
					query = query.Where(a => a.Author.Id == userId.Value);
				}

				if (editEvent.HasValue) {
					query = query.Where(a => a.EditEvent == editEvent.Value);
				}

				var activityEntries = query
					.OrderByDescending(a => a.CreateDate)
					.Take(maxResults)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted)
					.Select(a => new ActivityEntryForApiContract(a, 
						fields.HasFlag(ActivityEntryOptionalFields.Entry) ? entryForApiContractFactory.Create(a.EntryBase, entryFields, lang, ssl) : null, 
						userIconFactory, permissionContext, fields))
					.ToArray();

				var count = getTotalCount ? query.Count() : 0;

				return PartialFindResult.Create(activityEntries, count);

			});

		}

	}

}