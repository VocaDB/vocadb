#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Web.Helpers
{
	// TODO: implement
	/// <summary>
	/// Represents a helper class for rendering link and script elements.
	/// </summary>
	public class LaravelMixHelper
	{
		private readonly IHttpContext _context;

		public LaravelMixHelper(IHttpContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Gets the path to a <see href="https://laravel.com/docs/5.8/mix">versioned Mix file</see>.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <returns>The path to a versioned Mix file.</returns>
		public string GetPathToVersionedMixFile(string path)
		{
			path = path.Replace("~/", "/");
			var manifestPath = _context.ServerPathMapper.MapPath("~/mix-manifest.json");
			var json = File.ReadAllText(manifestPath);
			var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			return dict.TryGetValue(path, out var ret) ? ret : throw new Exception($"Unable to locate Mix file: {path}");
		}
	}
}