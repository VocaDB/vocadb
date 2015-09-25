using System;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using NLog;

namespace VocaDb.Web.Code.WebApi {

	public class UnhandledExceptionLogger : ExceptionLogger {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private bool ShouldLog(Exception x) {
			return x != null && !(x is TaskCanceledException);
		}

		public override void Log(ExceptionLoggerContext context) {

			if (!ShouldLog(context.Exception))
				return;

			log.Error(context.Exception, "Exception raised by web API");

		}

	}

}