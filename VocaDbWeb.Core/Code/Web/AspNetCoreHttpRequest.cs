#nullable disable

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetCoreHttpRequest : IHttpRequest
	{
		private readonly HttpRequest request;

		public AspNetCoreHttpRequest(HttpRequest request)
		{
			this.request = request;
		}

		public IReadOnlyDictionary<string, ICookie> Cookies => request.Cookies.Keys.Distinct().ToDictionary(k => k, v => (ICookie)new ReadOnlyCookie(request.Cookies[v]));
		public NameValueCollection Form => request.HasFormContentType ? ToNameValueCollection(request.Form) : new NameValueCollection();
		// https://docs.microsoft.com/en-us/dotnet/api/system.web.httprequest.params?view=netframework-4.8
		public NameValueCollection Params => new NameValueCollection { QueryString, Form, ToNameValueCollection(request.Cookies)/* TODO: ServerVariables */ };
		public NameValueCollection QueryString => ToNameValueCollection(request.Query);
		public string UserHostAddress => request.HttpContext.Connection.RemoteIpAddress.ToString();

		private static NameValueCollection ToNameValueCollection(IEnumerable<KeyValuePair<string, string>> source)
		{
			var ret = new NameValueCollection();
			foreach (var kv in source)
				ret.Add(kv.Key, kv.Value);
			return ret;
		}

		private static NameValueCollection ToNameValueCollection(IEnumerable<KeyValuePair<string, StringValues>> source)
		{
			var ret = new NameValueCollection();
			foreach (var kv in source)
				ret.Add(kv.Key, kv.Value);
			return ret;
		}
	}
}
