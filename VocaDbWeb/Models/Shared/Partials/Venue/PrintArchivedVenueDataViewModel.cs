using VocaDb.Model.DataContracts.Venues;

namespace VocaDb.Web.Models.Shared.Partials.Venue {

	public class PrintArchivedVenueDataViewModel {

		public PrintArchivedVenueDataViewModel(ComparedVenueContract comparedVenues) {
			ComparedVenues = comparedVenues;
		}

		public ComparedVenueContract ComparedVenues { get; set; }

	}

}