using System.Net.Http;
using System.Web.Http;

namespace VocaDb.Web.Code.Exceptions {

	public class HttpBadRequestException : HttpResponseException {

		public HttpBadRequestException(string reason = null) : 
			base(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { Content = reason != null ? new StringContent(reason) : null }) { }

	}

}