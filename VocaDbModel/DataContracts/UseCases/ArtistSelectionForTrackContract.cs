using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class ArtistSelectionForTrackContract
	{
		public ArtistSelectionForTrackContract() { }

		public ArtistSelectionForTrackContract(Artist artist, bool selected, ContentLanguagePreference languagePreference)
		{
			Artist = new ArtistContract(artist, languagePreference);
			Selected = selected;
		}

		public ArtistContract Artist { get; set; }

		public bool Selected { get; set; }
	}
}
