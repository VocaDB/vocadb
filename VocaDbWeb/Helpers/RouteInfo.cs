using System;
using System.Collections;
using System.Web;
using System.Web.Routing;

namespace VocaDb.Web.Helpers {

	public class RouteInfo {
		public RouteData RouteData { get; private set; }

		public RouteInfo(RouteData data) {
			RouteData = data;
		}

		public RouteInfo(Uri uri, string applicationPath, HttpContextBase context) {
			RouteData = RouteTable.Routes.GetRouteData(new InternalHttpContext(uri, applicationPath, context));
		}

		private class InternalHttpContext : HttpContextBase {

			private readonly HttpContextBase context;
			private readonly HttpRequestBase _request;

			public InternalHttpContext(Uri uri, string applicationPath, HttpContextBase contextBase) {
				_request = new InternalRequestContext(uri, applicationPath, contextBase.Request);
				context = contextBase;
			}

			public override IDictionary Items {
				get { return context.Items; }
			}

			public override HttpRequestBase Request { get { return _request; } }
		}

		private class InternalRequestContext : HttpRequestBase {

			private readonly string applicationPath;
			private readonly HttpRequestBase request;
			private readonly string _appRelativePath;
			private readonly string _pathInfo;

			public InternalRequestContext(Uri uri, string applicationPath, HttpRequestBase request) {

				this.applicationPath = applicationPath;
				_pathInfo = uri.Query;

				/*if (String.IsNullOrEmpty(applicationPath) || !uri.AbsolutePath.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
					_appRelativePath = uri.AbsolutePath.Substring(applicationPath.Length);
				else
					_appRelativePath = uri.AbsolutePath;*/
				_appRelativePath = uri.AbsolutePath;
				this.request = request;
			}

			public override string AppRelativeCurrentExecutionFilePath { get { return String.Concat("~", _appRelativePath); } }
			public override string PathInfo { get { return _pathInfo; } }

			public override string ApplicationPath {
				get { return applicationPath; }
			}

			public override string HttpMethod {
				get {
					return request.HttpMethod;
				}
			}

		}
	}  

}