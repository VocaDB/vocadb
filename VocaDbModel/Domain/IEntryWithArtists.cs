using System.Collections.Generic;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain {

	public interface IEntryWithArtists<TArtistLink> where TArtistLink : IArtistLink {

		IList<TArtistLink> AllArtists { get; }

	}

}
