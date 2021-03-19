using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VocaDb.Model.Utils;
using VocaDb.ReMikus;

namespace VocaDb.Web.Middleware
{
	public class HandleInertiaRequests
	{
		private readonly RequestDelegate _next;

		public HandleInertiaRequests(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, LaravelMix laravelMix)
		{
			var config = AppConfig.GetGlobalLinksSection();

			// shared data
			Inertia.SharedProps = new
			{
			};

			// asset versioning
			using var md5 = MD5.Create();
			using var stream = File.OpenRead(laravelMix.ManifestPath);
			var hash = await md5.ComputeHashAsync(stream);
			Inertia.VersionSelector = () => BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

			await _next(context);
		}
	}
}
