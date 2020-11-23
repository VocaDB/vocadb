using System;
using System.Collections.Generic;
using VocaDb.Model.Service.Search;

namespace VocaDb.Web.Models.Shared.Partials.Search
{
	public class AdvancedFiltersViewModel
	{
		public AdvancedFiltersViewModel(IEnumerable<Tuple<string, AdvancedFilterType, string, bool>> filters)
		{
			Filters = filters;
		}

		public IEnumerable<Tuple<string, AdvancedFilterType, string, bool>> Filters { get; set; }
	}
}