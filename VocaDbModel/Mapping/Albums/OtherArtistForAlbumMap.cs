#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Albums
{
	public class OtherArtistForAlbumMap : ClassMap<OtherArtistForAlbum>
	{
		public OtherArtistForAlbumMap()
		{
			Schema("dbo");
			Table("OtherArtistsForAlbums");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.IsSupport).Not.Nullable();
			Map(m => m.Name).Not.Nullable().Length(250);
			Map(m => m.Roles).CustomType(typeof(ArtistRoles)).Not.Nullable();
			References(m => m.Album).Not.Nullable();
		}
	}
}
