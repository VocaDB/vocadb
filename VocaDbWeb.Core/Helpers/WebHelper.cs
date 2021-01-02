#nullable disable

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Helpers
{
	// TODO: implement
	public static class WebHelper
	{
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
	}
}