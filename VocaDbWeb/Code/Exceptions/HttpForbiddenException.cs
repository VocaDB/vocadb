using System.Net;
using System.Web.Http;

namespace VocaDb.Web.Code.Exceptions {

	public class HttpForbiddenException : HttpResponseException {

		public HttpForbiddenException() : base(HttpStatusCode.Forbidden) { }

	}

}