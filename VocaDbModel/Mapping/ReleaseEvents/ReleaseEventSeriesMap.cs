using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents {

	public class ReleaseEventSeriesMap : ClassMap<ReleaseEventSeries> {

		public ReleaseEventSeriesMap() {

			Table("AlbumReleaseEventSeries");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Category).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Description).Length(400).Not.Nullable();
			Map(m => m.Name).Length(50).Not.Nullable();
			Map(m => m.PictureMime).Length(32).Nullable();
			Map(m => m.Version).Not.Nullable();

			HasMany(m => m.Aliases).KeyColumn("[Series]").Inverse().Cascade.All().Cache.ReadWrite();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Series]").Inverse().Cascade.All());

			HasMany(m => m.Events).OrderBy("[SeriesNumber]").KeyColumn("[Series]").Inverse().Cache.ReadWrite();
			HasMany(m => m.WebLinks).KeyColumn("[ReleaseEventSeries]").Inverse().Cascade.All().Cache.ReadWrite();

		}

	}

	public class ReleaseEventSeriesAliasMap : ClassMap<ReleaseEventSeriesAlias> {

		public ReleaseEventSeriesAliasMap() {

			Table("AlbumReleaseEventSeriesAliases");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Name).Length(50).Not.Nullable();
			References(m => m.Series).Column("[Series]").Not.Nullable();

		}

	}

	public class ReleaseEventSeriesWebLinkMap : WebLinkMap<ReleaseEventSeriesWebLink, ReleaseEventSeries> { }

}