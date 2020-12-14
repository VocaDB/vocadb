#nullable disable

using System.Web.Mvc;

namespace VocaDb.Web.Code.Filters
{
	/// <summary>
	/// Disable serving pages as text/vnd.wap.wml
	/// See http://stackoverflow.com/a/1193746 and http://stackoverflow.com/a/8947469
	/// </summary>
	public class DisableWapFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			if (filterContext.HttpContext.Response.ContentType == "text/vnd.wap.wml")
			{
				filterContext.HttpContext.Response.ContentType = "text/html";
			}
		}
	}
}