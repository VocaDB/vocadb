using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserMessagesContract {

		public UserMessagesContract() {}

		public UserMessagesContract(User user, int maxCount, bool unread, IUserIconFactory iconFactory) {

			ReceivedMessages = user.ReceivedMessages.Where(m => !unread || !m.Read).Take(maxCount).Select(m => new UserMessageContract(m, iconFactory)).ToArray();

			if (!unread)
				SentMessages = user.SentMessages.Take(maxCount).Select(m => new UserMessageContract(m, iconFactory)).ToArray();		

		}

		[DataMember]
		public UserMessageContract[] ReceivedMessages { get; set; }

		[DataMember]
		public UserMessageContract[] SentMessages { get; set; }

	}
}
