using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.Search.Tags {

	public class TagSearch {

		private readonly IDatabaseContext<Tag> dbContext;

		public TagSearch(IDatabaseContext<Tag> dbContext) {
			this.dbContext = dbContext;
		}

		public PartialFindResult<Tag> Find(TagQueryParams queryParams, bool onlyMinimalFields) {

			var textQuery = TagSearchTextQuery.Create(queryParams.Common.Query, queryParams.Common.NameMatchMode);

			var query = dbContext.Query()
				.WhereHasName(textQuery)
				.WhereAllowAliases(queryParams.AllowAliases)
				.WhereHasCategoryName(queryParams.CategoryName);

			var orderedAndPaged = query
				.OrderBy(queryParams.SortRule)
				.Paged(queryParams.Paging);

			Tag[] tags;

			if (onlyMinimalFields) {
				tags = orderedAndPaged.Select(t => new Tag {
					Id = t.Id, EnglishName = t.EnglishName, CategoryName = t.CategoryName, Status = t.Status, Version = t.Version
				}).ToArray();
			} else {
				tags = orderedAndPaged.ToArray();
			}

			var count = 0;

			if (queryParams.Paging.GetTotalCount) {

				count = query.Count();

			}

			return PartialFindResult.Create(tags, count);

		}

	}
}
