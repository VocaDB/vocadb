using System.Web.Mvc;
using System.Web.Routing;
using VocaDb.Web.Code;

namespace VocaDb.Web.App_Start {

	public static class RouteConfig {

		private const string numeric = "[0-9]+";

		public static void RegisterRoutes(RouteCollection routes) {

			// Ignored files
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("favicon.ico");

			// Invalid routes - redirects to 404
			routes.MapRoute("AlbumDetailsError", "Album/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
			routes.MapRoute("ArtistDetailsError", "Artist/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
			routes.MapRoute("SongDetailsError", "Song/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });

			// Action routes
			routes.MapRoute("Album", "Al/{id}/{friendlyName}", new { controller = "Album", action = "Details", friendlyName = UrlParameter.Optional }, new { id = numeric });
			routes.MapRoute("Artist", "Ar/{id}/{friendlyName}", new { controller = "Artist", action = "Details", friendlyName = UrlParameter.Optional }, new { id = numeric });

			// Song shortcut, for example /S/393939
			routes.MapRoute("Song", "S/{id}/{friendlyName}", new { controller = "Song", action = "Details", friendlyName = UrlParameter.Optional }, new { id = numeric });

			routes.MapRoute("SongList", "L/{id}/{slug}", new { controller = "SongList", action = "Details", slug = UrlParameter.Optional }, new { id = numeric });

			routes.MapRoute("Tag", "T/{id}/{slug}", new { controller = "Tag", action = "DetailsById", slug = UrlParameter.Optional }, new { id = numeric });

			// User profile route, for example /Profile/riipah
			routes.MapRoute("User", "Profile/{id}", new { controller = "User", action = "Profile" });

			routes.MapRoute("Discussion", "discussion/{*clientPath}", new { controller = "Discussion", action = "Index", clientPath = UrlParameter.Optional });

			// Default route
			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

	}

}