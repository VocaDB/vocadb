#nullable disable

using System;
using System.Net;
using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using NLog;

namespace VocaDb.Web.Code
{
	public static class ErrorLogger
	{
		public const int Code_BadRequest = (int)HttpStatusCode.BadRequest;
		public const int Code_Forbidden = (int)HttpStatusCode.Forbidden;
		public const int Code_NotFound = (int)HttpStatusCode.NotFound;
		public const int Code_InternalServerError = (int)HttpStatusCode.InternalServerError;

		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

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
			if (string.IsNullOrEmpty(msg))
				s_log.Log(level ?? LogLevel.Warn, RequestInfo($"HTTP error code {code} for", request));
			else
				s_log.Log(level ?? LogLevel.Warn, RequestInfo($"HTTP error code {code} ({msg}) for", request));
		}

		public static void LogException(HttpRequest request, Exception ex, LogLevel level = null)
		{
			s_log.Log(level ?? LogLevel.Error, ex, RequestInfo("Exception for", request));
		}

		public static void LogMessage(HttpRequest request, string msg, LogLevel level = null)
		{
			s_log.Log(level ?? LogLevel.Error, RequestInfo(msg + " for", request));
		}

		public static string RequestInfo(string msg, HttpRequest request)
		{
			var userHostAddress = request.HttpContext.Connection.RemoteIpAddress;
			var userHostName = request.GetTypedHeaders().Host;
			var httpMethod = request.Method;
			var pathAndQuery = request.GetEncodedPathAndQuery();
			var userAgent = request.Headers[HeaderNames.UserAgent];
			var urlReferrer = request.GetTypedHeaders().Referer;
			return $"{msg} '{userHostAddress}' [{userHostName}], URL {httpMethod} '{pathAndQuery}', UA '{userAgent}', referrer '{urlReferrer}'";
		}
	}
}