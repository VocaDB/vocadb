#nullable disable

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web
{
	public class AspNetHttpRequest : IHttpRequest
	{
		public AspNetHttpRequest(HttpRequestBase request)
		{
			this._request = request;
		}

		public AspNetHttpRequest(HttpRequest request)
		{
			this._request = new HttpRequestWrapper(request);
		}

		private readonly HttpRequestBase _request;

		public IReadOnlyDictionary<string, ICookie> Cookies => _request.Cookies.AllKeys.Distinct().ToDictionary(c => c, c => (ICookie)new ReadOnlyCookie(_request.Cookies[c].Value));
		public NameValueCollection Form => _request.Form;
		public NameValueCollection Params => _request.Params;
		public NameValueCollection QueryString => _request.QueryString;
		public string UserHostAddress => _request.UserHostAddress;
	}
}