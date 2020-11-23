using System.Collections.Generic;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Web.Models.Shared.Partials.Artist
{
	public class ArtistLinkListViewModel
	{
		public ArtistLinkListViewModel(IEnumerable<ArtistContract> artists, bool typeLabel = false, bool releaseYear = false)
		{
			Artists = artists;
			TypeLabel = typeLabel;
			ReleaseYear = releaseYear;
		}

		public IEnumerable<ArtistContract> Artists { get; set; }

		public bool TypeLabel { get; set; }

		public bool ReleaseYear { get; set; }
	}
}