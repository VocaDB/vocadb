namespace VocaDb.Web.Models.Shared.Partials.Shared {

	public class ValidationSummaryDivViewModel {

		public ValidationSummaryDivViewModel(string message) {
			Message = message;
		}

		public string Message { get; set; }

	}

}