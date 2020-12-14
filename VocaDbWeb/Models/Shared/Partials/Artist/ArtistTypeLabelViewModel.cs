#nullable disable

using VocaDb.Model.Domain.Artists;

namespace VocaDb.Web.Models.Shared.Partials.Artist
{
	public class ArtistTypeLabelViewModel
	{
		public ArtistTypeLabelViewModel(ArtistType artistType)
		{
			ArtistType = artistType;
		}

		public ArtistType ArtistType { get; set; }
	}
}