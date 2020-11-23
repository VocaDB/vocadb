using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists
{
	public class ArtistPictureFileMap : ClassMap<ArtistPictureFile>
	{
		public ArtistPictureFileMap()
		{
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Mime).Length(32).Not.Nullable();
			Map(m => m.Name).Length(200).Not.Nullable();

			References(m => m.Artist).Not.Nullable();
			References(m => m.Author).Not.Nullable();
		}
	}
}
