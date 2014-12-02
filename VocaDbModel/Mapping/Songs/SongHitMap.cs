using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongHitMap : ClassMap<SongHit> {

		public SongHitMap() {

			Cache.ReadOnly();
			Id(m => m.Id);
			ReadOnly();

			Map(m => m.Agent).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

}
