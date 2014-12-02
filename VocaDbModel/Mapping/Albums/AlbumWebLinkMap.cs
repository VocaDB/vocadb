using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumWebLinkMap : ClassMap<AlbumWebLink> {

		public AlbumWebLinkMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Category).Not.Nullable();
			Map(m => m.Description).Not.Nullable();
			Map(m => m.Url).Not.Nullable();

			References(m => m.Album).Not.Nullable();

		}

	}

}
