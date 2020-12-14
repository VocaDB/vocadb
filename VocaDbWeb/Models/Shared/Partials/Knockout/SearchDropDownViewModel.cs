#nullable disable

using System.Collections.Generic;

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class SearchDropDownViewModel
	{
		public SearchDropDownViewModel(string visibleBinding, string viewModel, Dictionary<string, string> sortRules)
		{
			VisibleBinding = visibleBinding;
			ViewModel = viewModel;
			SortRules = sortRules;
		}

		public string VisibleBinding { get; set; }

		public string ViewModel { get; set; }

		public Dictionary<string, string> SortRules { get; set; }
	}
}