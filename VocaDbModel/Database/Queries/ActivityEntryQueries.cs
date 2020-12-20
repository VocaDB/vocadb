#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;

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

		private const int AbsoluteMax = 500;
		private const int DefaultMax = 50;

		private readonly IRepository _repository;
		private readonly IUserIconFactory _userIconFactory;
		private readonly IUserPermissionContext _permissionContext;
		private readonly EntryForApiContractFactory _entryForApiContractFactory;

		public ActivityEntryQueries(IRepository repository, IUserIconFactory userIconFactory,
			IUserPermissionContext permissionContext, EntryForApiContractFactory entryForApiContractFactory)
		{
			_repository = repository;
			_userIconFactory = userIconFactory;
			_permissionContext = permissionContext;
			_entryForApiContractFactory = entryForApiContractFactory;
		}

		public ICollection<Tuple<DateTime, int>> GetEditsPerDay(int? userId, DateTime? cutoff)
		{
			var values = _repository.HandleQuery(ctx =>
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
			EntryType entryType = EntryType.Undefined,
			int maxResults = DefaultMax,
			bool getTotalCount = false,
			ActivityEntryOptionalFields fields = ActivityEntryOptionalFields.None,
			EntryOptionalFields entryFields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default,
			ActivityEntrySortRule sortRule = ActivityEntrySortRule.CreateDateDescending)
		{
			maxResults = Math.Min(maxResults, AbsoluteMax);

			return _repository.HandleQuery(ctx =>
			{
				var query = ctx.Query<ActivityEntry>();

				if (before.HasValue && !since.HasValue)
					query = query.Where(a => a.CreateDate < before.Value);

				if (!before.HasValue && since.HasValue)
					query = query.Where(a => a.CreateDate > since.Value);

				if (before.HasValue && since.HasValue)
					query = query.Where(a => a.CreateDate > since.Value && a.CreateDate < before.Value);

				if (userId.HasValue)
					query = query.Where(a => a.Author.Id == userId.Value);

				if (editEvent.HasValue)
					query = query.Where(a => a.EditEvent == editEvent.Value);

				if (entryType != EntryType.Undefined)
					query = query.Where(a => a.EntryType == entryType);

				var activityEntries = query
					.OrderBy(sortRule)
					.Take(maxResults)
					.ToArray()
					.Where(a => !a.EntryBase.Deleted)
					.Select(a => new ActivityEntryForApiContract(a,
						fields.HasFlag(ActivityEntryOptionalFields.Entry) ? _entryForApiContractFactory.Create(a.EntryBase, entryFields, lang) : null,
						_userIconFactory, _permissionContext, fields))
					.ToArray();

				var count = getTotalCount ? query.Count() : 0;

				return PartialFindResult.Create(activityEntries, count);
			});
		}
	}
}