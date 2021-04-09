using System.Globalization;

namespace VocaDb.Web.Helpers
{
	public static class NumberFormatHelper
	{
		private static readonly NumberFormatInfo s_dotNumberFormatInfo = new() { NumberDecimalSeparator = "." };

		public static string DecimalDot(double val)
		{
			return val.ToString(s_dotNumberFormatInfo);
		}
	}
}