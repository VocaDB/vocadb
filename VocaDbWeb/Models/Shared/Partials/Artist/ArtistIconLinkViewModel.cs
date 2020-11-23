using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Web.Models.Shared.Partials.Artist
{

	public class ArtistIconLinkViewModel
	{

		public ArtistIconLinkViewModel(ArtistContract artist)
		{
			Artist = artist;
		}

		public ArtistContract Artist { get; set; }

	}

}