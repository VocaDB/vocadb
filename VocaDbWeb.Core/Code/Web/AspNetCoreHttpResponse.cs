using System;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{

	public class AspNetCoreHttpResponse : IHttpResponse
	{

		private readonly HttpResponse response;

		public AspNetCoreHttpResponse(HttpResponse response)
		{
			this.response = response;
		}

		public void AddCookie(string name, string value, DateTime expires) => response.Cookies.Append(name, value, new CookieOptions { Expires = expires });

	}

}
