using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class TagMap : ClassMap<Tag> {

		public TagMap() {

			Cache.NonStrictReadWrite();
			Id(m => m.Id);

			Map(m => m.CategoryName).Length(30).Not.Nullable();
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.HideFromSuggestions).Not.Nullable();
			Map(m => m.Status).CustomType(typeof(EntryStatus)).Not.Nullable();
			Map(m => m.Targets).Not.Nullable();
			Map(m => m.UsageCount).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Parent).Nullable();

			HasMany(m => m.AllAlbumTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.AllArtistTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.AllSongTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.AllChildren).KeyColumn("[Parent]").Inverse().Cache.ReadWrite();
			HasMany(m => m.Comments).Inverse().KeyColumn("[Tag]").Cascade.AllDeleteOrphan();
			HasMany(m => m.RelatedTags).Inverse().KeyColumn("[OwnerTag]").Cascade.AllDeleteOrphan().Cache.ReadWrite();
			HasMany(m => m.TagsForUsers).Cascade.AllDeleteOrphan().Inverse();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Tag]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Description, c => {
				c.Map(m => m.Original).Column("Description").Not.Nullable().Length(int.MaxValue);
				c.Map(m => m.English).Column("DescriptionEng").Not.Nullable().Length(int.MaxValue);
			});

			Component(m => m.Names, c => {
				c.Map(m => m.AdditionalNamesString).Not.Nullable().Length(1024);
				c.HasMany(m => m.Names).Table("TagNames").KeyColumn("[Tag]").Inverse().Cascade.All().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 => {
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
				});
			});

			Component(m => m.Thumb, c => {
				c.Map(m => m.Mime).Column("ThumbMime").Length(30);
				c.ParentReference(m => m.Entry);
			});

			Component(m => m.WebLinks, c => {
				c.HasMany(m => m.Links).Table("TagWebLinks").KeyColumn("[Tag]").Inverse().Cascade.All().Cache.ReadWrite();
			});

		}

	}

	public class ArchivedTagVersionMap : ClassMap<ArchivedTagVersion> {

		public ArchivedTagVersionMap() {

			Id(m => m.Id);

			Map(m => m.CommonEditEvent).Length(30).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Nullable();
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

	public class TagNameMap : ClassMap<TagName> {

		public TagNameMap() {

			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Length(255).Not.Nullable().Unique();

			References(m => m.Entry).Column("[Tag]").Not.Nullable();

		}

	}

	public class TagWebLinkMap : WebLinkMap<TagWebLink, Tag> {

		public TagWebLinkMap() : base(false) { }

	}

}
