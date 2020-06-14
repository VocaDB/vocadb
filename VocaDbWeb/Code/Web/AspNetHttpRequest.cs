using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web {

	public class AspNetHttpRequest : IHttpRequest {

		public AspNetHttpRequest(HttpRequestBase request) {
			this.request = request;
		}

		public AspNetHttpRequest(HttpRequest request) {
			this.request = new HttpRequestWrapper(request);
		}

		private readonly HttpRequestBase request;

		public IReadOnlyDictionary<string, ICookie> Cookies => request.Cookies.Cast<HttpCookie>().ToDictionary(c => c.Name, c => (ICookie)new ReadOnlyCookie(c.Value));
		public NameValueCollection Form => request.Form;
		public NameValueCollection Params => request.Params;
		public NameValueCollection QueryString => request.QueryString;
		public string UserHostAddress => request.UserHostAddress;

	}

}