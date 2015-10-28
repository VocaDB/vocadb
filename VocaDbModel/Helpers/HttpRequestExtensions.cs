using System;
using System.Web;

namespace VocaDb.Model.Helpers {

	public static class HttpRequestExtensions {

		public static bool IsSSL(this HttpRequest request) {
			return request != null && request.Url.Scheme == Uri.UriSchemeHttps;
		}

		public static bool IsSSL(this HttpRequestBase request) {
			return request != null && request.Url != null && request.Url.Scheme == Uri.UriSchemeHttps;
		}

	}

}
