using System;

namespace VocaDb.Model.Helpers
{
	public static class UrlValidator
	{
		public static bool IsValid(string urlString)
		{
			if (string.IsNullOrWhiteSpace(urlString))
				return false;

			try
			{
				new Uri(urlString, UriKind.RelativeOrAbsolute);
				return true;
			}
			catch (UriFormatException)
			{
				return false;
			}
		}
	}
}
