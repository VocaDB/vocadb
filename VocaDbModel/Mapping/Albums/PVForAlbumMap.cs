using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class PVForAlbumMap : ClassMap<PVForAlbum> {

		public PVForAlbumMap() {

			Table("PVsForAlbums");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Author).Length(100).Not.Nullable();
			Map(m => m.Name).Length(200).Not.Nullable();
			Map(m => m.PVId).Length(255).Not.Nullable();
			Map(m => m.PVType).Not.Nullable();
			Map(m => m.Service).Not.Nullable();

			References(m => m.Album).Not.Nullable();

			Component(m => m.ExtendedMetadata, c => {
				c.Map(m => m.Json, "ExtendedMetadataJson").Nullable();
			});

		}

	}

}
