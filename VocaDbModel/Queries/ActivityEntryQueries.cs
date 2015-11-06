using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Queries {

	public class ActivityEntryQueries {

		public class EditsPerDay {
			public int Count { get; set; }
			public int Day { get; set; }
			public int Month { get; set; }
			public int Year { get; set; }
		}

		private readonly IRepository repository;

		public ActivityEntryQueries(IRepository repository) {
			this.repository = repository;
		}

		public ICollection<Tuple<DateTime, int>> GetEditsPerDay(int? userId, DateTime? cutoff) {
			
			var values = repository.HandleQuery(ctx => {

				var query = ctx.Query<ActivityEntry>();

				if (userId.HasValue) {
					query = query.Where(a => a.Author.Id == userId.Value);
				}

				if (cutoff.HasValue)
					query = query.Where(a => a.CreateDate >= cutoff);

				return query
					.OrderBy(a => a.CreateDate.Year)
					.ThenBy(a => a.CreateDate.Month)
					.ThenBy(a => a.CreateDate.Day)
					.GroupBy(a => new {
						Year = a.CreateDate.Year, 
						Month = a.CreateDate.Month,
						Day = a.CreateDate.Day
					})
					.Select(a => new {
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

	}

}