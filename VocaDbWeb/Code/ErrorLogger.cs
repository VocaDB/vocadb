#nullable disable

using System;
using System.Net;
using System.Web;
using NLog;

namespace VocaDb.Web.Code
{
	public static class ErrorLogger
	{
		public const int Code_BadRequest = (int)HttpStatusCode.BadRequest;
		public const int Code_Forbidden = (int)HttpStatusCode.Forbidden;
		public const int Code_NotFound = (int)HttpStatusCode.NotFound;
		public const int Code_InternalServerError = (int)HttpStatusCode.InternalServerError;

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public static void LogHttpError(HttpRequestBase request, int code, string msg = null, LogLevel level = null)
		{
			if (string.IsNullOrEmpty(msg))
				log.Log(level ?? LogLevel.Warn, RequestInfo($"HTTP error code {code} for", request));
			else
				log.Log(level ?? LogLevel.Warn, RequestInfo($"HTTP error code {code} ({msg}) for", request));
		}

		/// <summary>
		/// Logs HTTP error code sent to a client.
		/// This method is mostly for client errors (status code 4xx).
		/// 
		/// Client info and error summary will be logged.
		/// Full exceptions should be logged separately using <see cref="LogException"/>.
		/// </summary>
		/// <param name="request">HTTP request. Cannot be null.</param>
		/// <param name="code">HTTP response code.</param>
		/// <param name="msg">Optional simple message, usually exception message.</param>
		/// <param name="level">Logging level, optional.</param>
		public static void LogHttpError(HttpRequest request, int code, string msg = null, LogLevel level = null)
		{
			LogHttpError(new HttpRequestWrapper(request), code, msg, level);
		}

		public static void LogException(HttpRequest request, Exception ex, LogLevel level = null)
		{
			log.Log(level ?? LogLevel.Error, ex, RequestInfo("Exception for", new HttpRequestWrapper(request)));
		}

		public static void LogMessage(HttpRequestBase request, string msg, LogLevel level = null)
		{
			log.Log(level ?? LogLevel.Error, RequestInfo(msg + " for", request));
		}

		public static void LogMessage(HttpRequest request, string msg, LogLevel level = null)
		{
			LogMessage(new HttpRequestWrapper(request), msg, level);
		}

		public static string RequestInfo(string msg, HttpRequestBase request)
		{
			return $"{msg} '{request.UserHostAddress}' [{request.UserHostName}], URL {request.HttpMethod} '{request.Unvalidated.Url.PathAndQuery}', UA '{request.UserAgent}', referrer '{request.UrlReferrer}'";
		}
	}
}