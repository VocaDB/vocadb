using System.Text;

namespace VocaDb.Web.Helpers
{
	public static class BpmUtils
	{
		public static string FormatFromMilliBpm(int? minMilliBpm, int? maxMilliBpm)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append(minMilliBpm / 1000M);

			if (maxMilliBpm is not null && maxMilliBpm > minMilliBpm)
				stringBuilder.Append(" - ").Append(maxMilliBpm / 1000M);

			return stringBuilder.ToString();
		}
	}
}
