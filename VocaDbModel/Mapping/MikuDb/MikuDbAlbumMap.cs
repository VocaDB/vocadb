#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.MikuDb;

namespace VocaDb.Model.Mapping.MikuDb
{
	public class MikuDbAlbumMap : ClassMap<MikuDbAlbum>
	{
		public MikuDbAlbumMap()
		{
			Schema("mikudb");
			Table("ImportedAlbums");

			Id(m => m.Id);

			Map(m => m.CoverPictureMime).Length(32).Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.SourceUrl).Length(255).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Title).Length(100).Not.Nullable();

			Component(m => m.CoverPicture, c =>
			{
				c.Map(m => m.Bytes, "CoverPictureBytes").Length(int.MaxValue).LazyLoad();
			});
		}
	}
}
