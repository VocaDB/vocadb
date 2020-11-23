namespace VocaDb.Web.Models.Shared.Partials.EntryDetails
{
	public class EntryDeletePopupBaseViewModel
	{
		public EntryDeletePopupBaseViewModel(string confirmText, string viewModelBindingName, string deleteButtonId, string title, string deleteButtonText)
		{
			ConfirmText = confirmText;
			ViewModelBindingName = viewModelBindingName;
			DeleteButtonId = deleteButtonId;
			Title = title;
			DeleteButtonText = deleteButtonText;
		}

		public string ConfirmText { get; set; }

		public string ViewModelBindingName { get; set; }

		public string DeleteButtonId { get; set; }

		public string Title { get; set; }

		public string DeleteButtonText { get; set; }
	}
}