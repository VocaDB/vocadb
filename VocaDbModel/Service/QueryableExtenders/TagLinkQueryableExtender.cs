using System;
using System.Linq;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class TagLinkQueryableExtender {

		public static IQueryable<T> WhereTagHasTarget<T>(this IQueryable<T> query, TagTargetTypes target) where T : ITagLink {

			if (target == TagTargetTypes.All)
				return query;

			var types = Enum.GetValues(typeof(TagTargetTypes)).Cast<TagTargetTypes>().Where(t => t.HasFlag(target));
			return query.Where(t => types.Contains(t.Tag.Targets));

		}

	}

}
