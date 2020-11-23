using System.Collections.Generic;

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{

	public class DropdownListViewModel
	{

		public DropdownListViewModel(Dictionary<string, string> items, string valueBinding, string cssClass = null, bool required = false)
		{
			Items = items;
			ValueBinding = valueBinding;
			CssClass = cssClass;
			Required = required;
		}

		public Dictionary<string, string> Items { get; set; }

		public string ValueBinding { get; set; }

		public string CssClass { get; set; }

		public bool Required { get; set; }

	}

}