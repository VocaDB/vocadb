using System;
using System.Web;

namespace VocaDb.Model.Utils {

	/// <summary>
	/// Various URL manipulation functions.
	/// </summary>
	public static class VocaUriBuilder {

		private static readonly string hostAddress = RemoveTrailingSlash(AppConfig.HostAddress);
		private static readonly string hostAddressSSL = RemoveTrailingSlash(AppConfig.HostAddressSecure);

		// Path to static files root, for example http://static.vocadb.net. Possible trailing slash is removed.
		private static readonly string staticResourceBase = RemoveTrailingSlash(AppConfig.StaticContentHost);
		private static readonly string staticResourceBaseSSL = RemoveTrailingSlash(AppConfig.StaticContentHostSSL);

		private static bool IsSSL(HttpRequest request) {
			return request != null && request.Url.Scheme == Uri.UriSchemeHttps;
		}

		/// <summary>
		/// Returns an absolute URL when the URL is known to be relative.
		/// </summary>
		/// <param name="relative"></param>
		/// <param name="ssl"></param>
		/// <returns></returns>
		public static string Absolute(string relative, bool ssl) {
			
			return MergeUrls_BaseNoTrailingSlash(HostAddress(ssl), relative);

		}

		/// <summary>
		/// Returns an absolute URL based on an URL that's either absolute or relative.
		/// If the URL is absolute it will be preserved. Relative URL will be made absolute.
		/// </summary>
		/// <param name="relativeOrAbsolute"></param>
		/// <param name="ssl"></param>
		/// <returns></returns>
		public static string AbsoluteFromUnknown(string relativeOrAbsolute, bool preserveAbsolute, bool ssl) {
			
			Uri uri;
			if (Uri.TryCreate(relativeOrAbsolute, UriKind.RelativeOrAbsolute, out uri)) {
				if (uri.IsAbsoluteUri)
					return preserveAbsolute ? relativeOrAbsolute : Absolute(Relative(relativeOrAbsolute), ssl); // URL is absolute, replace it with main site URL or preserve original.
				else
					return Absolute(relativeOrAbsolute, ssl); // URL is relative, make it absolute
			} else
				return relativeOrAbsolute;

		}

		/// <summary>
		/// Creates a full, absolute uri which includes the domain and scheme.
		/// </summary>
		/// <param name="relative">Relative address, for example /User/Profile/Test</param>
		/// <returns>Absolute address, for example http://vocadb.net/User/Profile/Test </returns>
		public static Uri CreateAbsolute(string relative) {

			return new Uri(new Uri(AppConfig.HostAddress), relative);

		}

		/// <summary>
		/// Gets the host address including scheme, for example http://vocadb.net.
		/// Does not include the trailing slash.
		/// </summary>
		/// <param name="ssl">SSL URL.</param>
		public static string HostAddress(bool ssl) {
			return ssl ? hostAddressSSL : hostAddress;
		}

		public static string HostAddress() {
			return HostAddress(IsSSL(HttpContext.Current.Request));
		}

		public static string MakeSSL(string relative) {
			
			return Absolute(relative, true);

		}

		private static string MergeUrls_BaseNoTrailingSlash(string baseUrl, string relative) {
			
			if (relative.StartsWith("/"))
				return string.Format("{0}{1}", baseUrl, relative);
			else
				return string.Format("{0}/{1}", baseUrl, relative);

		}

		public static string MergeUrls(string baseUrl, string relative) {

			if (baseUrl.EndsWith("/")) {

				if (relative.StartsWith("/"))
					return string.Format("{0}{1}", baseUrl.Substring(0, baseUrl.Length - 1), relative);
				else
					return string.Format("{0}{1}", baseUrl, relative);

			} else {
				return MergeUrls_BaseNoTrailingSlash(baseUrl, relative);
			}

		}

		public static string RemoveTrailingSlash(string url) {

			if (string.IsNullOrEmpty(url))
				return url;

			return url.EndsWith("/") ? url.Substring(0, AppConfig.StaticContentHost.Length - 1) : url;

		}

		/// <summary>
		/// Covers an absolute URL to relative, or returns the relative URL intact.
		/// TODO: this probably doesn't work if the site is in a subfolder such as http://example/vocadb
		/// </summary>
		/// <param name="relativeOrAbsolute">URL that might be either relative or absolute. For example http://vocadb.net/ or /</param>
		/// <returns>Relative portion of the URL, for example /</returns>
		private static string Relative(string relativeOrAbsolute) {
			
			Uri uri;
			if (Uri.TryCreate(relativeOrAbsolute, UriKind.RelativeOrAbsolute, out uri))			
				return uri.IsAbsoluteUri ? uri.PathAndQuery : relativeOrAbsolute;
			else
				return relativeOrAbsolute;

		}

		/// <summary>
		/// Returns a path to a resource in the static VocaDB domain (static.vocadb.net).
		/// </summary>
		/// <param name="relative">Relative URL, for example /banners/rvocaloid.png</param>
		/// <returns>
		/// Full path to that static resource, for example http://static.vocadb.net/banners/rvocaloid.png
		/// </returns>
		public static string StaticResource(string relative) {
			return StaticResource(relative, false);
		}

		/// <summary>
		/// Returns a path to a resource in the static VocaDB domain (static.vocadb.net).
		/// </summary>
		/// <param name="relative">Relative URL, for example /banners/rvocaloid.png</param>
		/// <returns>
		/// Full path to that static resource, for example http://static.vocadb.net/banners/rvocaloid.png
		/// </returns>
		public static string StaticResource(string relative, bool ssl) {
			return MergeUrls_BaseNoTrailingSlash(ssl ? staticResourceBaseSSL : staticResourceBase, relative);
		}

	}
}
