#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums
{
	public class AlbumTagUsageMap : ClassMap<AlbumTagUsage>
	{
		public AlbumTagUsageMap()
		{
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Count).Not.Nullable();
			Map(m => m.Date).Not.Nullable();

			References(m => m.Entry).Column("[Album]").Not.Nullable();
			References(m => m.Tag).Not.Nullable();
			HasMany(m => m.Votes).KeyColumn("[Usage]").Inverse().Cascade.AllDeleteOrphan();
		}
	}
}
