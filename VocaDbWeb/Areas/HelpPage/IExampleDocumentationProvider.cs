using System.Web.Http.Controllers;

namespace VocaDb.Web.Areas.HelpPage {

	/// <summary>
	/// Provider for getting the documentation for the &lt;example&gt; tag.
	/// </summary>
	public interface IExampleDocumentationProvider {

		/// <summary>
		/// Gets the documentation for the &lt;example&gt; tag.
		/// </summary>
		string GetExampleDocumentation(HttpActionDescriptor actionDescriptor);

	}

}
