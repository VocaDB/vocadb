using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Web.Models.Shared.Partials.Artist
{
	public class PrintArchivedArtistDataViewModel
	{
		public PrintArchivedArtistDataViewModel(ComparedArtistsContract comparedArtists)
		{
			ComparedArtists = comparedArtists;
		}

		public ComparedArtistsContract ComparedArtists { get; set; }
	}
}