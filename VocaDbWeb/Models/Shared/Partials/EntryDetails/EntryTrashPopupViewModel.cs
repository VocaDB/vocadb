#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.EntryDetails
{
	public class EntryTrashPopupViewModel
	{
		public EntryTrashPopupViewModel(string confirmText, string viewModelBindingName = "trashViewModel", string deleteButtonId = "trashLink")
		{
			ConfirmText = confirmText;
			ViewModelBindingName = viewModelBindingName;
			DeleteButtonId = deleteButtonId;
		}

		public string ConfirmText { get; set; }

		public string ViewModelBindingName { get; set; }

		public string DeleteButtonId { get; set; }
	}
}