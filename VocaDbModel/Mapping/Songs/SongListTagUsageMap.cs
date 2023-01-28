#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs;

public class SongListTagUsageMap : ClassMap<SongListTagUsage>
{
	public SongListTagUsageMap()
	{
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.Count).Not.Nullable();
		Map(m => m.Date).Not.Nullable();

		References(m => m.Entry).Column("[SongList]").Not.Nullable();
		References(m => m.Tag).Not.Nullable();
		HasMany(m => m.Votes).KeyColumn("[Usage]").Inverse().Cascade.AllDeleteOrphan();
	}
}
