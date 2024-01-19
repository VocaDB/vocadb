using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.DataContracts.Users;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record UserRewindForApiContract
{
	[DataMember]
	public required string AccountName { get; set;  }
	
	[DataMember]
	public required int UserRank { get; set;  }
	
	[DataMember]
	public required double UserRankPercentage { get; set;  }
	
	[DataMember]
	public required SongHitOnDay[] SongHitsOnDays { get; set; }
	
	[DataMember]
	public required SongForApiContract[] FavoriteSongs { get; set; }
	
	[DataMember]
	public required SongForApiContract[] PopularSongs { get; set; }

	[DataMember]
	public required object FavoriteGenreTags { get; set; }
	
	[DataMember]
	public required object FavoriteSubjectiveTags { get; set; }
	
	[DataMember]
	public required object FavoriteVoicebanks { get; set; }
	
	[DataMember]
	public required object FavoriteProducers { get; set; }
	
	[DataMember]
	public required object FollowedArtists { get; set; }
}

public sealed record SongHitOnDay
{
	[DataMember]
	public DateTime Date { get; set; }
	
	[DataMember]
	public int Count { get; set; }
}