using System.Diagnostics.CodeAnalysis;

namespace VocaDb.Model.Helpers
{
	public static class StringExtensions
	{
		/*public static string FirstLine(this string str) {
			
			if (string.IsNullOrEmpty(str))
				return str;

			var newline = str.IndexOf('\n');
			return newline != -1 ? str.Substring(0, newline) : str;
		}*/

		public static string? EmptyToNull(this string str)
		{
			return str == string.Empty ? null : str;
		}

		/// <summary>
		/// Returns a string that's optimally between minimum and maximum length.
		/// Picks the first line, and if that's suitable, returns only that first line.
		/// If the string is too long it will be truncated.
		/// </summary>
		/// <param name="str">String to be parsed.</param>
		/// <param name="minLength">Minimum desired length.</param>
		/// <param name="maxLength">Maximum allowed length.</param>
		/// <returns>
		/// Summarized string. 
		/// 
		/// Length is always less then <paramref name="maxLength"/>, 
		/// and more than <paramref name="minLength"/>, 
		/// assuming <paramref name="str"/> is longer than <paramref name="minLength"/>.
		/// </returns>
		[return:NotNullIfNotNull("str"/* TODO: use nameof */)]
		public static string? Summarize(this string? str, int minLength, int maxLength)
		{
			if (string.IsNullOrEmpty(str))
				return str;

			if (str.Length < maxLength)
				return str;

			var newline = str.IndexOf('\n');

			if (newline != -1 && newline > minLength && newline < maxLength)
				return str.Substring(0, newline);

			return TruncateWithEllipsis(str, maxLength);
		}

		public static string Truncate(this string str, int length)
		{
			ParamIs.NotNull(() => str);

			return (str.Length > length ? str.Substring(0, length) : str);
		}

		/// <summary>
		/// Truncates a string, adding ellipsis (three dots) at the end if the length exceeds a specific number.
		/// </summary>
		/// <param name="str">String to be processed. Cannot be null.</param>
		/// <param name="length">Maximum length after which the string will be truncated.</param>
		/// <returns>Truncated string with three dots at the end, if the string length exceeded the specified length, otherwise the original string.</returns>
		public static string TruncateWithEllipsis(this string str, int length)
		{
			ParamIs.NotNull(() => str);

			return (str.Length > length ? $"{str.Substring(0, length)}..." : str);
		}
	}
}
