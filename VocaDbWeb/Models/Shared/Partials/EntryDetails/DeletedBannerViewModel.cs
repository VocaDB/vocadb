using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.EntryDetails {

	public class DeletedBannerViewModel {

		public DeletedBannerViewModel(IEntryBase mergedTo) {
			MergedTo = mergedTo;
		}

		public IEntryBase MergedTo { get; set; }

	}

}