using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Models.User {

	public class Messages {

		public Messages() {
			ReceiverName = string.Empty;
		}

		public Messages(UserBaseContract user, int? selectedMessageId, string receiverName) {

			User = user;
			ReceiverName = receiverName ?? string.Empty;
			SelectedMessageId = selectedMessageId;

		}

		public string ReceiverName { get; set; }

		public int? SelectedMessageId { get; set; }

		public UserBaseContract User { get; set; }

	}

}