using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Web.Models.User {

	public class Messages {

		public Messages() {
			ReceiverName = string.Empty;
		}

		public Messages(UserBaseContract user, int? selectedMessageId, string receiverName, UserInboxType inbox) {

			User = user;
			ReceiverName = receiverName ?? string.Empty;
			SelectedMessageId = selectedMessageId;
			Inbox = inbox;

		}

		public UserInboxType Inbox { get; set; }

		public string ReceiverName { get; set; }

		public int? SelectedMessageId { get; set; }

		public UserBaseContract User { get; set; }

	}

}