using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.Search {

	/// <summary>
	/// Common search parameters for all entry types.
	/// </summary>
	public class CommonSearchParams {

		public CommonSearchParams() {
			NameMatchMode = NameMatchMode.Auto;
		}

		public CommonSearchParams(string query, bool draftOnly, NameMatchMode nameMatchMode, bool onlyByName, bool moveExactToTop)
			: this() {

			DraftOnly = draftOnly;
			NameMatchMode = nameMatchMode;
			Query = query;
			OnlyByName = onlyByName;
			MoveExactToTop = moveExactToTop;

		}

		// TODO: replaced with EntryStatus filter.
		public bool DraftOnly { get; set; }

		public EntryStatus? EntryStatus { get; set; }

		/// <summary>
		/// Moves results that are exact matches or starting with the search term to top of the result set.
		/// This parameter is mostly used for autocompletes and is not intended to work with paging.
		/// </summary>
		public bool MoveExactToTop { get; set; }

		public NameMatchMode NameMatchMode { get; set; }

		public bool OnlyByName { get; set; }

		public string Query { get; set; }

	}

}
