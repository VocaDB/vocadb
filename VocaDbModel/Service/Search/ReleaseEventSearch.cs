using System;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Service.Search {

	public class ReleaseEventSearch {

		private static readonly Regex eventNameRegex = new Regex(@"([^\d]+)(\d+)(?:\s(\w+))?");
		private static readonly Regex eventNumberRegex = new Regex(@"^(\d+)(?:\s(\w+))?");

		private readonly IDatabaseContext querySource;

		private IQueryable<T> Query<T>() {
			return querySource.Query<T>();
		}

		public ReleaseEventSearch(IDatabaseContext querySource) {
			this.querySource = querySource;
		}

		private ReleaseEventFindResultContract AttemptSeriesMatch(string seriesName, ReleaseEventSeries series, string query) {
			
			var queryWithoutSeries = query.Remove(0, seriesName.Length).TrimStart();
			var match = eventNumberRegex.Match(queryWithoutSeries);

			if (!match.Success)
				return null;

			var seriesNumber = Convert.ToInt32(match.Groups[1].Value);
			var seriesSuffix = (match.Groups.Count >= 3 ? match.Groups[2].Value : string.Empty);

			var ev = Query<ReleaseEvent>().FirstOrDefault(e => e.Series != null && e.Series.Id == series.Id && e.SeriesNumber == seriesNumber && e.SeriesSuffix == seriesSuffix);

			if (ev != null) {
				return new ReleaseEventFindResultContract(ev);
			} else {
				return new ReleaseEventFindResultContract(series, seriesNumber, seriesSuffix, query);
			}

		}

		public ReleaseEventFindResultContract Find(string query) {

			if (string.IsNullOrEmpty(query))
				return new ReleaseEventFindResultContract();

			query = query.Trim();

			// Attempt to match exact name
			var ev = Query<ReleaseEvent>().FirstOrDefault(e => e.Name == query);

			if (ev != null)
				return new ReleaseEventFindResultContract(ev);

			var startsWithMatches = Query<ReleaseEventSeries>()
				.Where(s => query.StartsWith(s.Name) || s.Aliases.Any(a => query.StartsWith(a.Name)))
				.ToArray();

			foreach (var startsWithMatch in startsWithMatches) {

				if (query.StartsWith(startsWithMatch.Name, StringComparison.InvariantCultureIgnoreCase)) {
					
					var result = AttemptSeriesMatch(startsWithMatch.Name, startsWithMatch, query);

					if (result != null)
						return result;

				}

				foreach (var alias in startsWithMatch.Aliases.Where(a => query.StartsWith(a.Name, StringComparison.InvariantCultureIgnoreCase))) {
					
					var result = AttemptSeriesMatch(alias.Name, startsWithMatch, query);

					if (result != null)
						return result;

				}

			}

			var match = eventNameRegex.Match(query);

			if (match.Success) {

				var seriesName = match.Groups[1].Value.Trim();
				var seriesNumber = Convert.ToInt32(match.Groups[2].Value);
				var seriesSuffix = (match.Groups.Count >= 4 ? match.Groups[3].Value : string.Empty);

				// Attempt to match series + series number
				var results = Query<ReleaseEvent>()
					.Where(e => e.SeriesNumber == seriesNumber 
						&& e.SeriesSuffix == seriesSuffix 
						&& (seriesName.StartsWith(e.Series.Name) || e.Series.Name.Contains(seriesName)
							|| e.Series.Aliases.Any(a => seriesName.StartsWith(a.Name) || a.Name.Contains(seriesName)))).ToArray();

				if (results.Length > 1)
					return new ReleaseEventFindResultContract();

				if (results.Length == 1)
					return new ReleaseEventFindResultContract(results[0]);

				// Attempt to match just the series
				var series = Query<ReleaseEventSeries>().FirstOrDefault(s => seriesName.StartsWith(s.Name) || s.Name.Contains(seriesName) || s.Aliases.Any(a => seriesName.StartsWith(a.Name) || a.Name.Contains(seriesName)));

				if (series != null)
					return new ReleaseEventFindResultContract(series, seriesNumber, seriesSuffix, query);

			}

			var events = Query<ReleaseEvent>().Where(e => query.Contains(e.Name) || e.Name.Contains(query)).Take(2).ToArray();

			if (events.Length != 1) {
				return new ReleaseEventFindResultContract(query);
			}

			return new ReleaseEventFindResultContract(events[0]);

		}

	}

}
