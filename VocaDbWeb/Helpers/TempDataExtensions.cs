#nullable disable

using System.Web.Mvc;

namespace VocaDb.Web.Helpers
{
	public static class TempDataExtensions
	{
		private const string ErrorKey = "ErrorMessage";
		private const string StatusKey = "StatusMessage";
		private const string SuccessKey = "SuccessMessage";
		private const string WarnKey = "WarnMessage";

		private static string Get(TempDataDictionary temp, string key)
		{
			var msg = temp[key];
			return (msg != null ? msg.ToString() : string.Empty);
		}

		private static void Set(TempDataDictionary temp, string key, string val)
		{
			temp[key] = val;
		}

		public static void SetErrorMessage(this TempDataDictionary temp, string val)
		{
			Set(temp, ErrorKey, val);
		}

		public static string ErrorMessage(this TempDataDictionary temp)
		{
			return Get(temp, ErrorKey);
		}

		public static void SetStatusMessage(this TempDataDictionary temp, string val)
		{
			Set(temp, StatusKey, val);
		}

		public static string StatusMessage(this TempDataDictionary temp)
		{
			return Get(temp, StatusKey);
		}

		public static void SetSuccessMessage(this TempDataDictionary temp, string val)
		{
			Set(temp, SuccessKey, val);
		}

		public static string SuccessMessage(this TempDataDictionary temp)
		{
			return Get(temp, SuccessKey);
		}

		public static void SetWarnMessage(this TempDataDictionary temp, string val)
		{
			Set(temp, WarnKey, val);
		}

		public static string WarnMessage(this TempDataDictionary temp)
		{
			return Get(temp, WarnKey);
		}
	}
}