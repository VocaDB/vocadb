using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Artists
{

	public class ArtistComment : GenericComment<Artist>
	{

		public ArtistComment() { }

		public ArtistComment(Artist artist, string message, AgentLoginData loginData)
			: base(artist, message, loginData) { }

		public override void OnDelete()
		{
			EntryForComment.Comments.Remove(this);
		}

	}

}
