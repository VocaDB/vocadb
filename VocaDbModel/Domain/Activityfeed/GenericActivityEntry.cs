using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Activityfeed {

	public abstract class GenericActivityEntry<T> : ActivityEntry where T : class, IEntryBase, IEntryWithNames {

		private T entry;

		protected GenericActivityEntry() { }

		protected GenericActivityEntry(T entry, EntryEditEvent editEvent, User author)
			: base(author, editEvent) {

			Entry = entry;
			EditEvent = editEvent;

		}

		public virtual T Entry {
			get { return entry; }
			set {
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public override IEntryWithNames EntryBase {
			get { return Entry; }
		}

		public override EntryType EntryType {
			get { return Entry.EntryType; }
		}

	}

	public class AlbumActivityEntry : GenericActivityEntry<Album> {

		public AlbumActivityEntry() { }

		public AlbumActivityEntry(Album album, EntryEditEvent editEvent, User author)
			: base(album, editEvent, author) { }

	}

	public class ArtistActivityEntry : GenericActivityEntry<Artist> {

		public ArtistActivityEntry() { }

		public ArtistActivityEntry(Artist artist, EntryEditEvent editEvent, User author)
			: base(artist, editEvent, author) { }

	}

	public class SongActivityEntry : GenericActivityEntry<Song> {

		public SongActivityEntry() { }

		public SongActivityEntry(Song song, EntryEditEvent editEvent, User author)
			: base(song, editEvent, author) { }

	}

}
