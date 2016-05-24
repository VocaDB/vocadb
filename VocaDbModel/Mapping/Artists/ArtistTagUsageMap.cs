using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistTagUsageMap : ClassMap<ArtistTagUsage> {

		public ArtistTagUsageMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Count).Not.Nullable();
			Map(m => m.Date).Not.Nullable();

			References(m => m.Artist).Not.Nullable();
			References(m => m.Tag).Not.Nullable();
			HasMany(m => m.Votes).KeyColumn("[Usage]").Inverse().Cascade.AllDeleteOrphan();

		}

	}

}
