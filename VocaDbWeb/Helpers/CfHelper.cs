using System.Web;
using VocaDb.Model;

namespace VocaDb.Web.Helpers {

	/// <summary>
	/// CloudFlare-related helpers
	/// </summary>
	public static class CfHelper {

		public static string GetRealIp(HttpRequestBase request) {

			ParamIs.NotNull(() => request);

			if (!string.IsNullOrEmpty(request.Headers["CF-Connecting-IP"]))
				return request.Headers["CF-Connecting-IP"];

			return request.UserHostAddress;

		}

	}

}
