using System;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class TagLinkQueryableExtensions
	{
		public static IOrderedQueryable<T> OrderByName<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference) where T : ITagLink
		{
			switch (languagePreference)
			{
				case ContentLanguagePreference.Japanese:
					return query.OrderBy(e => e.Tag.Names.SortNames.Japanese);
				case ContentLanguagePreference.English:
					return query.OrderBy(e => e.Tag.Names.SortNames.English);
				default:
					return query.OrderBy(e => e.Tag.Names.SortNames.Romaji);
			}
		}

		public static IQueryable<T> WhereTagHasTarget<T>(this IQueryable<T> query, TagTargetTypes target) where T : ITagLink
		{
			if (target == TagTargetTypes.All)
				return query;

			return query.Where(t => (t.Tag.Targets & target) == target);
		}
	}
}
