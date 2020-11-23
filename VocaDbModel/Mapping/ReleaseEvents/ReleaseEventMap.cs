using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents
{
	public class ReleaseEventMap : ClassMap<ReleaseEvent>
	{
		public ReleaseEventMap()
		{
			Table("AlbumReleaseEvents");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Category).Not.Nullable();
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.CustomName).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Description).Length(1000).Not.Nullable();
			Map(m => m.PictureMime).Length(32).Nullable();
			Map(m => m.SeriesNumber).Not.Nullable();
			Map(m => m.SeriesSuffix).Length(50).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.VenueName).Column("VenueName").Length(1000).Nullable();
			Map(m => m.Version).Not.Nullable();

			HasMany(m => m.AllAlbums).KeyColumn("[ReleaseEvent]").Inverse().Cache.ReadWrite();
			HasMany(m => m.AllArtists).KeyColumn("[Event]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			HasMany(m => m.AllSongs).KeyColumn("[ReleaseEvent]").Inverse().Cache.ReadWrite();
			HasMany(m => m.Comments).KeyColumn("[ReleaseEvent]").Inverse().Cascade.AllDeleteOrphan();
			HasMany(m => m.Users).Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.WebLinks).KeyColumn("[ReleaseEvent]").Inverse().Cascade.All().Cache.ReadWrite();

			References(m => m.Series).Nullable();
			References(m => m.SongList).Nullable();
			References(m => m.Venue).Column("[Venue]").Nullable();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Event]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Names, c =>
			{
				c.Map(m => m.AdditionalNamesString).Not.Nullable().Length(1024);
				c.HasMany(m => m.Names).Table("EventNames").KeyColumn("[Event]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 =>
				{
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName").Not.Nullable();
					c2.Map(m => m.English, "EnglishName").Not.Nullable();
					c2.Map(m => m.Romaji, "RomajiName").Not.Nullable();
				});
			});

			Component(m => m.PVs, c =>
			{
				c.HasMany(m => m.PVs).KeyColumn("[Event]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			});

			Component(m => m.Tags, c =>
			{
				c.HasMany(m => m.Usages).KeyColumn("[Event]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			});

			Component(m => m.Date, c => c.Map(m => m.DateTime).Column("[Date]").Nullable());
			Component(m => m.EndDate, c => c.Map(m => m.DateTime).Column("[EndDate]").Nullable());
		}
	}

	public class ReleaseEventCommentMap : CommentMap<ReleaseEventComment, ReleaseEvent>
	{
		public ReleaseEventCommentMap()
		{
			References(m => m.EntryForComment).Column("[ReleaseEvent]").Not.Nullable();
		}
	}

	public class EventNameMap : ClassMap<EventName>
	{
		public EventNameMap()
		{
			Table("EventNames");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Length(255).Not.Nullable().Unique();
			References(m => m.Entry).Column("[Event]").Not.Nullable();
		}
	}

	public class ReleaseEventWebLinkMap : WebLinkMap<ReleaseEventWebLink, ReleaseEvent> { }

	public class ArchivedReleaseEventVersionMap : ClassMap<ArchivedReleaseEventVersion>
	{
		public ArchivedReleaseEventVersionMap()
		{
			Table("ArchivedEventVersions");
			Id(m => m.Id);

			Map(m => m.CommonEditEvent).Length(30).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data); // Some old events have null data
			Map(m => m.Hidden).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.ReleaseEvent).Column("[Event]").Not.Nullable();

			Component(m => m.Diff, c =>
			{
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
			});
		}
	}

	public class ArtistForEventMap : ClassMap<ArtistForEvent>
	{
		public ArtistForEventMap()
		{
			Table("ArtistsForEvents");
			Id(m => m.Id);
			Cache.ReadWrite();

			Map(m => m.Name).Length(250).Nullable();
			Map(m => m.Roles).CustomType(typeof(ArtistEventRoles)).Not.Nullable();

			References(m => m.Artist).Nullable();
			References(m => m.ReleaseEvent).Column("[Event]").Not.Nullable();
		}
	}
}
