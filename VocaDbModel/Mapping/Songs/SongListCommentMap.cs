using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs
{
	public class SongListCommentMap : CommentMap<SongListComment, SongList>
	{
		public SongListCommentMap()
		{
			References(m => m.EntryForComment).Column("SongList").Not.Nullable();
		}
	}
}
