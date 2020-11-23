using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists
{

	public class ArtistCommentMap : CommentMap<ArtistComment, Artist>
	{

		public ArtistCommentMap()
		{

			References(m => m.EntryForComment).Column("[Artist]").Not.Nullable();

		}

	}

}
