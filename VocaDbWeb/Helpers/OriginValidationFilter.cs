using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class OriginValidationFilter : IActionFilter
{
	public void OnActionExecuting(ActionExecutingContext context)
	{
		var requestOrigin = context.HttpContext.Request.Headers["Origin"].First();
		// TODO: Make this list configurable
		var allowedOrigins = new[] { "https://vocadb.net", "https://touhoudb.com", "https://utaitedb.net", "http://localhost:56401", "https://vocadb.vercel.app", "https://beta.vocadb.net", "http://localhost:5173" };

		// TODO: Don't allow a null origin (Breaking Change)
		if (requestOrigin != null && !allowedOrigins.Contains(requestOrigin))
		{
			context.Result = new BadRequestResult();
		}
	}

	public void OnActionExecuted(ActionExecutedContext context)
	{
		// No action needed after the action has executed
	}
}