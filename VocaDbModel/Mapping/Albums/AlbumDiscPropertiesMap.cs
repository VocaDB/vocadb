#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums
{
	public class AlbumDiscPropertiesMap : ClassMap<AlbumDiscProperties>
	{
		public AlbumDiscPropertiesMap()
		{
			Table("AlbumDiscProperties");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.DiscNumber).Not.Nullable();
			Map(m => m.MediaType).Not.Nullable();
			Map(m => m.Name).Not.Nullable().Length(200);

			References(m => m.Album).Not.Nullable();
		}
	}
}
