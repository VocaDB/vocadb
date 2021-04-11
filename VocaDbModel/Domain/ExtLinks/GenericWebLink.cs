#nullable disable

using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.Domain.ExtLinks
{
	public abstract class GenericWebLink<TEntry> : WebLink where TEntry : class
	{
		private TEntry _entry;

		protected GenericWebLink() { }

		protected GenericWebLink(TEntry entry, WebLinkContract contract)
			: base(contract)
		{
			Entry = entry;
		}

		protected GenericWebLink(TEntry entry, string description, string url, WebLinkCategory category, bool disabled)
			: base(description, url, category, disabled)
		{
			Entry = entry;
		}

		public virtual TEntry Entry
		{
			get => _entry;
			set
			{
				ParamIs.NotNull(() => value);
				_entry = value;
			}
		}

#nullable enable
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
#nullable disable

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

#nullable enable
		public override string ToString()
		{
			return $"{base.ToString()} for {Entry}";
		}
#nullable disable
	}

	public class AlbumWebLink : GenericWebLink<Album>
	{
		public AlbumWebLink() { }

		public AlbumWebLink(Album album, string description, string url, WebLinkCategory category, bool disabled)
			: base(album, description, url, category, disabled) { }
	}

	public class ArtistWebLink : GenericWebLink<Artist>
	{
		public ArtistWebLink() { }

		public ArtistWebLink(Artist artist, string description, string url, WebLinkCategory category, bool disabled)
			: base(artist, description, url, category, disabled) { }
	}

	public class ReleaseEventWebLink : GenericWebLink<ReleaseEvent>
	{
		public ReleaseEventWebLink() { }

		public ReleaseEventWebLink(ReleaseEvent releaseEvent, string description, string url, WebLinkCategory category, bool disabled)
			: base(releaseEvent, description, url, category, disabled) { }
	}

	public class ReleaseEventSeriesWebLink : GenericWebLink<ReleaseEventSeries>
	{
		public ReleaseEventSeriesWebLink() { }

		public ReleaseEventSeriesWebLink(ReleaseEventSeries series, string description, string url, WebLinkCategory category, bool disabled)
			: base(series, description, url, category, disabled) { }
	}

	public class SongWebLink : GenericWebLink<Song>
	{
		public SongWebLink() { }

		public SongWebLink(Song song, string description, string url, WebLinkCategory category, bool disabled)
			: base(song, description, url, category, disabled) { }
	}

	public class TagWebLink : GenericWebLink<Tag>
	{
		public TagWebLink() { }

		public TagWebLink(Tag tag, WebLinkContract contract)
			: base(tag, contract) { }

		public TagWebLink(Tag tag, string description, string url, bool disabled)
			: base(tag, description, url, WebLinkCategory.Other, disabled) { }
	}

	public class UserWebLink : GenericWebLink<User>
	{
		public UserWebLink() { }

		public UserWebLink(User user, WebLinkContract contract)
			: base(user, contract) { }
	}

	public class VenueWebLink : GenericWebLink<Venue>
	{
		public VenueWebLink() { }

		public VenueWebLink(Venue venue, string description, string url, WebLinkCategory category, bool disabled)
			: base(venue, description, url, category, disabled) { }
	}
}
