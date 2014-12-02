using System.Runtime.Serialization;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Service.Paging {

	/// <summary>
	/// Common query properties for paging.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class PagingProperties {

		/// <summary>
		/// Creates paging properties based on a page number (instead of absolute entry index).
		/// </summary>
		/// <param name="page">Page number (starting from 0)</param>
		/// <param name="entriesPerPage">Number of entries per page.</param>
		/// <param name="getTotalCount">Whether to get total count.</param>
		/// <returns>Paging properties. Cannot be null.</returns>
		public static PagingProperties CreateFromPage(int page, int entriesPerPage, bool getTotalCount) {

			return new PagingProperties(page * entriesPerPage, entriesPerPage, getTotalCount);

		}

		public static PagingProperties FirstPage(int entriesPerPage, bool getTotalCount = false) {
			return new PagingProperties(0, entriesPerPage, getTotalCount);
		}

		/// <summary>
		/// Initializes paging properties.
		/// </summary>
		/// <param name="start">Index of the first entry to be returned, starting from 0.</param>
		/// <param name="maxEntries">Maximum number of entries per page.</param>
		/// <param name="getTotalCount">Whether to get the total number of entries.</param>
		public PagingProperties(int start, int maxEntries, bool getTotalCount) {
			Start = start;
			MaxEntries = maxEntries;
			GetTotalCount = getTotalCount;
		}

		/// <summary>
		/// Whether to get the total number of entries.
		/// </summary>
		[DataMember]
		public bool GetTotalCount { get; set; }

		/// <summary>
		/// Maximum number of entries per page.
		/// </summary>
		[DataMember]
		public int MaxEntries { get; set; }

		/// <summary>
		/// Index of the first entry to be returned, starting from 0.
		/// </summary>
		[DataMember]
		public int Start { get; set; }

	}
}
