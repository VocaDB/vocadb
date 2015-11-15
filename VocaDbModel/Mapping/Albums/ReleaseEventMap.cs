using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class ReleaseEventMap : ClassMap<ReleaseEvent> {

		public ReleaseEventMap() {

			Table("AlbumReleaseEvents");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Length(400).Not.Nullable();
			Map(m => m.Name).Length(50).Not.Nullable();
			Map(m => m.SeriesNumber).Not.Nullable();
			Map(m => m.SeriesSuffix).Length(50).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			HasMany(m => m.Albums).KeyColumn("[ReleaseEventName]").PropertyRef("Name").ForeignKeyConstraintName("[Name]").Inverse().ReadOnly();

			References(m => m.Series).Nullable();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Event]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Date, c => c.Map(m => m.DateTime).Column("[Date]").Nullable());

		}

	}

	public class ReleaseEventSeriesMap : ClassMap<ReleaseEventSeries> {

		public ReleaseEventSeriesMap() {

			Table("AlbumReleaseEventSeries");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Length(400).Not.Nullable();
			Map(m => m.Name).Length(50).Not.Nullable();
			Map(m => m.PictureMime).Length(32).Nullable();

			HasMany(m => m.Aliases).KeyColumn("[Series]").Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.Events).OrderBy("[SeriesNumber]").KeyColumn("[Series]").Inverse().Cache.ReadWrite();

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

	public class ArchivedReleaseEventVersionMap : ClassMap<ArchivedReleaseEventVersion> {

		public ArchivedReleaseEventVersionMap() {

			Table("ArchivedEventVersions");
			Id(m => m.Id);

			Map(m => m.CommonEditEvent).Length(30).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Date).Nullable();
			Map(m => m.Description).Length(400).Not.Nullable();
			Map(m => m.Name).Not.Nullable();
			Map(m => m.SeriesNumber).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.ReleaseEvent).Column("[Event]").Not.Nullable();

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
			});

		}

	}

}
