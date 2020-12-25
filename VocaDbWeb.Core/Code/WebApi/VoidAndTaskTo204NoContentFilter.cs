// Code from: https://stackoverflow.com/questions/50325969/how-do-i-make-an-asp-net-core-void-task-action-method-return-204-no-content/60563876#60563876

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VocaDb.Web.Code.WebApi
{
	/// <summary>
	/// A filter that transforms http status code 200 OK to 204 No Content for controller actions that return nothing,
	/// i.e. <see cref="System.Void"/> or <see cref="Task"/>.
	/// </summary>
	internal class VoidAndTaskTo204NoContentFilter : IResultFilter
	{
		/// <inheritdoc/>
		public void OnResultExecuting(ResultExecutingContext context)
		{
			if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
			{
				var returnType = actionDescriptor.MethodInfo.ReturnType;
				if (returnType == typeof(void) || returnType == typeof(Task))
					context.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
			}
		}

		/// <inheritdoc/>
		public void OnResultExecuted(ResultExecutedContext context)
		{
		}
	}
}
