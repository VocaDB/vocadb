using System;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Service.ExtSites {

	public class ArtistDescriptionGenerator {

		public string GenerateDescription(ArtistContract artist, Func<ArtistType, string> artistTypeNames) {

			return artistTypeNames(artist.ArtistType);

		}

	}

}
