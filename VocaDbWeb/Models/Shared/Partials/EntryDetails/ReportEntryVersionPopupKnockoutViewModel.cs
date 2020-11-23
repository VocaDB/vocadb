namespace VocaDb.Web.Models.Shared.Partials.EntryDetails
{
	public class ReportEntryVersionPopupKnockoutViewModel
	{
		public ReportEntryVersionPopupKnockoutViewModel(string viewModelBindingName = "reportViewModel", string reportButtonId = "reportEntryLink")
		{
			ViewModelBindingName = viewModelBindingName;
			ReportButtonId = reportButtonId;
		}

		public string ViewModelBindingName { get; set; }

		public string ReportButtonId { get; set; }
	}
}