using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents
{
	public class ReleaseEventSeriesMap : ClassMap<ReleaseEventSeries>
	{
		public ReleaseEventSeriesMap()
		{
			Table("AlbumReleaseEventSeries");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Category).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Description).Length(1000).Not.Nullable();
			Map(m => m.PictureMime).Length(32).Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Series]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Names, c =>
			{
				c.Map(m => m.AdditionalNamesString).Not.Nullable().Length(1024);
				c.HasMany(m => m.Names).Table("EventSeriesNames").KeyColumn("[Series]").Inverse().Cascade.All().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 =>
				{
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
				});
			});

			Component(m => m.Tags, c =>
			{
				c.HasMany(m => m.Usages).KeyColumn("[EventSeries]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			});

			HasMany(m => m.AllEvents).OrderBy("[SeriesNumber]").KeyColumn("[Series]").Inverse().Cache.ReadWrite();
			HasMany(m => m.WebLinks).KeyColumn("[ReleaseEventSeries]").Inverse().Cascade.All().Cache.ReadWrite();
		}
	}

	public class EventSeriesNameMap : ClassMap<EventSeriesName>
	{
		public EventSeriesNameMap()
		{
			Table("EventSeriesNames");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Length(255).Not.Nullable();
			References(m => m.Entry).Column("[Series]").Not.Nullable();
		}
	}

	public class ReleaseEventSeriesWebLinkMap : WebLinkMap<ReleaseEventSeriesWebLink, ReleaseEventSeries> { }
}