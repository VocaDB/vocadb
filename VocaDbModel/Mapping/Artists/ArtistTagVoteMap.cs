using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists
{

	public class ArtistTagVoteMap : ClassMap<ArtistTagVote>
	{

		public ArtistTagVoteMap()
		{

			Id(m => m.Id);

			References(m => m.User).Not.Nullable();
			References(m => m.Usage).Not.Nullable();

		}

	}
}
