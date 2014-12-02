using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VocaDb.Model;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code {

	/// <summary>
	/// JSON result using the JSON.net library instead of standard .NET.
	/// </summary>
	public class JsonNetResult : JsonResult {

		public JsonNetResult() {
			LowercaseNames = true;
		}

		/// Serializes all property names as camelCase, starting with a lowercase letter 
		/// (as opposed to the standard behavior of keeping a capital letter).
		public bool LowercaseNames { get; set; }

		public override void ExecuteResult(ControllerContext context) {

			ParamIs.NotNull(() => context);

			if (context.HttpContext == null || context.HttpContext.Request == null)
				throw new InvalidOperationException("Request couldn't be accessed.");

			/*if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
				String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)) {
				throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
			}*/

			var response = context.HttpContext.Response;

			response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
			if (ContentEncoding != null) {
				response.ContentEncoding = ContentEncoding;
			}

			if (Data != null) {

				var json = JsonHelpers.Serialize(Data, LowercaseNames);
				response.Write(json);

			}

		}
	}

}