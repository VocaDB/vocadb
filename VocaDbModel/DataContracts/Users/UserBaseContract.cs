using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// Data contract for <see cref="User"/> with minimal information.
	/// Contains no sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserBaseContract : IUser {

		public UserBaseContract() { }

		public UserBaseContract(User user) {

			ParamIs.NotNull(() => user);

			Id = user.Id;
			Name = user.Name;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		public override string ToString() {
			return string.Format("User contract '{0}' [{1}]", Name, Id);
		}

	}
}
