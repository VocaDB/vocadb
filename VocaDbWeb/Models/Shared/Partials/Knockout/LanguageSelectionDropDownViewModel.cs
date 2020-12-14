#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class LanguageSelectionDropDownViewModel
	{
		public LanguageSelectionDropDownViewModel(string valueBinding)
		{
			ValueBinding = valueBinding;
		}

		public string ValueBinding { get; set; }
	}
}