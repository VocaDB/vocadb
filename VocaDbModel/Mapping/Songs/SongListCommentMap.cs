using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongListCommentMap : ClassMap<SongListComment> {

		public SongListCommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(4000).Not.Nullable();

			References(m => m.EntryForComment).Column("SongList").Not.Nullable();
			References(m => m.Author).Not.Nullable();

		}

	}

}
