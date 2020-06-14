using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;

namespace VocaDb.Model.Domain.Web {

	public interface IHttpContext {
        System.Collections.IDictionary Items { get; }
		IHttpRequest Request { get; }
        IHttpResponse Response { get; }
        IPrincipal User { get; set; }
	}

	public interface IHttpRequest {
        IReadOnlyDictionary<string, ICookie> Cookies { get; }
        NameValueCollection Form { get; }
        NameValueCollection Params { get; }
        NameValueCollection QueryString { get; }
        string UserHostAddress { get; }
	}

    public interface IHttpResponse {
        void AddCookie(string name, string value, DateTime expires);
    }

    public interface ICookie {
        string Value { get; }
    }

	public class ReadOnlyCookie : ICookie {

		public ReadOnlyCookie(string value) {
			Value = value;
		}

		public string Value { get; }

	}
}
