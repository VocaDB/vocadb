using System.Collections.Generic;

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class EntryValidationMessageViewModel
	{
		public EntryValidationMessageViewModel(bool draft, Dictionary<string, string> validationMessages, string helpSection)
		{
			Draft = draft;
			ValidationMessages = validationMessages;
			HelpSection = helpSection;
		}

		public bool Draft { get; set; }

		public Dictionary<string, string> ValidationMessages { get; set; }

		public string HelpSection { get; set; }
	}
}