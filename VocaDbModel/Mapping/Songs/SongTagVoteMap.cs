using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs
{
	public class SongTagVoteMap : ClassMap<SongTagVote>
	{
		public SongTagVoteMap()
		{
			Id(m => m.Id);

			References(m => m.User).Not.Nullable();
			References(m => m.Usage).Not.Nullable();
		}
	}
}
