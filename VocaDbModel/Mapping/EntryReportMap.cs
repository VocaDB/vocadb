#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.Mapping;

public class EntryReportMap : ClassMap<EntryReport>
{
	public EntryReportMap()
	{
		Id(m => m.Id);
		DiscriminateSubClassesOnColumn("[EntryType]");

		Map(m => m.ClosedAt).Nullable();
		Map(m => m.Created).Not.Nullable();
		Map(m => m.Hostname).Length(50).Not.Nullable();
		Map(m => m.Notes).Length(EntryReport.MaxNotesLength).Not.Nullable();
		Map(m => m.Status).Not.Nullable();
		Map(m => m.VersionNumber).Nullable();

		References(m => m.ClosedBy).Nullable();
		References(m => m.User).Nullable();
	}
}

public class AlbumReportMap : SubclassMap<AlbumReport>
{
	public AlbumReportMap()
	{
		DiscriminatorValue("Album");

		Map(m => m.ReportType).Not.Nullable();

		References(m => m.Entry).Column("Album").Not.Nullable();
	}
}

public class ArtistReportMap : SubclassMap<ArtistReport>
{
	public ArtistReportMap()
	{
		DiscriminatorValue("Artist");

		Map(m => m.ReportType).Not.Nullable();

		References(m => m.Entry).Column("Artist").Not.Nullable();
	}
}

public class EventReportMap : SubclassMap<EventReport>
{
	public EventReportMap()
	{
		DiscriminatorValue("Event");

		Map(m => m.ReportType).Not.Nullable();

		References(m => m.Entry).Column("Event").Not.Nullable();
	}
}

public class SongReportMap : SubclassMap<SongReport>
{
	public SongReportMap()
	{
		DiscriminatorValue("Song");

		Map(m => m.ReportType).Not.Nullable();

		References(m => m.Entry).Column("Song").Not.Nullable();
	}
}

public class TagReportMap : SubclassMap<TagReport>
{
	public TagReportMap()
	{
		DiscriminatorValue("Tag");

		Map(m => m.ReportType).Not.Nullable();

		References(m => m.Entry).Column("Tag").Not.Nullable();
	}
}

public class UserReportMap : SubclassMap<UserReport>
{
	public UserReportMap()
	{
		DiscriminatorValue("User");

		Map(m => m.ReportType).Not.Nullable();

		References(m => m.Entry).Column("ReportedUser").Not.Nullable();
	}
}

public class VenueReportMap : SubclassMap<VenueReport>
{
	public VenueReportMap()
	{
		DiscriminatorValue("Venue");

		Map(m => m.ReportType).Not.Nullable();

		References(m => m.Entry).Column("Venue").Not.Nullable();
	}
}
