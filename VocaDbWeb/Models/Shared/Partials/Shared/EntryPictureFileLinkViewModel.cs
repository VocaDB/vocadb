using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.Shared {

	public class EntryPictureFileLinkViewModel {

		public EntryPictureFileLinkViewModel(IEntryPictureFile entryPictureFile) {
			EntryPictureFile = entryPictureFile;
		}

		public IEntryPictureFile EntryPictureFile { get; set; }

	}

}