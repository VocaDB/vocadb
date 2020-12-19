#nullable disable

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using NHibernate;
using NLog;

namespace VocaDb.Web.Code.WebApi
{
	public class ObjectNotFoundExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		public override void OnException(HttpActionExecutedContext context)
		{
			var ex = context.Exception as ObjectNotFoundException;

			if (ex == null)
				return;

			var msg = $"Object not found: {ex.EntityName}#{ex.Identifier}";

			_log.Warn(msg);

			var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
			{
				Content = new StringContent(msg),
				ReasonPhrase = msg
			};

			throw new HttpResponseException(resp);
		}
	}
}