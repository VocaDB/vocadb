using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users;

[DataContract]
public sealed record EntryEditDataContract
{
	[DataMember]
	public DateTime Time { get; set; }

	[DataMember]
	public int UserId { get; private set; }

	[DataMember]
	public string UserName { get; private set; }

#nullable disable
	public EntryEditDataContract() { }
#nullable enable

	public EntryEditDataContract(IUser user)
		: this()
	{
		UserId = user.Id;
		UserName = user.Name;
		Time = DateTime.Now;
	}

	public void Refresh(IUser user)
	{
		if (user.Id == UserId)
			Time = DateTime.Now;
	}
}
