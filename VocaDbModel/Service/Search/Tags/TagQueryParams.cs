using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.Tags
{
	public sealed record TagQueryParams
	{
		public TagQueryParams() { }

		public TagQueryParams(CommonSearchParams common, PagingProperties paging)
		{
			Common = common;
			Paging = paging;
		}

		public bool AllowChildren { get; init; }

		public string? CategoryName { get; init; }

		public CommonSearchParams Common { get; init; } = CommonSearchParams.Default;

		public ContentLanguagePreference LanguagePreference { get; init; }

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public TagSortRule SortRule { get; init; } = TagSortRule.Name;

		public TagTargetTypes Target { get; init; } = TagTargetTypes.All;
	}
}
