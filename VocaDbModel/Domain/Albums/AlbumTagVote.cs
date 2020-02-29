using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumTagVote : GenericTagVote<AlbumTagUsage> {

		public AlbumTagVote() { }
		public AlbumTagVote(AlbumTagUsage usage, User user) : base(usage, user) { }

	}
}
