using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace VocaDb.Web.Helpers {

	public static class ApiHelper {

		public static void ThrowHttpStatusCodeResult(HttpStatusCode statusCode, string message) {
			throw new HttpResponseException(new HttpResponseMessage(statusCode) { Content = new StringContent(message) });
		}

	}

}