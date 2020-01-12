using System.Web;
using System.Web.Optimization;

namespace VocaDb.Web.Helpers {

	public static class LaravelMixHelper {

		public static IHtmlString RenderScripts(params string[] paths) => Scripts.Render(paths);

		public static IHtmlString RenderStyles(params string[] paths) => Styles.Render(paths);

	}

}