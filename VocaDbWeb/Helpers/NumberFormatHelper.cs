using System.Globalization;

namespace VocaDb.Web.Helpers {

	public static class NumberFormatHelper {
	
		private static readonly NumberFormatInfo dotNumberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
		
		public static string DecimalDot(double val) {
			return val.ToString(dotNumberFormatInfo);
		}
	
	}

}