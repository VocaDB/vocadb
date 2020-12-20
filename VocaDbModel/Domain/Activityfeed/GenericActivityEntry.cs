#nullable disable

using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Activityfeed
{
	public abstract class GenericActivityEntry<TEntry, TArchivedVersion> : ActivityEntry
		where TEntry : class, IEntryBase, IEntryWithNames
		where TArchivedVersion : ArchivedObjectVersion
	{
		private TEntry _entry;

		protected GenericActivityEntry() { }

		protected GenericActivityEntry(TEntry entry, EntryEditEvent editEvent, User author, TArchivedVersion archivedVersion)
			: base(author, editEvent)
		{
			ParamIs.NotNull(() => entry);

			Entry = entry;
			EditEvent = editEvent;
			ArchivedVersion = archivedVersion;
		}

		public virtual TArchivedVersion ArchivedVersion { get; set; }

		public override ArchivedObjectVersion ArchivedVersionBase => ArchivedVersion;

		public virtual TEntry Entry
		{
			get => _entry;
			set
			{
				ParamIs.NotNull(() => value);
				_entry = value;
			}
		}

		public override IEntryWithNames EntryBase => Entry;

		public override EntryType EntryType { get; }
	}

	public class AlbumActivityEntry : GenericActivityEntry<Album, ArchivedAlbumVersion>
	{
		public AlbumActivityEntry() { }

		public AlbumActivityEntry(Album album, EntryEditEvent editEvent, User author, ArchivedAlbumVersion archivedVersion)
			: base(album, editEvent, author, archivedVersion) { }
	}

	public class ArtistActivityEntry : GenericActivityEntry<Artist, ArchivedArtistVersion>
	{
		public ArtistActivityEntry() { }

		public ArtistActivityEntry(Artist artist, EntryEditEvent editEvent, User author, ArchivedArtistVersion archivedVersion)
			: base(artist, editEvent, author, archivedVersion) { }
	}

	public class ReleaseEventActivityEntry : GenericActivityEntry<ReleaseEvent, ArchivedReleaseEventVersion>
	{
		public ReleaseEventActivityEntry() { }

		public ReleaseEventActivityEntry(ReleaseEvent releaseEvent, EntryEditEvent editEvent, User author, ArchivedReleaseEventVersion archivedVersion)
			: base(releaseEvent, editEvent, author, archivedVersion) { }
	}

	public class SongActivityEntry : GenericActivityEntry<Song, ArchivedSongVersion>
	{
		public SongActivityEntry() { }

		public SongActivityEntry(Song song, EntryEditEvent editEvent, User author, ArchivedSongVersion archivedVersion)
			: base(song, editEvent, author, archivedVersion) { }
	}

	public class SongListActivityEntry : GenericActivityEntry<SongList, ArchivedSongListVersion>
	{
		public SongListActivityEntry() { }

		public SongListActivityEntry(SongList songlist, EntryEditEvent editEvent, User author, ArchivedSongListVersion archivedVersion)
			: base(songlist, editEvent, author, archivedVersion) { }
	}

	public class TagActivityEntry : GenericActivityEntry<Tag, ArchivedTagVersion>
	{
		public TagActivityEntry() { }

		public TagActivityEntry(Tag tag, EntryEditEvent editEvent, User author, ArchivedTagVersion archivedVersion)
			: base(tag, editEvent, author, archivedVersion) { }
	}

	public class VenueActivityEntry : GenericActivityEntry<Venue, ArchivedVenueVersion>
	{
		public VenueActivityEntry() { }

		public VenueActivityEntry(Venue venue, EntryEditEvent editEvent, User author, ArchivedVenueVersion archivedVersion)
			: base(venue, editEvent, author, archivedVersion) { }
	}
}
