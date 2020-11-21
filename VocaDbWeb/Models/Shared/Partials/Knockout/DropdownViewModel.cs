using System.Collections.Generic;

namespace VocaDb.Web.Models.Shared.Partials.Knockout {

	public class DropdownViewModel {

		public DropdownViewModel(Dictionary<string, string> items, string valueBinding, string currentTextBinding) {
			Items = items;
			ValueBinding = valueBinding;
			CurrentTextBinding = currentTextBinding;
		}

		public Dictionary<string, string> Items { get; set; }

		public string ValueBinding { get; set; }

		public string CurrentTextBinding { get; set; }

	}

}