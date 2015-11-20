using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagSelectionContract {

		public TagSelectionContract() { }

		public TagSelectionContract(Tag tag, bool selected) {
			TagName = tag.Name;
			TagId = tag.Id;
			Selected = selected;
		}

		public bool Selected { get; set; }

		public int TagId { get; set; }

		public string TagName { get; set; }
	
	}

}
