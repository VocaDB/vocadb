using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Web.Controllers;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.API.v1.Controllers {

	public class UserApiController : Web.Controllers.ControllerBase {

		private UserService Service {
			get { return Services.Users; }
		}

		[HttpPost]
		public ActionResult Authenticate(string username, string accesskey) {
			
			var user = Service.CheckAccessWithKey(username, accesskey, WebHelper.GetRealHost(Request));

			if (user == null) {
				//Response.StatusCode = 401;
				//return Content("Error: Username or password doesn't match.");
				// Note: can't return 401 (Unauthorized) because of forms authentication.
				// See http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/b6b0cd09-a95a-483e-8ad3-48a90d66d11c
				return HttpStatusCodeResult(HttpStatusCode.Forbidden, "Error: Username or password doesn't match.");
			} else {

				FormsAuthentication.SetAuthCookie(user.Name, true);
				return Content("OK");

			}

		}

		public ActionResult PostRating(int songId, SongVoteRating rating, string callback) {

			if (!PermissionContext.IsLoggedIn) {
				return HttpStatusCodeResult(HttpStatusCode.Forbidden, "Error: Must be logged in.");				
			}

			Service.UpdateSongRating(PermissionContext.LoggedUserId, songId, rating);
			return Object("OK", DataFormat.Auto, callback);

		}

 	}
}