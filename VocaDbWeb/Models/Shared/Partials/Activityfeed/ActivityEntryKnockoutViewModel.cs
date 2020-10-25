namespace VocaDb.Web.Models.Shared.Partials.Activityfeed {

	public class ActivityEntryKnockoutViewModel {

		public ActivityEntryKnockoutViewModel(string entryTypeNamesBinding, string activityFeedEventNamesBinding, string changedFieldNamesBinding, bool showDetails = false) {

			EntryTypeNamesBinding = entryTypeNamesBinding;
			ActivityFeedEventNamesBinding = activityFeedEventNamesBinding;
			ChangedFieldNamesBinding = changedFieldNamesBinding;
			ShowDetails = showDetails;

		}

		public string EntryTypeNamesBinding { get; set; }

		public string ActivityFeedEventNamesBinding { get; set; }

		public string ChangedFieldNamesBinding { get; set; }

		public bool ShowDetails { get; set; }

	}

}