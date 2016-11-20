using System.Collections.Generic;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain {

	/// <summary>
	/// Entry with associated artists (possibly through link object).
	/// </summary>
	public interface IEntryWithArtists {

		IEnumerable<Artist> ArtistList { get; }

	}

}
