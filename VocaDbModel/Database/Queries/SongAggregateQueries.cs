using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Aggregate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Model.Database.Queries {

	public class SongAggregateQueries : QueriesBase<ISongRepository, Song> {

		public SongAggregateQueries(ISongRepository repository, IUserPermissionContext permissionContext) 
			: base(repository, permissionContext) {}

		private CountPerDayContract[] SongsPerDay(IDatabaseContext ctx, Expression<Func<Song, bool>> where) {

			var query = ctx.Query<Song>()
				.Where(a => !a.Deleted && a.PublishDate.DateTime != null);

			if (where != null)
				query = query.Where(where);

			return query
				.OrderBy(a => a.PublishDate.DateTime.Value.Year)
				.ThenBy(a => a.PublishDate.DateTime.Value.Month)
				.ThenBy(a => a.PublishDate.DateTime.Value.Day)
				.GroupBy(a => new {
					Year = a.PublishDate.DateTime.Value.Year,
					Month = a.PublishDate.DateTime.Value.Month,
					Day = a.PublishDate.DateTime.Value.Day,
				})
				.Select(a => new CountPerDayContract {
					Year = a.Key.Year,
					Month = a.Key.Month,
					Day = a.Key.Day,
					Count = a.Count()
				})
				.ToArray();

		}

		private CountPerDayContract[] SongsPerMonth(IDatabaseContext ctx, Expression<Func<Song, bool>> where) {

			var query = ctx.Query<Song>()
				.Where(a => !a.Deleted && a.PublishDate.DateTime != null);

			if (where != null)
				query = query.Where(where);

			return query
				.OrderBy(a => a.PublishDate.DateTime.Value.Year)
				.ThenBy(a => a.PublishDate.DateTime.Value.Month)
				.GroupBy(a => new {
					Year = a.PublishDate.DateTime.Value.Year,
					Month = a.PublishDate.DateTime.Value.Month,
				})
				.Select(a => new CountPerDayContract {
					Year = a.Key.Year,
					Month = a.Key.Month,
					Count = a.Count()
				})
				.ToArray();

		}

		public CountPerDayContract[][] SongsOverTime(TimeUnit timeUnit, params Expression<Func<Song, bool>>[] filters) {

			return repository.HandleQuery(ctx => {

				var results = filters.Select(f => timeUnit == TimeUnit.Month ? SongsPerMonth(ctx, f) : SongsPerDay(ctx, f)).ToArray();
				return results;

			});

		}

	}

	public enum TimeUnit {
		Month,
		Day
	}

}
