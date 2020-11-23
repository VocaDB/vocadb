namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class ArtistFiltersViewModel
	{
		public ArtistFiltersViewModel(bool artistParticipationStatus)
		{
			ArtistParticipationStatus = artistParticipationStatus;
		}

		public bool ArtistParticipationStatus { get; set; }
	}
}