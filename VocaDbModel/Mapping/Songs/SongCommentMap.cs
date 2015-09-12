using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongCommentMap : ClassMap<SongComment> {

		public SongCommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(4000).Not.Nullable();

			References(m => m.EntryForComment).Column("[Song]").Not.Nullable();
			References(m => m.Author).Not.Nullable();

		}

	}

}
