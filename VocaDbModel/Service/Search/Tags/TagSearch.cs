using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.Tags
{
	public class TagSearch
	{
		private readonly IDatabaseContext<Tag> dbContext;
		private readonly ContentLanguagePreference languagePreference;

		public TagSearch(IDatabaseContext<Tag> dbContext, ContentLanguagePreference languagePreference)
		{
			this.dbContext = dbContext;
			this.languagePreference = languagePreference;
		}

		private IQueryable<Tag> CreateQuery(TagQueryParams queryParams, string queryText, NameMatchMode nameMatchMode)
		{
			var textQuery = TagSearchTextQuery.Create(queryText, nameMatchMode);

			var query = dbContext.Query()
				.WhereIsDeleted(false)
				.WhereHasName(textQuery)
				.WhereAllowChildren(queryParams.AllowChildren)
				.WhereHasCategoryName(queryParams.CategoryName)
				.WhereHasTarget(queryParams.Target);

			return query;
		}

		public PartialFindResult<Tag> Find(TagQueryParams queryParams, bool onlyMinimalFields)
		{
			var isMoveToTopQuery = queryParams.Common.MoveExactToTop
				&& queryParams.Common.NameMatchMode != NameMatchMode.StartsWith
				&& queryParams.Common.NameMatchMode != NameMatchMode.Exact
				&& queryParams.Paging.Start == 0
				&& !queryParams.Common.TextQuery.IsEmpty;

			if (isMoveToTopQuery)
			{
				return GetTagsMoveExactToTop(queryParams);
			}

			var query = CreateQuery(queryParams, queryParams.Common.Query, queryParams.Common.NameMatchMode);

			var orderedAndPaged = query
				.OrderBy(queryParams.SortRule, languagePreference)
				.Paged(queryParams.Paging);

			Tag[] tags;

			if (onlyMinimalFields)
			{
				tags = orderedAndPaged.Select(t => new Tag
				{
					Id = t.Id,
					CategoryName = t.CategoryName,
					CreateDate = t.CreateDate,
					Status = t.Status,
					Version = t.Version,
					Names = new NameManager<TagName>
					{
						SortNames = {
						English = t.Names.SortNames.English,
						Romaji = t.Names.SortNames.Romaji,
						Japanese = t.Names.SortNames.Japanese,
						DefaultLanguage = t.Names.SortNames.DefaultLanguage
					}
					}
				}).ToArray();
			}
			else
			{
				tags = orderedAndPaged.ToArray();
			}

			var count = 0;

			if (queryParams.Paging.GetTotalCount)
			{
				count = query.Count();
			}

			return PartialFindResult.Create(tags, count);
		}

		/// <summary>
		/// Get tags, searching by exact matches FIRST.
		/// This mode does not support paging.
		/// </summary>
		private PartialFindResult<Tag> GetTagsMoveExactToTop(TagQueryParams queryParams)
		{
			var sortRule = queryParams.SortRule;
			var maxResults = queryParams.Paging.MaxEntries;
			var getCount = queryParams.Paging.GetTotalCount;

			// Exact query contains the "exact" matches.
			// Note: the matched name does not have to be in user's display language, it can be any name.
			// The songs are sorted by user's display language though
			var exactQ = CreateQuery(queryParams, queryParams.Common.Query, NameMatchMode.StartsWith);

			int count;
			int[] ids;
			var exactResults = exactQ
				.OrderBy(sortRule, languagePreference)
				.Select(s => s.Id)
				.Take(maxResults)
				.ToArray();

			if (exactResults.Length >= maxResults)
			{
				ids = exactResults;
				count = getCount ? CreateQuery(queryParams, queryParams.Common.Query, queryParams.Common.NameMatchMode).Count() : 0;
			}
			else
			{
				var directQ = CreateQuery(queryParams, queryParams.Common.Query, queryParams.Common.NameMatchMode);

				var direct = directQ
					.OrderBy(sortRule, languagePreference)
					.Select(s => s.Id)
					.Take(maxResults)
					.ToArray();

				ids = exactResults
					.Concat(direct)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				count = getCount ? directQ.Count() : 0;
			}

			var tags = dbContext
				.LoadMultiple<Tag>(ids)
				.ToArray()
				.OrderByIds(ids);

			return new PartialFindResult<Tag>(tags, count, queryParams.Common.Query);
		}
	}
}
