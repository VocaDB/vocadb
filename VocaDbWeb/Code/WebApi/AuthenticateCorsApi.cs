using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Web.Mvc;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code.WebApi {

	/// <summary>
	/// CORS policy for APIs that require authentication. Origins are restricted.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class AuthenticatedCorsApi : Attribute, ICorsPolicyProvider {

		private readonly CorsPolicy _policy;

		public AuthenticatedCorsApi(HttpVerbs verbs) {

			_policy = new CorsPolicy {
				AllowAnyHeader = true,
				SupportsCredentials = true
			};

			if (verbs.HasFlag(HttpVerbs.Get)) {
				_policy.Methods.Add("get");
			}
			if (verbs.HasFlag(HttpVerbs.Post)) {
				_policy.Methods.Add("post");
			}
			if (verbs.HasFlag(HttpVerbs.Put)) {
				_policy.Methods.Add("put");
			}

			var origins = AppConfig.AllowedCorsOrigins;
			if (!string.IsNullOrEmpty(origins)) {
				Array.ForEach(origins.Split(','), _policy.Origins.Add);
			}
			
		}

		public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			return Task.FromResult(_policy);
		}

	}
}