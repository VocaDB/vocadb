using System;
using System.Web;

namespace VocaDb.Model.Utils
{
	/// <summary>
	/// Various URL manipulation functions.
	/// </summary>
	public static class VocaUriBuilder
	{
		private static readonly string hostAddress = RemoveTrailingSlash(AppConfig.HostAddress);

		// Path to static files root, for example https://static.vocadb.net. Possible trailing slash is removed.
		private static readonly string staticResourceBase = RemoveTrailingSlash(AppConfig.StaticContentHost);

		/// <summary>
		/// Returns an absolute URL when the URL is known to be relative.
		/// </summary>
		/// <param name="relative"></param>
		/// <returns></returns>
		public static string Absolute(string relative)
		{
			return MergeUrls_BaseNoTrailingSlash(HostAddress, relative);
		}

		/// <summary>
		/// Returns an absolute URL based on an URL that's either absolute or relative.
		/// If the URL is absolute it will be preserved. Relative URL will be made absolute.
		/// </summary>
		/// <param name="relativeOrAbsolute"></param>
		/// <returns></returns>
		public static string AbsoluteFromUnknown(string relativeOrAbsolute, bool preserveAbsolute)
		{
			Uri uri;
			if (Uri.TryCreate(relativeOrAbsolute, UriKind.RelativeOrAbsolute, out uri))
			{
				if (uri.IsAbsoluteUri)
					return preserveAbsolute ? relativeOrAbsolute : Absolute(Relative(relativeOrAbsolute)); // URL is absolute, replace it with main site URL or preserve original.
				else
					return Absolute(relativeOrAbsolute); // URL is relative, make it absolute
			}
			else
				return relativeOrAbsolute;
		}

		/// <summary>
		/// Creates a full, absolute uri which includes the domain and scheme.
		/// </summary>
		/// <param name="relative">Relative address, for example /User/Profile/Test</param>
		/// <returns>Absolute address, for example https://vocadb.net/User/Profile/Test </returns>
		public static Uri CreateAbsolute(string relative)
		{
			return new Uri(new Uri(HostAddress), relative);
		}

		/// <summary>
		/// Gets the host address including scheme, for example https://vocadb.net.
		/// Does not include the trailing slash.
		/// </summary>
		public static string HostAddress => hostAddress;

		[Obsolete]
		public static string MakeSSL(string relative) => Absolute(relative);

		private static string MergeUrls_BaseNoTrailingSlash(string baseUrl, string relative)
		{
			if (relative.StartsWith("/"))
				return string.Format("{0}{1}", baseUrl, relative);
			else
				return string.Format("{0}/{1}", baseUrl, relative);
		}

		public static string MergeUrls(string baseUrl, string relative)
		{
			if (baseUrl.EndsWith("/"))
			{
				if (relative.StartsWith("/"))
					return string.Format("{0}{1}", baseUrl.Substring(0, baseUrl.Length - 1), relative);
				else
					return string.Format("{0}{1}", baseUrl, relative);
			}
			else
			{
				return MergeUrls_BaseNoTrailingSlash(baseUrl, relative);
			}
		}

		public static string RemoveTrailingSlash(string url)
		{
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
		private static string Relative(string relativeOrAbsolute)
		{
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
		public static string StaticResource(string relative)
		{
			return MergeUrls_BaseNoTrailingSlash(staticResourceBase, relative);
		}
	}
}
