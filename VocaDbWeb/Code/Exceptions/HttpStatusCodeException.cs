using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace VocaDb.Web.Code.Exceptions
{

	public class HttpStatusCodeException : HttpResponseException
	{

		public HttpStatusCodeException(HttpStatusCode statusCode, string reason = null) :
			base(new HttpResponseMessage(statusCode)
			{
				Content = reason != null ? new StringContent(reason) : null
			})
		{
		}

	}

}