using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumCommentMap : CommentMap<AlbumComment, Album> {

		public AlbumCommentMap() {

			Map(m => m.AuthorName).Length(100).Not.Nullable();

			References(m => m.EntryForComment).Column("[Album]").Not.Nullable();

		}

	}

}
