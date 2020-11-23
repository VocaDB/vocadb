using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists
{

	public class ArtistNameMap : ClassMap<ArtistName>
	{

		public ArtistNameMap()
		{

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Length(255).Not.Nullable();

			References(m => m.Artist).Not.Nullable();

		}

	}

}
