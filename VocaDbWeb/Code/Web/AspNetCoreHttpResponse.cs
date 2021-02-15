#nullable disable

using System;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetCoreHttpResponse : IHttpResponse
	{
		private readonly HttpResponse _response;

		public AspNetCoreHttpResponse(HttpResponse response)
		{
			_response = response;
		}

		public void AddCookie(string name, string value, DateTime expires) => _response.Cookies.Append(name, value, new CookieOptions { Expires = expires });
	}
}
