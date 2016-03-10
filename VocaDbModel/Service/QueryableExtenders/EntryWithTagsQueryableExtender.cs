using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class EntryWithTagsQueryableExtender {

		public static IQueryable<TEntry> WhereHasTag<TEntry, TTagLink>(this IQueryable<TEntry> query, string tagName) 
			where TEntry : IEntryWithTags<TTagLink> where TTagLink : TagUsage {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Tags.Usages.Any(t => t.Tag.Names.Names.Any(n => n.Value == tagName)));

		}

		public static IQueryable<TEntry> WhereHasTags<TEntry, TTagLink>(this IQueryable<TEntry> query, string[] tagNames) 
			where TEntry : IEntryWithTags<TTagLink> where TTagLink : TagUsage {

			if (tagNames == null || !tagNames.Any())
				return query;

			return tagNames.Aggregate(query, WhereHasTag<TEntry, TTagLink>);

		}

		public static IQueryable<TEntry> WhereHasTag<TEntry, TTagLink>(this IQueryable<TEntry> query, int tagId)
			where TEntry : IEntryWithTags<TTagLink> where TTagLink : TagUsage {

			if (tagId == 0)
				return query;

			return query.Where(s => s.Tags.Usages.Any(a => a.Tag.Id == tagId));

		}

		/// <summary>
		/// Filter query by one or more tags.
		/// </summary>
		/// <typeparam name="TEntry">Type of query to be filtered.</typeparam>
		/// <typeparam name="TTagLink">Type of tag link.</typeparam>
		/// <param name="query">Query to be filtered. Cannot be null.</param>
		/// <param name="tagIds">List of tag IDs to filter by. All tags need to be present. If null or empty no filtering is done.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<TEntry> WhereHasTags<TEntry, TTagLink>(this IQueryable<TEntry> query, int[] tagIds)
			where TEntry : IEntryWithTags<TTagLink> where TTagLink : TagUsage {

			if (tagIds == null || !tagIds.Any())
				return query;

			return tagIds.Aggregate(query, WhereHasTag<TEntry, TTagLink>);

		}

	}

}
