#nullable disable

using System.Linq;
using Microsoft.AspNetCore.Http;

namespace VocaDb.Web.Helpers
{
	// TODO: implement
	public static class WebHelper
	{
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