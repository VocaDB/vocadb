using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistTagVote : GenericTagVote<ArtistTagUsage> {

		public ArtistTagVote() { }
		public ArtistTagVote(ArtistTagUsage usage, User user) : base(usage, user) { }
		
	}

}
