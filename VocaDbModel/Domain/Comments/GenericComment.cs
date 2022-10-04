using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Comments;

public class GenericComment<T> : Comment where T : class, IEntryWithNames
{
	private T _entry;

#nullable disable
	public GenericComment() { }
#nullable enable

	public GenericComment(T entry, string message, AgentLoginData loginData)
		: base(message, loginData)
	{
		EntryForComment = entry;
	}

	public override IEntryWithNames Entry => EntryForComment;

	public virtual T EntryForComment
	{
		get => _entry;
		[MemberNotNull(nameof(_entry))]
		set
		{
			ParamIs.NotNull(() => value);
			_entry = value;
		}
	}

	public override CommentType CommentType { get; }

	public virtual bool Equals(GenericComment<T>? another)
	{
		if (another == null)
			return false;

		if (ReferenceEquals(this, another))
			return true;

		if (Id == 0)
			return false;

		return Id == another.Id;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as GenericComment<T>);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("comment [{0}] for " + Entry, Id);
	}
}

public class AlbumComment : GenericComment<Album>
{
	public AlbumComment() { }

	public AlbumComment(Album album, string message, AgentLoginData loginData)
		: base(album, message, loginData) { }
}

public class ArtistComment : GenericComment<Artist>
{
	public ArtistComment() { }

	public ArtistComment(Artist artist, string message, AgentLoginData loginData)
		: base(artist, message, loginData) { }
}

public class DiscussionComment : GenericComment<DiscussionTopic>
{
	public DiscussionComment() { }

	public DiscussionComment(DiscussionTopic topic, string message, AgentLoginData loginData)
		: base(topic, message, loginData) { }
}

public class ReleaseEventComment : GenericComment<ReleaseEvent>
{
	public ReleaseEventComment() { }

	public ReleaseEventComment(ReleaseEvent entry, string message, AgentLoginData loginData)
		: base(entry, message, loginData) { }
}

public class SongComment : GenericComment<Song>
{
	public SongComment() { }

	public SongComment(Song song, string message, AgentLoginData loginData)
		: base(song, message, loginData) { }
}

public class SongListComment : GenericComment<SongList>
{
	public SongListComment() { }

	public SongListComment(SongList list, string message, AgentLoginData loginData)
		: base(list, message, loginData) { }
}

public class TagComment : GenericComment<Tag>
{
	public TagComment() { }

	public TagComment(Tag entry, string message, AgentLoginData loginData)
		: base(entry, message, loginData) { }

	public virtual TagComment Move(Tag target)
	{
		if (target.Equals(EntryForComment))
			return this;

		// TODO: have to make a clone because of NH reparenting issues, see http://stackoverflow.com/questions/28114508/nhibernate-change-parent-deleted-object-would-be-re-saved-by-cascade
		EntryForComment.AllComments.Remove(this);

		var newComment = new TagComment(
			entry: target,
			message: Message,
			loginData: new AgentLoginData(user: Author, name: Author.Name)
		)
		{
			Created = Created,
			Deleted = Deleted,
		};
		target.AllComments.Add(newComment);

		return newComment;
	}
}

/// <summary>
/// Comment created on user's profile.
/// </summary>
public class UserComment : GenericComment<User>
{
	public UserComment() { }

	public UserComment(User user, string message, AgentLoginData loginData)
		: base(user, message, loginData) { }
}

public class AlbumReview : GenericComment<Album>, IAlbumLink, IEntryWithIntId
{
#nullable disable
	public AlbumReview() { }
#nullable enable

	public AlbumReview(Album entry, string message, AgentLoginData loginData, string? title, string? languageCode)
		: base(entry, message, loginData)
	{
		Title = title ?? string.Empty;
		LanguageCode = languageCode ?? string.Empty;
	}

	public virtual string LanguageCode { get; set; }

	public virtual string Title { get; set; }

	Album IAlbumLink.Album => EntryForComment;
}
