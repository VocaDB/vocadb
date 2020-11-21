namespace VocaDb.Web.Models.Shared.Partials.EntryDetails {

	public class ReportEntryPopupKnockoutViewModel {

		public ReportEntryPopupKnockoutViewModel(string viewModelBindingName = "reportViewModel", string reportButtonId = "reportEntryLink") {
			ViewModelBindingName = viewModelBindingName;
			ReportButtonId = reportButtonId;
		}

		public string ViewModelBindingName { get; set; }

		public string ReportButtonId { get; set; }

	}

}