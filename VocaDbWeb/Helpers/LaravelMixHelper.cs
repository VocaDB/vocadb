using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using Newtonsoft.Json;

namespace VocaDb.Web.Helpers {

	/// <summary>
	/// Represents a helper class for rendering link and script elements.
	/// </summary>
	public static class LaravelMixHelper {

		/// <summary>
		/// Gets the path to a <see href="https://laravel.com/docs/5.8/mix">versioned Mix file</see>.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <returns>The path to a versioned Mix file.</returns>
		private static string GetPathToVersionedMixFile(string path) {

			var manifestPath = HttpContext.Current.Server.MapPath("~/mix-manifest.json");
			var json = File.ReadAllText(manifestPath);
			var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			return dict.TryGetValue(path, out var ret) ? ret : throw new Exception($"Unable to locate Mix file: {path}");
		}

		private static string GetPath(string virtualPath) => VirtualPathUtility.ToAbsolute(virtualPath);

		/// <summary>
		/// Renders script tags for the following paths.
		/// </summary>
		/// <param name="paths">Set of virtual paths for which to generate script tags.</param>
		/// <returns>The HTML string containing the script tag or tags.</returns>
		public static IHtmlString RenderScripts(params string[] paths) => Scripts.Render(paths.Select(p => GetPathToVersionedMixFile(GetPath(p))).ToArray());

		/// <summary>
		/// Renders link tags for a set of paths.
		/// </summary>
		/// <param name="paths">Set of virtual paths for which to generate link tags.</param>
		/// <returns>A HTML string containing the link tag or tags.</returns>
		public static IHtmlString RenderStyles(params string[] paths) => Styles.Render(paths.Select(p => GetPathToVersionedMixFile(GetPath(p))).ToArray());
	}
}