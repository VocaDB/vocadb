using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using NLog;
using VocaDb.Web.Code;

namespace VocaDb.Web.Helpers {

	public static class WebHelper {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// User agent strings for which hits won't be counted
		/// </summary>
		private static readonly string[] forbiddenUserAgents = {
			"Googlebot", "bingbot"
		};

		public static string GetRealHost(HttpRequestBase request) {

			return CfHelper.GetRealIp(request);

		}

		public static bool IsLocalhost(string hostname) {

			if (string.IsNullOrEmpty(hostname))
				return false;

			var localhosts = new[] { "localhost", "127.0.0.1", "::1" };
			return localhosts.Contains(hostname);

		}

		public static bool IsSSL(HttpRequest request) {
			return request != null && request.Url.Scheme == Uri.UriSchemeHttps;
		}

		public static bool IsSSL(HttpRequestBase request) {
			return request != null && request.Url != null && request.Url.Scheme == Uri.UriSchemeHttps;
		}

		public static bool IsSSL(HttpRequestMessage request) {
			return request != null && request.RequestUri != null && request.RequestUri.Scheme == Uri.UriSchemeHttps;
		}

		/// <summary>
		/// Checks whether the request should be counted as a valid hit (view) 
		/// for an entry.
		/// 
		/// Bots and blank user agents are excluded.
		/// </summary>
		/// <param name="request">HTTP request. Cannot be null.</param>
		/// <returns>True if the request should be counted.</returns>
		public static bool IsValidHit(HttpRequestBase request) {

			var ua = request.UserAgent;

			if (string.IsNullOrEmpty(ua)) {
				log.Warn(ErrorLogger.RequestInfo("Blank user agent from", request));
				return false;
			}

			return !forbiddenUserAgents.Any(ua.Contains);

		}

		public static void VerifyUserAgent(HttpRequestBase request) {

			var ua = request.UserAgent;
			if (string.IsNullOrEmpty(ua)) {
				log.Warn(ErrorLogger.RequestInfo("Blank user agent from", request));
				//throw new NotAllowedException();
			}

		}

	}

}