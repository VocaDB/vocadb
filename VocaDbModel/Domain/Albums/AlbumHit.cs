
namespace VocaDb.Model.Domain.Albums {

	public class AlbumHit : GenericEntryHit<Album> {

		public AlbumHit() { }

		public AlbumHit(Album album, int agent) 
			: base(album, agent) {}

	}

}
