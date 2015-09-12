using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistComment : GenericComment<Artist> {

		public ArtistComment() { }

		public ArtistComment(Artist artist, string message, User user)
			: base(artist, message, new AgentLoginData(user)) {}

		public override void OnDelete() {
			EntryForComment.Comments.Remove(this);
		}

	}

}
