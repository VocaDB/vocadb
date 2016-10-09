
namespace VocaDb.Web.Code.Exceptions {

	public class HttpBadRequestException : HttpStatusCodeException {

		public HttpBadRequestException(string reason = null) : 
			base(System.Net.HttpStatusCode.BadRequest, reason) { }

	}

}