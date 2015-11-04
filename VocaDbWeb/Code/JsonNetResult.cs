using System;
using System.Web.Mvc;
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