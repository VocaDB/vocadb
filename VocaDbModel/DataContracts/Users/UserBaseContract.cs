#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Data contract for <see cref="User"/> with minimal information.
	/// Contains no sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserBaseContract : IUser
	{
		public UserBaseContract() { }

#nullable enable
		public UserBaseContract(IUser user)
		{
			ParamIs.NotNull(() => user);

			Id = user.Id;
			Name = user.Name;
		}
#nullable disable

		public UserBaseContract(IUser user, string fallbackName)
		{
			if (user != null)
			{
				Id = user.Id;
				Name = user.Name;
			}
			else
			{
				Name = fallbackName;
			}
		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

#nullable enable
		public override string ToString()
		{
			return $"User contract '{Name}' [{Id}]";
		}
#nullable disable
	}
}
