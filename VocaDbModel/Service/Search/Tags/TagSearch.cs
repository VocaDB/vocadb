using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.Search.Tags {

	public class TagSearch {

		private readonly IDatabaseContext<Tag> dbContext;
		private readonly ContentLanguagePreference languagePreference;

		public TagSearch(IDatabaseContext<Tag> dbContext, ContentLanguagePreference languagePreference) {
			this.dbContext = dbContext;
			this.languagePreference = languagePreference;
		}

		public PartialFindResult<Tag> Find(TagQueryParams queryParams, bool onlyMinimalFields) {

			var textQuery = TagSearchTextQuery.Create(queryParams.Common.Query, queryParams.Common.NameMatchMode);

			var query = dbContext.Query()
				.WhereHasName(textQuery)
				.WhereAllowAliases(queryParams.AllowAliases)
				.WhereHasCategoryName(queryParams.CategoryName);

			var orderedAndPaged = query
				.OrderBy(queryParams.SortRule, languagePreference)
				.Paged(queryParams.Paging);

			Tag[] tags;

			if (onlyMinimalFields) {
				tags = orderedAndPaged.Select(t => new Tag {
					Id = t.Id, CategoryName = t.CategoryName, Status = t.Status, Version = t.Version,
					Names = new NameManager<TagName> { SortNames = {
						English = t.Names.SortNames.English,
						Romaji = t.Names.SortNames.Romaji,
						Japanese = t.Names.SortNames.Japanese,
						DefaultLanguage = t.Names.SortNames.DefaultLanguage
					} }
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
