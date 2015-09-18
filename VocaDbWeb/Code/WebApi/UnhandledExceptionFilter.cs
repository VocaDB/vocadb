using System.Web.Http.Filters;
using NLog;

namespace VocaDb.Web.Code.WebApi {

	public class UnhandledExceptionFilter : ExceptionFilterAttribute {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override void OnException(HttpActionExecutedContext actionExecutedContext) {

			log.Error(actionExecutedContext.Exception, "Exception raised by web API");

			base.OnException(actionExecutedContext);

		}

	}

}