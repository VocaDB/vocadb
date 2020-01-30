using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using Newtonsoft.Json;

namespace VocaDb.Web.Helpers {

	public static class LaravelMixHelper {

		private static string GetPathToVersionedMixFile(string path) {

			var manifestPath = HttpContext.Current.Server.MapPath("~/mix-manifest.json");
			var json = File.ReadAllText(manifestPath);
			var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			return dict.TryGetValue(path, out var ret) ? ret : throw new Exception($"Unable to locate Mix file: {path}");
		}

		private static string GetPath(string virtualPath) => VirtualPathUtility.ToAbsolute(virtualPath);

		public static IHtmlString RenderScripts(params string[] paths) => Scripts.Render(paths.Select(p => GetPathToVersionedMixFile(GetPath(p))).ToArray());

		public static IHtmlString RenderStyles(params string[] paths) => Styles.Render(paths.Select(p => GetPathToVersionedMixFile(GetPath(p))).ToArray());

	}

}