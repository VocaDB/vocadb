#nullable disable

using System.Collections.Generic;
using System.Linq;
using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using NLog;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Web.Code;

namespace VocaDb.Web.Helpers
{
	// TODO: implement
	public static class WebHelper
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// User agent strings for which hits won't be counted
		/// </summary>
		private static readonly string[] s_forbiddenUserAgents = {
			"Googlebot", "bingbot"
		};

		public static IEnumerable<OptionalCultureCode> GetUserLanguageCodes(HttpRequest request) => request.GetTypedHeaders().AcceptLanguage?.Select(l => new OptionalCultureCode(l.ToString().Split(';')[0])); // en-US;q=0.8 -> en-US

		/// <summary>
		/// Gets the user interface culture name from the current request, as specified by the client.
		/// The first culture, if any, that matches one of the available user interface languages is returned.
		/// </summary>
		/// <param name="request">HTTP request.</param>
		/// <returns>Culture name. Empty if not matched.</returns>
		public static string GetInterfaceCultureName(HttpRequest request)
		{
			if (request.GetTypedHeaders().AcceptLanguage == null || !request.GetTypedHeaders().AcceptLanguage.Any())
				return string.Empty;

			return GetUserLanguageCodes(request)
				.Select(l => l.GetCultureInfoSafe())
				.Where(l => l != null && InterfaceLanguage.IsValidUserInterfaceCulture(l))
				.Select(l => l.Name)
				.FirstOrDefault() ?? string.Empty;
		}

		public static string GetRealHost(HttpRequest request) => request.HttpContext.Connection.RemoteIpAddress.ToString();

		public static bool IsLocalhost(string hostname)
		{
			if (string.IsNullOrEmpty(hostname))
				return false;

			var localhosts = new[] { "localhost", "127.0.0.1", "::1" };
			return localhosts.Contains(hostname);
		}

		/// <summary>
		/// Checks whether the request should be counted as a valid hit (view) 
		/// for an entry.
		/// 
		/// Bots and blank user agents are excluded.
		/// </summary>
		/// <param name="request">HTTP request. Cannot be null.</param>
		/// <returns>True if the request should be counted.</returns>
		public static bool IsValidHit(HttpRequest request)
		{
			var ua = request.Headers[HeaderNames.UserAgent];

			if (string.IsNullOrEmpty(ua))
			{
				s_log.Warn(ErrorLogger.RequestInfo("Blank user agent from", request));
				return false;
			}

			return !s_forbiddenUserAgents.Any(ua.Contains);
		}

		public static void VerifyUserAgent(HttpRequest request)
		{
			var ua = request.Headers[HeaderNames.UserAgent];
			if (string.IsNullOrEmpty(ua))
			{
				s_log.Warn(ErrorLogger.RequestInfo("Blank user agent from", request));
				//throw new NotAllowedException();
			}
		}
	}
}