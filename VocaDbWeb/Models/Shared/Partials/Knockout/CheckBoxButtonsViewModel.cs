using System.Collections.Generic;

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{

	public class CheckBoxButtonsViewModel
	{

		public CheckBoxButtonsViewModel(Dictionary<string, string> options, string binding)
		{
			Options = options;
			Binding = binding;
		}

		public Dictionary<string, string> Options { get; set; }

		public string Binding { get; set; }

	}

}