#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Data contract for <see cref="User"/> with email.
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary> 
	[DataContract(Namespace = Schemas.VocaDb, Name = "UserWithEmailContract")]
	public class ServerOnlyUserWithEmailContract : UserBaseContract, IUserWithEmail
	{
		public ServerOnlyUserWithEmailContract() { }

		public ServerOnlyUserWithEmailContract(User user)
			: base(user)
		{
			Email = user.Email;
		}

		[DataMember]
		public string Email { get; init; }
	}
}
