using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents {

	public class ReleaseEventMap : ClassMap<ReleaseEvent> {

		public ReleaseEventMap() {

			Table("AlbumReleaseEvents");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.CustomName).Not.Nullable();
			Map(m => m.Description).Length(400).Not.Nullable();
			Map(m => m.Name).Length(50).Not.Nullable();
			Map(m => m.PictureMime).Length(32).Nullable();
			Map(m => m.SeriesNumber).Not.Nullable();
			Map(m => m.SeriesSuffix).Length(50).Not.Nullable();
			Map(m => m.Venue).Length(1000).Nullable();
			Map(m => m.Version).Not.Nullable();

			HasMany(m => m.AllAlbums).KeyColumn("[ReleaseEvent]").Inverse().Cache.ReadWrite();
			HasMany(m => m.AllSongs).KeyColumn("[ReleaseEvent]").Inverse().Cache.ReadWrite();
			HasMany(m => m.Comments).KeyColumn("[ReleaseEvent]").Inverse().Cascade.AllDeleteOrphan();
			HasMany(m => m.Users).Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.WebLinks).KeyColumn("[ReleaseEvent]").Inverse().Cascade.All().Cache.ReadWrite();

			References(m => m.Series).Nullable();
			References(m => m.SongList).Nullable();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Event]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Date, c => c.Map(m => m.DateTime).Column("[Date]").Nullable());

		}

	}

	public class ReleaseEventCommentMap : CommentMap<ReleaseEventComment, ReleaseEvent> {

		public ReleaseEventCommentMap() {
			References(m => m.EntryForComment).Column("[ReleaseEvent]").Not.Nullable();
		}

	}

	public class ReleaseEventWebLinkMap : WebLinkMap<ReleaseEventWebLink, ReleaseEvent> { }

	public class ArchivedReleaseEventVersionMap : ClassMap<ArchivedReleaseEventVersion> {

		public ArchivedReleaseEventVersionMap() {

			Table("ArchivedEventVersions");
			Id(m => m.Id);

			Map(m => m.CommonEditEvent).Length(30).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.ReleaseEvent).Column("[Event]").Not.Nullable();

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
			});

		}

	}

}
