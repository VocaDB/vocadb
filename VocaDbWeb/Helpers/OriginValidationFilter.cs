using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class OriginValidationFilter : IActionFilter
{
	public void OnActionExecuting(ActionExecutingContext context)
	{
		var requestOrigin = context.HttpContext.Request.Headers["Origin"].FirstOrDefault();
		var allowedOrigins = new[] { "https://vocadb.net", "https://touhoudb.com", "https://utaitedb.net", "http://localhost:56401", "https://vocadb.vercel.app" };

		if (!allowedOrigins.Contains(requestOrigin))
		{
			context.Result = new BadRequestResult();
		}
	}

	public void OnActionExecuted(ActionExecutedContext context)
	{
		// No action needed after the action has executed
	}
}