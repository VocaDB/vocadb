using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code.WebApi
{
	// Code from: https://github.com/aspnet/AspNetWebStack/blob/ba26cfbfbf958d548e4c0a96e853250f13450dc6/src/System.Web.Mvc/HttpVerbs.cs
	[Flags]
	public enum HttpVerbs
	{
		Get = 1 << 0,
		Post = 1 << 1,
		Put = 1 << 2,
		Delete = 1 << 3,
		Head = 1 << 4,
		Patch = 1 << 5,
		Options = 1 << 6,
	}

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
				SupportsCredentials = true
			};

			// Verbs are case sensitive
			if (verbs.HasFlag(HttpVerbs.Get))
				_policy.Methods.Add("GET");
			if (verbs.HasFlag(HttpVerbs.Post))
				_policy.Methods.Add("POST");
			if (verbs.HasFlag(HttpVerbs.Put))
				_policy.Methods.Add("PUT");
			if (verbs.HasFlag(HttpVerbs.Options))
				_policy.Methods.Add("OPTIONS");

			var origins = AppConfig.AllowedCorsOrigins;
			if (!string.IsNullOrEmpty(origins))
			{
				if (origins != "*")
					Array.ForEach(origins.Split(','), _policy.Origins.Add);
			}
		}

		public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName) => Task.FromResult(_policy);
	}
}