using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumHitMap : ClassMap<AlbumHit> {

		public AlbumHitMap() {

			Cache.NonStrictReadWrite();
			Id(m => m.Id);
			ReadOnly();

			Map(m => m.Agent).Not.Nullable();
			References(m => m.Album).Not.Nullable();

		}

	}

}
