using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class TagMap : ClassMap<Tag> {

		public TagMap() {

			Cache.NonStrictReadWrite();
			Id(m => m.Name).Column("[Name]").Length(30).GeneratedBy.Assigned();

			Map(m => m.Id).Generated.Insert().Not.Nullable().Not.Insert().Not.Update();
			Map(m => m.CategoryName).Length(30).Not.Nullable();
			Map(m => m.Description).Length(1000).Not.Nullable();
			Map(m => m.EnglishName).Length(100).Not.Nullable();
			Map(m => m.Status).CustomType(typeof(EntryStatus)).Not.Nullable();
			Map(m => m.TagName).Column("[Name]").ReadOnly().Not.Insert();
			Map(m => m.Version).Not.Nullable();

			References(m => m.AliasedTo).Nullable();
			References(m => m.Parent).Nullable();

			HasMany(m => m.Aliases).KeyColumn("[AliasedTo]").Inverse();
			HasMany(m => m.AllAlbumTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.AllArtistTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.AllSongTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.Children).KeyColumn("[Parent]").Inverse().Cache.ReadWrite();
			HasMany(m => m.Comments).Inverse().KeyColumn("[Tag]").PropertyRef("Id").Cascade.AllDeleteOrphan();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Tag]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Thumb, c => {
				c.Map(m => m.Mime).Column("ThumbMime").Length(30);
				c.ParentReference(m => m.Entry);
			});

		}

	}

	public class ArchivedTagVersionMap : ClassMap<ArchivedTagVersion> {

		public ArchivedTagVersionMap() {

			Id(m => m.Id);

			Map(m => m.CategoryName).Length(30).Not.Nullable();
			Map(m => m.CommonEditEvent).Length(30).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Description).Length(1000).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.Tag).Not.Nullable();

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
			});

		}

	}
}
