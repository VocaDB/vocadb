using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Queries;

namespace VocaDb.Model.DataContracts.Aggregate
{
	public static class CountPerDayContractExtender
	{
		public static IEnumerable<TResult> CumulativeSelect<TSource, TResult>(this IEnumerable<TSource> sequence, Func<TSource, TResult, TResult> func)
		{
			var previous = default(TResult);
			foreach (var item in sequence)
			{
				var res = func(item, previous);
				previous = res;
				yield return res;
			}
		}

		public static Tuple<DateTime, int>[] CumulativeSum(this IEnumerable<CountPerDayContract> sequence)
		{
			return sequence.CumulativeSelect<CountPerDayContract, Tuple<DateTime, int>>((v, previous) =>
			{
				return Tuple.Create(v.ToDateTime(), (previous?.Item2 ?? 0) + v.Count);
			}).ToArray();
		}

		public static IEnumerable<CountPerDayContract> CumulativeSumContract(this IEnumerable<CountPerDayContract> sequence)
		{
			return sequence.CumulativeSelect<CountPerDayContract, CountPerDayContract>((v, previous) =>
			{
				return new CountPerDayContract(v.ToDateTime(), (previous?.Count ?? 0) + v.Count);
			});
		}

		public static Tuple<DateTime, int>[] ToDatePoints(this IEnumerable<CountPerDayContract> sequence)
		{
			return sequence.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();
		}

		/// <summary>
		/// Generates dates between a range
		/// </summary>
		private static IEnumerable<DateTime> DateGenerator(DateTime start, DateTime end, TimeUnit timeUnit)
		{
			var current = start;
			while (current <= end)
			{
				yield return current;

				if (timeUnit == TimeUnit.Month)
					current = current.AddMonths(1);
				else
					current = current.AddDays(1);
			}
		}

		// https://stackoverflow.com/a/3683217
		public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> source,
			Func<TSource, TResult, TResult> projection)
		{
			using (var iterator = source.GetEnumerator())
			{
				/*if (!iterator.MoveNext()) {
					yield break;
				}
				var previous = projection(iterator.Current, default(TResult));
				yield return previous;*/
				var previous = default(TResult);
				while (iterator.MoveNext())
				{
					previous = projection(iterator.Current, previous);
					yield return previous;
				}
			}
		}

		private static CountPerDayContract GetCountOrZero(Dictionary<DateTime, CountPerDayContract> dict, DateTime date, CountPerDayContract prev)
		{
			return dict.ContainsKey(date) ? dict[date] : new CountPerDayContract(date, 0);
		}

		private static CountPerDayContract GetCountOrPrevious(Dictionary<DateTime, CountPerDayContract> dict, DateTime date, CountPerDayContract prev)
		{
			return dict.ContainsKey(date) ? dict[date] : new CountPerDayContract(date, prev.Count);
		}

		private static CountPerDayContract[] FillValues(this CountPerDayContract[] query, DateTime? endDate, bool addZeros, TimeUnit timeUnit, Func<Dictionary<DateTime, CountPerDayContract>, DateTime, CountPerDayContract, CountPerDayContract> func)
		{
			if (!addZeros || !query.Any())
				return query;

			var dict = query.ToDictionary(t => new DateTime(t.Year, t.Month, t.Day));
			var end = endDate ?? dict.Last().Key;

			return DateGenerator(dict.First().Key, end, timeUnit)
				.SelectWithPrevious<DateTime, CountPerDayContract>((d, prev) => func(dict, d, prev))
				.ToArray();
		}

		/// <summary>
		/// Makes sure that a value is generated for every day, inserting zero for days without value.
		/// </summary>
		public static CountPerDayContract[] AddZeros(this CountPerDayContract[] query, bool addZeros, TimeUnit timeUnit)
		{
			return FillValues(query, null, addZeros, timeUnit, GetCountOrZero);
		}

		/// <summary>
		/// Makes sure that a value is generated for every day, inserting previous value for days without value.
		/// </summary>
		public static CountPerDayContract[] AddPreviousValues(this CountPerDayContract[] query, bool addZeros, TimeUnit timeUnit, DateTime? endDate = null)
		{
			return FillValues(query, endDate, addZeros, timeUnit, GetCountOrPrevious);
		}
	}
}
