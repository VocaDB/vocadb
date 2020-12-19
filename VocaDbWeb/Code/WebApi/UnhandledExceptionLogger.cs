#nullable disable

using System;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using NHibernate;
using NLog;

namespace VocaDb.Web.Code.WebApi
{
	public class UnhandledExceptionLogger : ExceptionLogger
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		public override bool ShouldLog(ExceptionLoggerContext context)
		{
			var x = context.Exception;
			return x != null && !(x is TaskCanceledException) && !(x is ObjectNotFoundException);
		}


		public override void Log(ExceptionLoggerContext context)
		{
			if (!ShouldLog(context))
				return;

			s_log.Error(context.Exception, "Exception raised by web API");
		}
	}
}