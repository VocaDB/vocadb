#nullable disable

using System.Linq;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class RelatedSongsContract
	{
		public bool Any => ArtistMatches.Any() || LikeMatches.Any() || TagMatches.Any();

		[DataMember]
		public SongForApiContract[] ArtistMatches { get; set; }

		[DataMember]
		public SongForApiContract[] LikeMatches { get; set; }

		[DataMember]
		public SongForApiContract[] TagMatches { get; set; }
	}
}