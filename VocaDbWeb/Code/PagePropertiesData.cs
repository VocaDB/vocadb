using VocaDb.Model.Helpers;

namespace VocaDb.Web.Code {

	/// <summary>
	/// Common properties for a page, to be used by the layout page.
	/// </summary>
	public class PagePropertiesData {

		public static PagePropertiesData Get(dynamic viewBag) {

			return viewBag.PageProperties ?? (viewBag.PageProperties = new PagePropertiesData());

		}

		public PagePropertiesData() {
			AddMainScripts = true;
		}

		/// <summary>
		/// Include main scripts (on most pages except the front page).
		/// </summary>
		public bool AddMainScripts { get; set; }

		/// <summary>
		/// Description appears in both the description meta field and og:description (for Facebook).
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Page title is what appears in the browser title bar.
		/// By default this is the same as Title.
		/// </summary>
		public string PageTitle { get; set; }

		/// <summary>
		/// Short page description 
		/// </summary>
		public string SummarizedDescription {
			get {
				
				if (string.IsNullOrEmpty(Description))
					return string.Empty;

				return Description
					.Trim()
					.Summarize(30, 300);

			}
		}

		/// <summary>
		/// Subtitle appears next to the main Title.
		/// </summary>
		public string Subtitle { get; set; }

		/// <summary>
		/// Title is what appears at the top of the page.
		/// </summary>
		public string Title { get; set; }

	}
}