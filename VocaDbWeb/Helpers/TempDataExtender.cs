using System.Web.Mvc;

namespace VocaDb.Web.Helpers {

	public static class TempDataExtender {

		private const string errorKey = "ErrorMessage";
		private const string statusKey = "StatusMessage";
		private const string successKey = "SuccessMessage";
		private const string warnKey = "WarnMessage";

		private static string Get(TempDataDictionary temp, string key) {
			var msg = temp[key];
			return (msg != null ? msg.ToString() : string.Empty);
		}

		private static void Set(TempDataDictionary temp, string key, string val) {
			temp[key] = val;
		}

		public static void SetErrorMessage(this TempDataDictionary temp, string val) {
			Set(temp, errorKey, val);
		}

		public static string ErrorMessage(this TempDataDictionary temp) {
			return Get(temp, errorKey);
		}

		public static void SetStatusMessage(this TempDataDictionary temp, string val) {
			Set(temp, statusKey, val);
		}

		public static string StatusMessage(this TempDataDictionary temp) {
			return Get(temp, statusKey);
		}

		public static void SetSuccessMessage(this TempDataDictionary temp, string val) {
			Set(temp, successKey, val);
		}

		public static string SuccessMessage(this TempDataDictionary temp) {
			return Get(temp, successKey);
		}

		public static void SetWarnMessage(this TempDataDictionary temp, string val) {
			Set(temp, warnKey, val);
		}

		public static string WarnMessage(this TempDataDictionary temp) {
			return Get(temp, warnKey);
		}

	}

}