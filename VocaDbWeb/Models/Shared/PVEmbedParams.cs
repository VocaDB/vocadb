using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Models.Shared {

	/// <summary>
	/// Custom PV embedding parameters
	/// </summary>
	public class PVEmbedParams {

		/// <summary>
		/// Id of the created element (usually an iframe), if supported.
		/// </summary>
		public string ElementId { get; set; }

		/// <summary>
		/// Include API (only for Youtube at the moment).
		/// </summary>
		public bool EnableScriptAccess { get; set; }

		public PVContract PV { get; set; }

	}

}