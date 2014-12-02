using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongTagUsageMap : ClassMap<SongTagUsage> {

		public SongTagUsageMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Count).Not.Nullable();
			References(m => m.Song).Not.Nullable();
			References(m => m.Tag).Not.Nullable();
			HasMany(m => m.Votes).KeyColumn("[Usage]").Inverse().Cascade.AllDeleteOrphan();

		}

	}

}
