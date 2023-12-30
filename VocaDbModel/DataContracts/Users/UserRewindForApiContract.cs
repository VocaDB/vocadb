using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Users;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record UserRewindForApiContract
{
		[DataMember]
		public SongHitOnDay[] SongHitsOnDays { get; set; }
}

public sealed record SongHitOnDay
{
	[DataMember]
	public DateTime Date { get; set; }
	
	[DataMember]
	public int Count { get; set; }
}