using VocaDb.Model.DataContracts.Venues;

namespace VocaDb.Web.Models.Shared.Partials.Venue
{
	public class VenueLinkViewModel
	{
		public VenueLinkViewModel(VenueForApiContract venue)
		{
			Venue = venue;
		}

		public VenueForApiContract Venue { get; set; }
	}
}