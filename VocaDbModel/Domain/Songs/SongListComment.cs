using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Songs {

	public class SongListComment : GenericComment<SongList> {

		public SongListComment() { }

		public SongListComment(SongList list, string message, AgentLoginData loginData)
			: base(list, message, loginData) {}

	}

}
