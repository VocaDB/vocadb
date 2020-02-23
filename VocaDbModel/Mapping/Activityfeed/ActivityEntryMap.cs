using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Mapping.Activityfeed {

	public class ActivityEntryMap : ClassMap<ActivityEntry> {

		public ActivityEntryMap() {

			DiscriminateSubClassesOnColumn("[EntryType]");
			Table("ActivityEntries");
			Id(m => m.Id);
			Cache.ReadWrite();

			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.EditEvent).Not.Nullable();

			References(m => m.Author).Not.Nullable();

		}

	}

	public class AlbumActivityEntryMap : SubclassMap<AlbumActivityEntry> {

		public AlbumActivityEntryMap() {

			DiscriminatorValue("Album");

			References(m => m.ArchivedVersion).Column("[ArchivedAlbumVersion]").Nullable();
			References(m => m.Entry).Column("[Album]").Nullable();

		}

	}

	public class ArtistActivityEntryMap : SubclassMap<ArtistActivityEntry> {

		public ArtistActivityEntryMap() {

			DiscriminatorValue("Artist");

			References(m => m.ArchivedVersion).Column("[ArchivedArtistVersion]").Nullable();
			References(m => m.Entry).Column("[Artist]").Nullable();

		}

	}

	public class ReleaseEventActivityEntryMap : SubclassMap<ReleaseEventActivityEntry> {

		public ReleaseEventActivityEntryMap() {

			DiscriminatorValue(EntryType.ReleaseEvent.ToString());

			References(m => m.ArchivedVersion).Column("[ArchivedReleaseEventVersion]").Nullable();
			References(m => m.Entry).Column("[ReleaseEvent]").Nullable();

		}

	}

	public class SongActivityEntryMap : SubclassMap<SongActivityEntry> {

		public SongActivityEntryMap() {

			DiscriminatorValue("Song");

			References(m => m.ArchivedVersion).Column("[ArchivedSongVersion]").Nullable();
			References(m => m.Entry).Column("[Song]").Nullable();

		}

	}

	public class SongListActivityEntryMap : SubclassMap<SongListActivityEntry> {

		public SongListActivityEntryMap() {

			DiscriminatorValue("SongList");

			References(m => m.ArchivedVersion).Column("[ArchivedSongListVersion]").Nullable();
			References(m => m.Entry).Column("[SongList]").Nullable();

		}

	}

	public class TagActivityEntryMap : SubclassMap<TagActivityEntry> {

		public TagActivityEntryMap() {

			DiscriminatorValue("Tag");

			References(m => m.ArchivedVersion).Column("[ArchivedTagVersion]").Nullable();
			References(m => m.Entry).Column("[Tag]").Nullable();

		}

	}

	public class VenueActivityEntryMap : SubclassMap<VenueActivityEntry> {

		public VenueActivityEntryMap() {

			DiscriminatorValue("Venue");

			References(m => m.ArchivedVersion).Column("[ArchivedVenueVersion]").Nullable();
			References(m => m.Entry).Column("[Venue]").Nullable();

		}

	}

}
