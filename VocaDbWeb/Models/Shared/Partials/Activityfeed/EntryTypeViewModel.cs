using VocaDb.Model.DataContracts.Activityfeed;

namespace VocaDb.Web.Models.Shared.Partials.Activityfeed {

	public class EntryTypeViewModel {

		public EntryTypeViewModel(ActivityEntryForApiContract entry) {
			Entry = entry;
		}

		public ActivityEntryForApiContract Entry { get; set; }

	}

}