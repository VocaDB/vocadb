using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.Domain.ExtLinks;

public abstract class GenericWebLink<TEntry> : WebLink where TEntry : class
{
	private TEntry _entry;

#nullable disable
	protected GenericWebLink() { }
#nullable enable

	protected GenericWebLink(TEntry entry, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(description, address, category, disabled)
	{
		Entry = entry;
	}

	public virtual TEntry Entry
	{
		get => _entry;
		[MemberNotNull(nameof(_entry))]
		set
		{
			ParamIs.NotNull(() => value);
			_entry = value;
		}
	}

	public virtual bool Equals(GenericWebLink<TEntry>? another)
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
		return Equals(obj as GenericWebLink<TEntry>);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public override string ToString()
	{
		return $"{base.ToString()} for {Entry}";
	}
}

public class AlbumWebLink : GenericWebLink<Album>
{
	public AlbumWebLink() { }

	public AlbumWebLink(Album album, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(album, description, address, category, disabled) { }
}

public class ArtistWebLink : GenericWebLink<Artist>
{
	public ArtistWebLink() { }

	public ArtistWebLink(Artist artist, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(artist, description, address, category, disabled) { }
}

public class ReleaseEventWebLink : GenericWebLink<ReleaseEvent>
{
	public ReleaseEventWebLink() { }

	public ReleaseEventWebLink(ReleaseEvent releaseEvent, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(releaseEvent, description, address, category, disabled) { }
}

public class ReleaseEventSeriesWebLink : GenericWebLink<ReleaseEventSeries>
{
	public ReleaseEventSeriesWebLink() { }

	public ReleaseEventSeriesWebLink(ReleaseEventSeries series, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(series, description, address, category, disabled) { }
}

public class SongWebLink : GenericWebLink<Song>
{
	public SongWebLink() { }

	public SongWebLink(Song song, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(song, description, address, category, disabled) { }
}

public class TagWebLink : GenericWebLink<Tag>
{
	public TagWebLink() { }

	public TagWebLink(Tag tag, string description, WebAddress address, bool disabled)
		: base(tag, description, address, WebLinkCategory.Other, disabled) { }
}

public class UserWebLink : GenericWebLink<User>
{
	public UserWebLink() { }

	public UserWebLink(User user, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(user, description, address, category, disabled) { }
}

public class VenueWebLink : GenericWebLink<Venue>
{
	public VenueWebLink() { }

	public VenueWebLink(Venue venue, string description, WebAddress address, WebLinkCategory category, bool disabled)
		: base(venue, description, address, category, disabled) { }
}
