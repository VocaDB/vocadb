using System;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class LocalizedStringQueryableExtensions
	{
		public const int MaxSearchWords = 10;

		public static IQueryable<T> WhereEntryNameIs<T>(this IQueryable<T> query, SearchTextQuery textQuery)
			where T : LocalizedString
		{
			if (textQuery.IsEmpty)
				return query;

			var nameFilter = textQuery.Query;

			switch (textQuery.MatchMode)
			{
				case NameMatchMode.Exact:
					return query.Where(m => m.Value == nameFilter);

				case NameMatchMode.Partial:
					return query.Where(m => m.Value.Contains(nameFilter));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Value.StartsWith(nameFilter));

				case NameMatchMode.Words:
					var words = textQuery.Words;

					foreach (var word in words.Take(MaxSearchWords))
					{
						var temp = word;
						query = query.Where(q => q.Value.Contains(temp));
					}

					return query;
			}

			return query;
		}
	}
}
