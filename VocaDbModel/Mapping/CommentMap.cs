using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping
{
	public abstract class CommentMap<TComment, TEntry> : ClassMap<TComment> where TComment : GenericComment<TEntry> where TEntry : class, IEntryWithNames
	{
		protected CommentMap()
		{
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(4000).Not.Nullable();

			References(m => m.Author).Not.Nullable();
		}
	}

	public class AlbumCommentMap : CommentMap<AlbumComment, Album>
	{
		public AlbumCommentMap()
		{
			References(m => m.EntryForComment).Column("[Album]").Not.Nullable();
		}
	}

	public class ArtistCommentMap : CommentMap<ArtistComment, Artist>
	{
		public ArtistCommentMap()
		{
			References(m => m.EntryForComment).Column("[Artist]").Not.Nullable();
		}
	}

	public class DiscussionCommentMap : CommentMap<DiscussionComment, DiscussionTopic>
	{
		public DiscussionCommentMap()
		{
			Schema("discussions");
			Table("DiscussionComments");

			References(m => m.EntryForComment).Column("[Topic]").Not.Nullable();
		}
	}

	public class ReleaseEventCommentMap : CommentMap<ReleaseEventComment, ReleaseEvent>
	{
		public ReleaseEventCommentMap()
		{
			References(m => m.EntryForComment).Column("[ReleaseEvent]").Not.Nullable();
		}
	}

	public class SongCommentMap : CommentMap<SongComment, Song>
	{
		public SongCommentMap()
		{
			References(m => m.EntryForComment).Column("[Song]").Not.Nullable();
		}
	}

	public class SongListCommentMap : CommentMap<SongListComment, SongList>
	{
		public SongListCommentMap()
		{
			References(m => m.EntryForComment).Column("SongList").Not.Nullable();
		}
	}

	public class TagCommentMap : CommentMap<TagComment, Tag>
	{
		public TagCommentMap()
		{
			References(m => m.EntryForComment).Column("[Tag]").Not.Nullable();
		}
	}

	public class UserCommentMap : CommentMap<UserComment, User>
	{
		public UserCommentMap()
		{
			References(m => m.EntryForComment).Column("[User]").Not.Nullable();
		}
	}
}
