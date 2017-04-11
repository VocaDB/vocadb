using System;
using System.Linq;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class TagLinkQueryableExtender {

		public static IQueryable<T> WhereTagHasTarget<T>(this IQueryable<T> query, TagTargetTypes target) where T : ITagLink {

			if (target == TagTargetTypes.All)
				return query;

			return query.Where(t => (t.Tag.Targets & target) == target);

		}

	}

}
