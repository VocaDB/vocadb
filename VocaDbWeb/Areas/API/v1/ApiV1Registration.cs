using System.Web.Mvc;
using VocaDb.Web.Controllers;

namespace VocaDb.Web.API.v1 {

	public class ApiV1Registration : AreaRegistration {

		public override string AreaName {
            get { return "ApiV1"; }
        }
 
        public override void RegisterArea(AreaRegistrationContext context)
        {

			context.MapRoute(
				"SongApi",
				"api/v1/Song/{action}/{id}",
				new { action = "Index", controller = "SongApi", id = UrlParameter.Optional });

			/*context.MapRoute(
				"SongApiJson",
				"api/v1/Song/{action}.json",
				new { action = "Index", controller = "SongApi", format = DataFormat.Json });*/

            context.MapRoute(
                "AlbumApi",
                "api/v1/Album/{action}/{id}",
				new { action = "Index", controller = "AlbumApi", id = UrlParameter.Optional });

			context.MapRoute(
				"ArtistApi",
				"api/v1/Artist/{action}/{id}",
				new { action = "Index", controller = "ArtistApi", id = UrlParameter.Optional });

			context.MapRoute(
				"UserApi",
				"api/v1/User/{action}/{id}",
				new { action = "Index", controller = "UserApi", id = UrlParameter.Optional });

        }

	}

}