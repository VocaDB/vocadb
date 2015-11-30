using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagSelectionContract {

		public TagSelectionContract() { }

		public TagSelectionContract(Tag tag, bool selected) {
			Tag = new TagBaseContract(tag);
			Selected = selected;
		}

		public bool Selected { get; set; }

		public TagBaseContract Tag { get; set; }
	
	}

}
