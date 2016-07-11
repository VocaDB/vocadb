namespace VocaDb.Model.Service.Search {

	public class AdvancedSearchFilter {

		public AdvancedFilterType FilterType { get; set; }

		public bool Negate { get; set; }

		public string Param { get; set; }

	}

}
