using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.Search.Tags
{

	public class TagQueryParams
	{

		public TagQueryParams() { }

		public TagQueryParams(CommonSearchParams common, PagingProperties paging)
		{
			Common = common;
			Paging = paging;
		}

		public bool AllowChildren { get; set; }

		public string CategoryName { get; set; }

		public CommonSearchParams Common { get; set; }

		public ContentLanguagePreference LanguagePreference { get; set; }

		public PagingProperties Paging { get; set; }

		public TagSortRule SortRule { get; set; } = TagSortRule.Name;

		public TagTargetTypes Target { get; set; } = TagTargetTypes.All;

	}

}
