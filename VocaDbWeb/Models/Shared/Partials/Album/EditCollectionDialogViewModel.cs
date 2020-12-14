#nullable disable

using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.Shared.Partials.Album
{
	public class EditCollectionDialogViewModel
	{
		public EditCollectionDialogViewModel(PurchaseStatus purchaseStatus, MediaType mediaType)
		{
			PurchaseStatus = purchaseStatus;
			MediaType = mediaType;
		}

		public PurchaseStatus PurchaseStatus { get; set; }

		public MediaType MediaType { get; set; }
	}
}