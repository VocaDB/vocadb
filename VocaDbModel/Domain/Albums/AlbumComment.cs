using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Albums
{

	public class AlbumComment : GenericComment<Album>
	{

		public AlbumComment() { }

		public AlbumComment(Album album, string message, AgentLoginData loginData)
			: base(album, message, loginData) { }

		public override void OnDelete()
		{
			EntryForComment.Comments.Remove(this);
		}

	}
}
