using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Songs {

	public class SongComment : GenericComment<Song> {

		public SongComment() { }

		public SongComment(Song song, string message, AgentLoginData loginData)
			: base(song, message, loginData) {}

		public override void OnDelete() {
			EntryForComment.Comments.Remove(this);
		}

	}

}
