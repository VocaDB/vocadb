using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongListMap : ClassMap<SongList> {

		public SongListMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Description).Length(4000).Not.Nullable();
			Map(m => m.FeaturedCategory).Length(20).Not.Nullable();
			Map(m => m.Name).Length(200).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[SongList]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.EventDate, c => c.Map(m => m.DateTime).Column("EventDate").Nullable());

			Component(m => m.Thumb, c => {
				c.Map(m => m.Mime).Column("ThumbMime").Length(30);
				c.ParentReference(m => m.Entry);
			});

			References(m => m.Author).Not.Nullable();

			HasMany(m => m.AllSongs)
				.KeyColumn("[List]")
				.OrderBy("[Order]")
				.Inverse().Cascade.AllDeleteOrphan()
				.Cache.ReadWrite();

			HasMany(m => m.Comments).Inverse().KeyColumn("SongList").Cascade.AllDeleteOrphan().OrderBy("Created");

		}

	}

	public class SongInListMap : ClassMap<SongInList> {

		public SongInListMap() {

			Table("SongsInLists");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Order).Not.Nullable();

			References(m => m.List).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class ArchivedSongListVersionMap : ClassMap<ArchivedSongListVersion> {

		public ArchivedSongListVersionMap() {

			Id(m => m.Id);

			Map(m => m.CommonEditEvent).Length(30).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.SongList).Not.Nullable();

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
			});

		}

	}

}
