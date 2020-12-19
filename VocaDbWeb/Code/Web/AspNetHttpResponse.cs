#nullable disable

using System;
using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetHttpResponse : IHttpResponse
	{
		public AspNetHttpResponse(HttpResponseBase response)
		{
			this._response = response;
		}

		public AspNetHttpResponse(HttpResponse response)
		{
			this._response = new HttpResponseWrapper(response);
		}

		private readonly HttpResponseBase _response;

		public void AddCookie(string name, string value, DateTime expires)
		{
			_response.AppendCookie(new HttpCookie(name, value) { Expires = expires });
		}
	}
}