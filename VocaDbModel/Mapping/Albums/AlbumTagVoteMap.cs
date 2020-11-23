using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums
{
	public class AlbumTagVoteMap : ClassMap<AlbumTagVote>
	{
		public AlbumTagVoteMap()
		{
			Id(m => m.Id);

			References(m => m.User).Not.Nullable();
			References(m => m.Usage).Not.Nullable();
		}
	}
}
