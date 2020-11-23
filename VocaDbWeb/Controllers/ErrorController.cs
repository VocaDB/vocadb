using System.Web.Mvc;
using NLog;
using VocaDb.Web.Code;

namespace VocaDb.Web.Controllers
{
	public class ErrorController : ControllerBase
	{
		//
		// GET: /Error/

		public ActionResult BadRequest(bool? redirect)
		{
			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, ErrorLogger.Code_BadRequest);

			Response.StatusCode = ErrorLogger.Code_BadRequest;
			return View("BadRequest");
		}

		public ActionResult Forbidden(bool? redirect)
		{
			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, ErrorLogger.Code_Forbidden);

			Response.StatusCode = ErrorLogger.Code_Forbidden;
			return View("Forbidden");
		}

		public ActionResult Index(int? code, bool? redirect)
		{
			var realCode = code ?? ErrorLogger.Code_InternalServerError;

			if (realCode == ErrorLogger.Code_BadRequest)
				return BadRequest(redirect);

			if (realCode == ErrorLogger.Code_Forbidden)
				return Forbidden(redirect);

			if (realCode == ErrorLogger.Code_NotFound)
				return NotFound(redirect);

			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, realCode);

			Response.StatusCode = realCode;
			return View("Index");
		}

		public ActionResult IPForbidden(bool? redirect)
		{
			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, ErrorLogger.Code_Forbidden);

			// 403.6: IP address of the client has been rejected.
			Response.StatusCode = ErrorLogger.Code_Forbidden;
			Response.SubStatusCode = 6;
			return View("IPForbidden");
		}

		public ActionResult NotFound(bool? redirect)
		{
			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, ErrorLogger.Code_NotFound);

			Response.StatusCode = ErrorLogger.Code_NotFound;
			return View("NotFound");
		}
	}
}
