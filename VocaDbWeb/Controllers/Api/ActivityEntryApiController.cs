using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Repositories;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for activity feed.
	/// </summary>
	[RoutePrefix("api/activityEntries")]
	public class ActivityEntryApiController : ApiController {

		private const int absoluteMax = 500;
		private const int defaultMax = 50;
		private readonly IEntryThumbPersister entryThumbPersister;
		private readonly IEntryImagePersisterOld entryImagePersisterOld;
		private readonly IRepository repository;
		private readonly IUserIconFactory userIconFactory;
		private readonly IUserPermissionContext permissionContext;

		public ActivityEntryApiController(IRepository repository, IUserIconFactory userIconFactory, 
			IEntryThumbPersister entryThumbPersister, IEntryImagePersisterOld entryImagePersisterOld,
			IUserPermissionContext permissionContext) {

			this.repository = repository;
			this.userIconFactory = userIconFactory;
			this.entryThumbPersister = entryThumbPersister;
			this.entryImagePersisterOld = entryImagePersisterOld;
			this.permissionContext = permissionContext;

		}

		/// <summary>
		/// Gets a list of recent activity entries.
		/// Entries are always returned sorted from newest to oldest.
		/// </summary>
		/// <param name="before">Filter to return activity entries only before this date. Optional, by default no filter.</param>
		/// <param name="userId">Filter by user Id. Optional, by default no filter.</param>
		/// <param name="editEvent">Filter by entry edit event (either Created or Updated). Optional, by default no filter.</param>
		/// <param name="maxResults">Maximum number of results to return. Default 50. Maximum value 500.</param>
		/// <param name="fields">Optional fields.</param>
		/// <param name="entryFields">Optional fields for entries.</param>
		/// <param name="lang">Content language preference.</param>
		/// <returns>List of activity entries.</returns>
		[Route("")]
		public IEnumerable<ActivityEntryForApiContract> GetList(DateTime? before = null,
 			int? userId = null,
			EntryEditEvent? editEvent = null, 
			int maxResults = defaultMax, 
			ActivityEntryOptionalFields fields = ActivityEntryOptionalFields.None,
			EntryOptionalFields entryFields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			bool ssl = WebHelper.IsSSL(Request);

			return repository.HandleQuery(ctx => {
				
				var query = ctx.Query<ActivityEntry>();

				if (before.HasValue) {
					query = query.Where(a => a.CreateDate < before.Value);
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
						fields.HasFlag(ActivityEntryOptionalFields.Entry) ? EntryForApiContract.Create(a.EntryBase, lang, entryThumbPersister, entryImagePersisterOld, ssl, entryFields) : null, 
						userIconFactory, permissionContext, fields))
					.ToArray();

				return activityEntries;

			});

		}

	}

}