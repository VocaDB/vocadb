using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.Mapping
{
	public class CommentMap : ClassMap<Comment>
	{
		protected CommentMap()
		{
			DiscriminateSubClassesOnColumn("[EntryType]");
			Table("Comments");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Message).Length(4000).Not.Nullable();

			References(m => m.Author).Not.Nullable();
		}
	}

	public class AlbumCommentMap : SubclassMap<AlbumComment>
	{
		public AlbumCommentMap()
		{
			DiscriminatorValue(EntryType.Album.ToString());

			References(m => m.EntryForComment).Column("[Album]").Nullable();
		}
	}

	public class ArtistCommentMap : SubclassMap<ArtistComment>
	{
		public ArtistCommentMap()
		{
			DiscriminatorValue(EntryType.Artist.ToString());

			References(m => m.EntryForComment).Column("[Artist]").Nullable();
		}
	}

	public class DiscussionCommentMap : SubclassMap<DiscussionComment>
	{
		public DiscussionCommentMap()
		{
			DiscriminatorValue(EntryType.DiscussionTopic.ToString());

			References(m => m.EntryForComment).Column("[Topic]").Nullable();
		}
	}

	public class ReleaseEventCommentMap : SubclassMap<ReleaseEventComment>
	{
		public ReleaseEventCommentMap()
		{
			DiscriminatorValue(EntryType.ReleaseEvent.ToString());

			References(m => m.EntryForComment).Column("[ReleaseEvent]").Nullable();
		}
	}

	public class SongCommentMap : SubclassMap<SongComment>
	{
		public SongCommentMap()
		{
			DiscriminatorValue(EntryType.Song.ToString());

			References(m => m.EntryForComment).Column("[Song]").Nullable();
		}
	}

	public class SongListCommentMap : SubclassMap<SongListComment>
	{
		public SongListCommentMap()
		{
			DiscriminatorValue(EntryType.SongList.ToString());

			References(m => m.EntryForComment).Column("SongList").Nullable();
		}
	}

	public class TagCommentMap : SubclassMap<TagComment>
	{
		public TagCommentMap()
		{
			DiscriminatorValue(EntryType.Tag.ToString());

			References(m => m.EntryForComment).Column("[Tag]").Nullable();
		}
	}

	public class UserCommentMap : SubclassMap<UserComment>
	{
		public UserCommentMap()
		{
			DiscriminatorValue(EntryType.User.ToString());

			References(m => m.EntryForComment).Column("[User]").Nullable();
		}
	}
}
