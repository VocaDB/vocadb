using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents {

	public class EventSeriesTagUsageMap : ClassMap<EventSeriesTagUsage> {

		public EventSeriesTagUsageMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Count).Not.Nullable();
			Map(m => m.Date).Not.Nullable();

			References(m => m.Entry).Column("[EventSeries]").Not.Nullable();
			References(m => m.Tag).Not.Nullable();
			HasMany(m => m.Votes).KeyColumn("[Usage]").Inverse().Cascade.AllDeleteOrphan();

		}

	}

	public class EventSeriesTagVoteMap : ClassMap<EventSeriesTagVote> {

		public EventSeriesTagVoteMap() {

			Id(m => m.Id);

			References(m => m.User).Not.Nullable();
			References(m => m.Usage).Not.Nullable();

		}

	}
}
