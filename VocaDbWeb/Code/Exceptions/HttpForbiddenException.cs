using System.Net;

namespace VocaDb.Web.Code.Exceptions
{
	public class HttpForbiddenException : HttpStatusCodeException
	{
		public HttpForbiddenException(string reason = null)
			: base(HttpStatusCode.Forbidden, reason) { }
	}
}