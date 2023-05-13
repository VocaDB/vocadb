using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.DataContracts.Api;

[DataContract(Namespace = Schemas.VocaDb)]
public class ActiveEditorForApiContract
{
	public ActiveEditorForApiContract(EntryForApiContract entry, UserForApiContract user, DateTime time)
	{
		Time = time;
		User = user;
		Entry = entry;
	}

	[DataMember]
	public DateTime Time { get; init; }

	[DataMember]
	public UserForApiContract User { get; init; }

	[DataMember]
	public EntryForApiContract Entry { get; init; }
}