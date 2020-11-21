namespace VocaDb.Web.Models.Shared.Partials.EntryDetails {

	public class EntryDeletePopupViewModel {

		public EntryDeletePopupViewModel(string confirmText, string viewModelBindingName = "deleteViewModel", string deleteButtonId = "deleteLink") {
			ConfirmText = confirmText;
			ViewModelBindingName = viewModelBindingName;
			DeleteButtonId = deleteButtonId;
		}

		public string ConfirmText { get; set; }

		public string ViewModelBindingName { get; set; }

		public string DeleteButtonId { get; set; }

	}

}