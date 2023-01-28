#nullable disable

using System.Collections.Specialized;
using Microsoft.Extensions.Primitives;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web;

public class AspNetCoreHttpRequest : IHttpRequest
{
	private readonly HttpRequest _request;

	public AspNetCoreHttpRequest(HttpRequest request)
	{
		_request = request;
	}

	public IReadOnlyDictionary<string, ICookie> Cookies => _request.Cookies.Keys.Distinct().ToDictionary(k => k, v => (ICookie)new ReadOnlyCookie(_request.Cookies[v]));
	public NameValueCollection Form => _request.HasFormContentType ? ToNameValueCollection(_request.Form) : new NameValueCollection();
	// https://docs.microsoft.com/en-us/dotnet/api/system.web.httprequest.params?view=netframework-4.8
	public NameValueCollection Params => new NameValueCollection { QueryString, Form, ToNameValueCollection(_request.Cookies)/* TODO: ServerVariables */ };
	public NameValueCollection QueryString => ToNameValueCollection(_request.Query);
	public string UserHostAddress => _request.HttpContext.Connection.RemoteIpAddress.ToString();

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
