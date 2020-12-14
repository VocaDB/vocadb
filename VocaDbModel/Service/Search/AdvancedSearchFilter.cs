#nullable disable

namespace VocaDb.Model.Service.Search
{
	/// <summary>
	/// Advanced search filter.
	/// </summary>
	public class AdvancedSearchFilter
	{
		public const string Any = "*";

		public AdvancedFilterType FilterType { get; set; }

		/// <summary>
		/// Whether to negate the filter (entry is included only if the filter does not match). 
		/// Not supported by all filters.
		/// </summary>
		public bool Negate { get; set; }

		/// <summary>
		/// Filter parameter, for example artist type to filter by.
		/// </summary>
		public string Param { get; set; }
	}
}
