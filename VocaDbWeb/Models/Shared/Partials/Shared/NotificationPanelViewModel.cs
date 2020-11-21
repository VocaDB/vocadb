namespace VocaDb.Web.Models.Shared.Partials.Shared {

	public class NotificationPanelViewModel {

		public NotificationPanelViewModel(string message, string messageId = "") {
			Message = message;
			MessageId = messageId;
		}

		public string Message { get; set; }

		public string MessageId { get; set; }

	}

}