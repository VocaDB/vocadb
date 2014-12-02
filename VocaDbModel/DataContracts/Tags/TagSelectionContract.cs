namespace VocaDb.Model.DataContracts.Tags {

	public class TagSelectionContract {

		public TagSelectionContract() { }

		public TagSelectionContract(string tagName, bool selected) {
			TagName = tagName;
			Selected = selected;
		}

		public bool Selected { get; set; }

		public string TagName { get; set; }
	
	}

}
