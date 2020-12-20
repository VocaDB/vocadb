#nullable disable

using System.Linq;

namespace VocaDb.Web.Helpers
{
	// TODO: implement
	public static class WebHelper
	{
		public static bool IsLocalhost(string hostname)
		{
			if (string.IsNullOrEmpty(hostname))
				return false;

			var localhosts = new[] { "localhost", "127.0.0.1", "::1" };
			return localhosts.Contains(hostname);
		}
	}
}