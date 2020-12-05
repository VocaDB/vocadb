using System;
using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web {

	public class AspNetHttpResponse : IHttpResponse {

		public AspNetHttpResponse(HttpResponseBase response) {
			this.response = response;
		}

		public AspNetHttpResponse(HttpResponse response) {
			this.response = new HttpResponseWrapper(response);
		}

		private readonly HttpResponseBase response;

		public void AddCookie(string name, string value, DateTime expires) {
			response.AppendCookie(new HttpCookie(name, value) { Expires = expires });
		}

	}

}