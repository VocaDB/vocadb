using System.Web.Http;

namespace VocaDb.Web.Code.Exceptions {

	public class HttpBadRequestException : HttpResponseException {

		public HttpBadRequestException() : 
			base(System.Net.HttpStatusCode.BadRequest) { }

	}

}