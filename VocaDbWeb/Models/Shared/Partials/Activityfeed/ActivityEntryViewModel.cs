using VocaDb.Model.DataContracts.Activityfeed;

namespace VocaDb.Web.Models.Shared.Partials.Activityfeed {

	public class ActivityEntryViewModel {

		public ActivityEntryViewModel(ActivityEntryForApiContract entry, bool showDetails = false) {
			Entry = entry;
			ShowDetails = showDetails;
		}

		public ActivityEntryForApiContract Entry { get; set; }

		public bool ShowDetails { get; set; }

	}

}