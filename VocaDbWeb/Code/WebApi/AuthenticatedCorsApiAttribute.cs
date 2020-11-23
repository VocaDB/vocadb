using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Web.Mvc;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code.WebApi
{
	/// <summary>
	/// CORS policy for APIs that require authentication. Origins are restricted.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class AuthenticatedCorsApiAttribute : Attribute, ICorsPolicyProvider
	{
		private readonly CorsPolicy _policy;

		public AuthenticatedCorsApiAttribute(HttpVerbs verbs)
		{
			_policy = new CorsPolicy
			{
				AllowAnyHeader = true,
				SupportsCredentials = true
			};

			// Verbs are case sensitive
			if (verbs.HasFlag(HttpVerbs.Get))
			{
				_policy.Methods.Add("GET");
			}
			if (verbs.HasFlag(HttpVerbs.Post))
			{
				_policy.Methods.Add("POST");
			}
			if (verbs.HasFlag(HttpVerbs.Put))
			{
				_policy.Methods.Add("PUT");
			}
			if (verbs.HasFlag(HttpVerbs.Options))
			{
				_policy.Methods.Add("OPTIONS");
			}

			var origins = AppConfig.AllowedCorsOrigins;
			if (!string.IsNullOrEmpty(origins))
			{
				if (origins != "*")
				{
					Array.ForEach(origins.Split(','), _policy.Origins.Add);
				}
				else
				{
					_policy.AllowAnyOrigin = true;
				}
			}
		}

		public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(_policy);
		}
	}
}