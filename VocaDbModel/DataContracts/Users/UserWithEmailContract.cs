using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// Data contract for <see cref="User"/> with email.
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary> 
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserWithEmailContract : UserBaseContract, IUserWithEmail {

		public UserWithEmailContract() {}

		public UserWithEmailContract(User user)
			: base(user) {

			this.Email = user.Email;

		}

		[DataMember]
		public string Email { get; set; }

	}

}
