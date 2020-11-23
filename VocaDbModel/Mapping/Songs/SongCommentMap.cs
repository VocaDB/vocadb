using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs
{

	public class SongCommentMap : CommentMap<SongComment, Song>
	{

		public SongCommentMap()
		{

			References(m => m.EntryForComment).Column("[Song]").Not.Nullable();

		}

	}

}
