using System.Collections.Generic;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain
{

	public interface IEntryWithArtistLinks<TArtistLink> where TArtistLink : IArtistLink
	{

		IList<TArtistLink> AllArtists { get; }

	}

}
