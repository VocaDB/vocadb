using System;
using System.Collections.Generic;
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

		public CountPerDayContract[] AddZeros(CountPerDayContract[] query, bool addZeros, TimeUnit timeUnit) {
			return query.AddZeros(addZeros, timeUnit);
		}

		private CountPerDayContract[] SongsPerDay(IDatabaseContext ctx, Expression<Func<Song, bool>> where, DateTime? after = null) {

			var query = ctx.Query<Song>()
				.Where(a => !a.Deleted && a.PublishDate.DateTime != null);

			if (where != null)
				query = query.Where(where);

			if (after != null)
				query = query.Where(s => s.PublishDate.DateTime >= after);

			return query
				.OrderBy(a => a.PublishDate.DateTime.Value.Year) // Need to order by part of publish date because we're grouping
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

		private CountPerDayContract[] SongsPerMonth(IDatabaseContext ctx, Expression<Func<Song, bool>> where, DateTime? after = null) {

			var query = ctx.Query<Song>()
				.Where(a => !a.Deleted && a.PublishDate.DateTime != null);

			if (where != null)
				query = query.Where(where);

			if (after != null)
				query = query.Where(s => s.PublishDate.DateTime >= after);

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

		/// <summary>
		/// Get number of songs published per month or per day.
		/// </summary>
		/// <param name="timeUnit">Time unit, whether to get per month or per day.</param>
		/// <param name="addZeros">
		/// Whether to add zeros for missing values. This is needed by highcharts when drawing area charts for example.
		/// </param>
		/// <param name="filters">Additional filters for songs. One result will be produced per filter. Cannot be null.</param>
		/// <returns>Results. One result per filter.</returns>
		public CountPerDayContract[][] SongsOverTime(TimeUnit timeUnit, bool addZeros, DateTime? after, params Expression<Func<Song, bool>>[] filters) {

			return repository.HandleQuery(ctx => {

				var results = filters
					.Select(f => AddZeros(timeUnit == TimeUnit.Month ? SongsPerMonth(ctx, f, after) : SongsPerDay(ctx, f, after), addZeros, timeUnit))
					.ToArray();

				return results;

			});

		}

		public CountPerDayContract[] SongsOverTime(TimeUnit timeUnit, bool addZeros, DateTime? after, int artistId, int tagId) {

			Expression<Func<Song, bool>> query = (s => s.PublishDate.DateTime <= DateTime.Now);

			if (artistId != 0) {
				query = query.And(s => s.AllArtists.Any(a => a.Artist.Id == artistId));
			}

			if (tagId != 0) {
				query = query.And(s => s.Tags.Usages.Any(a => a.Tag.Id == tagId));
			}

			return SongsOverTime(timeUnit, addZeros, after, query)[0];

		}

	}

	public enum TimeUnit {
		Month,
		Day
	}

}
