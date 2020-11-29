using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.Mapping
{
	public class CommentMap : ClassMap<Comment>
	{
		protected CommentMap()
		{
			Table("Comments");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Message).Length(int.MaxValue).Not.Nullable();

			References(m => m.Author).Not.Nullable();
		}
	}

	public class AlbumCommentMap : SubclassMap<AlbumComment>
	{
		public AlbumCommentMap()
		{
			Table("AlbumComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("[Album]").Nullable();
		}
	}

	public class ArtistCommentMap : SubclassMap<ArtistComment>
	{
		public ArtistCommentMap()
		{
			Table("ArtistComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("[Artist]").Nullable();
		}
	}

	public class DiscussionCommentMap : SubclassMap<DiscussionComment>
	{
		public DiscussionCommentMap()
		{
			Schema("discussions");
			Table("DiscussionComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("[Topic]").Nullable();
		}
	}

	public class ReleaseEventCommentMap : SubclassMap<ReleaseEventComment>
	{
		public ReleaseEventCommentMap()
		{
			Table("ReleaseEventComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("[ReleaseEvent]").Nullable();
		}
	}

	public class SongCommentMap : SubclassMap<SongComment>
	{
		public SongCommentMap()
		{
			Table("SongComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("[Song]").Nullable();
		}
	}

	public class SongListCommentMap : SubclassMap<SongListComment>
	{
		public SongListCommentMap()
		{
			Table("SongListComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("SongList").Nullable();
		}
	}

	public class TagCommentMap : SubclassMap<TagComment>
	{
		public TagCommentMap()
		{
			Table("TagComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("[Tag]").Nullable();
		}
	}

	public class UserCommentMap : SubclassMap<UserComment>
	{
		public UserCommentMap()
		{
			Table("UserComments");
			KeyColumn("Comment");

			References(m => m.EntryForComment).Column("[User]").Nullable();
		}
	}

	public class AlbumReviewMap : SubclassMap<AlbumReview>
	{
		public AlbumReviewMap()
		{
			Table("AlbumReviews");
			KeyColumn("Comment");

			Map(m => m.LanguageCode).Not.Nullable().UniqueKey("UX_AlbumReviews");
			Map(m => m.Title).Not.Nullable();

			References(m => m.EntryForComment).Column("Album").Not.Nullable();
		}
	}
}
