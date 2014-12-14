using System.Web;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Helpers {

	public static class VocaUrlHelper {

		public static string MergeUrls(string baseUrl, string relative) {

			return VocaUriBuilder.MergeUrls(baseUrl, relative);

		}

		/// <summary>
		/// Returns a path to a resource in the static VocaDB domain (static.vocadb.net).
		/// This method automatically identifies scheme of the current request.
		/// </summary>
		/// <param name="relative">Relative URL, for example /banners/rvocaloid.png</param>
		/// <returns>
		/// Full path to that static resource, for example http://static.vocadb.net/banners/rvocaloid.png
		/// </returns>
		public static string StaticResource(string relative) {
			return VocaUriBuilder.StaticResource(relative, WebHelper.IsSSL(HttpContext.Current.Request));
		}
	
	}

}