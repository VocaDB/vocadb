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
		private TEntry entry;

		protected GenericWebLink() { }

		protected GenericWebLink(TEntry entry, WebLinkContract contract)
			: base(contract)
		{
			Entry = entry;
		}

		protected GenericWebLink(TEntry entry, string description, string url, WebLinkCategory category)
			: base(description, url, category)
		{
			Entry = entry;
		}

		public virtual TEntry Entry
		{
			get { return entry; }
			set
			{
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public virtual bool Equals(GenericWebLink<TEntry> another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as GenericWebLink<TEntry>);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0} for {1}", base.ToString(), Entry);
		}
	}

	public class AlbumWebLink : GenericWebLink<Album>
	{
		public AlbumWebLink() { }

		public AlbumWebLink(Album album, string description, string url, WebLinkCategory category)
			: base(album, description, url, category) { }
	}

	public class ArtistWebLink : GenericWebLink<Artist>
	{
		public ArtistWebLink() { }

		public ArtistWebLink(Artist artist, string description, string url, WebLinkCategory category)
			: base(artist, description, url, category) { }
	}

	public class ReleaseEventWebLink : GenericWebLink<ReleaseEvent>
	{
		public ReleaseEventWebLink() { }

		public ReleaseEventWebLink(ReleaseEvent releaseEvent, string description, string url, WebLinkCategory category)
			: base(releaseEvent, description, url, category) { }
	}

	public class ReleaseEventSeriesWebLink : GenericWebLink<ReleaseEventSeries>
	{
		public ReleaseEventSeriesWebLink() { }

		public ReleaseEventSeriesWebLink(ReleaseEventSeries series, string description, string url, WebLinkCategory category)
			: base(series, description, url, category) { }
	}

	public class SongWebLink : GenericWebLink<Song>
	{
		public SongWebLink() { }

		public SongWebLink(Song song, string description, string url, WebLinkCategory category)
			: base(song, description, url, category) { }
	}

	public class TagWebLink : GenericWebLink<Tag>
	{
		public TagWebLink() { }

		public TagWebLink(Tag tag, WebLinkContract contract)
			: base(tag, contract) { }

		public TagWebLink(Tag tag, string description, string url)
			: base(tag, description, url, WebLinkCategory.Other) { }
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

		public VenueWebLink(Venue venue, string description, string url, WebLinkCategory category)
			: base(venue, description, url, category) { }
	}
}
