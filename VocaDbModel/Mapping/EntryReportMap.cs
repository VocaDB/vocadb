using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping {

	public class EntryReportMap : ClassMap<EntryReport> {

		public EntryReportMap() {

			Id(m => m.Id);
			DiscriminateSubClassesOnColumn("[EntryType]");

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Hostname).Length(50).Not.Nullable();
			Map(m => m.Notes).Length(EntryReport.MaxNotesLength).Not.Nullable();

			References(m => m.User).Nullable();

		}

	}

	public class AlbumReportMap : SubclassMap<AlbumReport> {

		public AlbumReportMap() {

			DiscriminatorValue("Album");

			Map(m => m.ReportType).Not.Nullable();

			References(m => m.Album).Not.Nullable();

		}

	}

	public class ArtistReportMap : SubclassMap<ArtistReport> {

		public ArtistReportMap() {

			DiscriminatorValue("Artist");

			Map(m => m.ReportType).Not.Nullable();

			References(m => m.Artist).Not.Nullable();

		}

	}

	public class SongReportMap : SubclassMap<SongReport> {

		public SongReportMap() {

			DiscriminatorValue("Song");

			Map(m => m.ReportType).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

	public class UserReportMap : SubclassMap<UserReport> {

		public UserReportMap() {

			DiscriminatorValue("User");

			Map(m => m.ReportType).Not.Nullable();

			References(m => m.ReportedUser).Not.Nullable();

		}

	}
}
