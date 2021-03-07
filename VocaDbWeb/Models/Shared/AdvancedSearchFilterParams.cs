using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Service.Search;

namespace VocaDb.Web.Models.Shared
{
	public sealed record AdvancedSearchFilterParams
	{
		[FromQuery(Name = "[filterType]")]
		public AdvancedFilterType FilterType { get; init; }

		/// <summary>
		/// Whether to negate the filter (entry is included only if the filter does not match). 
		/// Not supported by all filters.
		/// </summary>
		[FromQuery(Name = "[negate]")]
		public bool Negate { get; init; }

		/// <summary>
		/// Filter parameter, for example artist type to filter by.
		/// </summary>
		[FromQuery(Name = "[param]")]
		public string? Param { get; init; }

		public AdvancedSearchFilter ToAdvancedSearchFilter() => new()
		{
			FilterType = FilterType,
			Negate = Negate,
			Param = Param,
		};
	}
}
