using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;

namespace VocaDb.Model.Database.Queries
{
	public class ActivityEntryQueries
	{
		public class EditsPerDay
		{
			public int Count { get; set; }
			public int Day { get; set; }
			public int Month { get; set; }
			public int Year { get; set; }
		}

		private const int absoluteMax = 500;
		private const int defaultMax = 50;

		private readonly IRepository repository;
		private readonly IUserIconFactory userIconFactory;
		private readonly IUserPermissionContext permissionContext;
		private readonly EntryForApiContractFactory entryForApiContractFactory;

		public ActivityEntryQueries(IRepository repository, IUserIconFactory userIconFactory,
			IUserPermissionContext permissionContext, EntryForApiContractFactory entryForApiContractFactory)
		{
			this.repository = repository;
			this.userIconFactory = userIconFactory;
			this.permissionContext = permissionContext;
			this.entryForApiContractFactory = entryForApiContractFactory;
		}

		public ICollection<Tuple<DateTime, int>> GetEditsPerDay(int? userId, DateTime? cutoff)
		{
			var values = repository.HandleQuery(ctx =>
			{
				var query = ctx.Query<ActivityEntry>();

				if (userId.HasValue)
				{
					query = query.Where(a => a.Author.Id == userId.Value);
				}

				if (cutoff.HasValue)
					query = query.Where(a => a.CreateDate >= cutoff);

				return query
					.OrderBy(a => a.CreateDate.Year)
					.ThenBy(a => a.CreateDate.Month)
					.ThenBy(a => a.CreateDate.Day)
					.GroupBy(a => new
					{
						Year = a.CreateDate.Year,
						Month = a.CreateDate.Month,
						Day = a.CreateDate.Day
					})
					.Select(a => new
					{
						a.Key.Year,
						a.Key.Month,
						a.Key.Day,
						Count = a.Count()
					})
					.ToArray();
			});

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();

			return points;
		}

		public PartialFindResult<ActivityEntryForApiContract> GetList(
			DateTime? before = null,
			DateTime? since = null,
 			int? userId = null,
			EntryEditEvent? editEvent = null,
			int maxResults = defaultMax,
			bool getTotalCount = false,
			ActivityEntryOptionalFields fields = ActivityEntryOptionalFields.None,
			EntryOptionalFields entryFields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default)
		{
			maxResults = Math.Min(maxResults, absoluteMax);

			return repository.HandleQuery(ctx =>
			{
				var query = ctx.Query<ActivityEntry>();

				if (before.HasValue && !since.HasValue)
				{
					query = query.Where(a => a.CreateDate < before.Value);
				}

				if (!before.HasValue && since.HasValue)
				{
					query = query.Where(a => a.CreateDate > since.Value);
				}

				if (before.HasValue && since.HasValue)
				{
					query = query.Where(a => a.CreateDate > since.Value && a.CreateDate < before.Value);
				}

				if (userId.HasValue)
				{
					query = query.Where(a => a.Author.Id == userId.Value);
				}

				if (editEvent.HasValue)
				{
					query = query.Where(a => a.EditEvent == editEvent.Value);
				}

				var activityEntries = query
					.OrderByDescending(a => a.CreateDate)
					.Take(maxResults)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted)
					.Select(a => new ActivityEntryForApiContract(a,
						fields.HasFlag(ActivityEntryOptionalFields.Entry) ? entryForApiContractFactory.Create(a.EntryBase, entryFields, lang) : null,
						userIconFactory, permissionContext, fields))
					.ToArray();

				var count = getTotalCount ? query.Count() : 0;

				return PartialFindResult.Create(activityEntries, count);
			});
		}
	}
}