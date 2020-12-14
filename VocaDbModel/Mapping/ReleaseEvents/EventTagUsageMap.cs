#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents
{
	public class EventTagUsageMap : ClassMap<EventTagUsage>
	{
		public EventTagUsageMap()
		{
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Count).Not.Nullable();
			Map(m => m.Date).Not.Nullable();

			References(m => m.Entry).Column("[Event]").Not.Nullable();
			References(m => m.Tag).Not.Nullable();
			HasMany(m => m.Votes).KeyColumn("[Usage]").Inverse().Cascade.AllDeleteOrphan();
		}
	}

	public class EventTagVoteMap : ClassMap<EventTagVote>
	{
		public EventTagVoteMap()
		{
			Id(m => m.Id);

			References(m => m.User).Not.Nullable();
			References(m => m.Usage).Not.Nullable();
		}
	}
}
