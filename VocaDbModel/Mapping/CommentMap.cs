using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.Mapping
{
	public class CommentMap : ClassMap<Comment>
	{
		protected CommentMap()
		{
			DiscriminateSubClassesOnColumn("[EntryType]");
			Table("Comments");
			Id(m => m.Id);
			Cache.ReadWrite();

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
			DiscriminatorValue(nameof(AlbumComment));

			References(m => m.EntryForComment).Column("[Album]").Nullable();
		}
	}

	public class ArtistCommentMap : SubclassMap<ArtistComment>
	{
		public ArtistCommentMap()
		{
			DiscriminatorValue(nameof(ArtistComment));

			References(m => m.EntryForComment).Column("[Artist]").Nullable();
		}
	}

	public class DiscussionCommentMap : SubclassMap<DiscussionComment>
	{
		public DiscussionCommentMap()
		{
			DiscriminatorValue(nameof(DiscussionComment));

			References(m => m.EntryForComment).Column("[Topic]").Nullable();
		}
	}

	public class ReleaseEventCommentMap : SubclassMap<ReleaseEventComment>
	{
		public ReleaseEventCommentMap()
		{
			DiscriminatorValue(nameof(ReleaseEventComment));

			References(m => m.EntryForComment).Column("[ReleaseEvent]").Nullable();
		}
	}

	public class SongCommentMap : SubclassMap<SongComment>
	{
		public SongCommentMap()
		{
			DiscriminatorValue(nameof(SongComment));

			References(m => m.EntryForComment).Column("[Song]").Nullable();
		}
	}

	public class SongListCommentMap : SubclassMap<SongListComment>
	{
		public SongListCommentMap()
		{
			DiscriminatorValue(nameof(SongListComment));

			References(m => m.EntryForComment).Column("SongList").Nullable();
		}
	}

	public class TagCommentMap : SubclassMap<TagComment>
	{
		public TagCommentMap()
		{
			DiscriminatorValue(nameof(TagComment));

			References(m => m.EntryForComment).Column("[Tag]").Nullable();
		}
	}

	public class UserCommentMap : SubclassMap<UserComment>
	{
		public UserCommentMap()
		{
			DiscriminatorValue(nameof(UserComment));

			References(m => m.EntryForComment).Column("[User]").Nullable();
		}
	}

	public class AlbumReviewMap : SubclassMap<AlbumReview>
	{
		public AlbumReviewMap()
		{
			DiscriminatorValue(nameof(AlbumReview));

			Map(m => m.LanguageCode).Not.Nullable();
			Map(m => m.Title).Not.Nullable();

			References(m => m.EntryForComment).Column("[Album]").Not.Nullable();
		}
	}
}
