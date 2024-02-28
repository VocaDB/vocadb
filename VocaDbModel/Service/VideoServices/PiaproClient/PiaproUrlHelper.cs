namespace VocaDb.Model.Service.VideoServices.PiaproClient;

using System.Text.RegularExpressions;

/// <summary>
/// Helper methods for Piapro URLs.
/// </summary>
public static class PiaproUrlHelper
{
	public const string PiaproUrlRegex = @"^(?:https?://)?piapro.jp/(?:t|content)?/([\w\-]+)";

	/// <summary>
	/// Tests whether an URL is a valid Piapro content URL.
	/// </summary>
	/// <param name="url">URL to be tested. Can be null or empty.</param>
	/// <returns>True if the URL is valid, otherwise false.</returns>
	public static bool IsValidContentUrl(string url)
	{
		if (string.IsNullOrEmpty(url))
			return false;

		return Regex.IsMatch(url, PiaproUrlRegex, RegexOptions.IgnoreCase);
	}

	public static bool TryGetUrl(string str, out string url)
	{
		var match = Regex.Match(str, PiaproUrlRegex);

		if (match.Success)
		{
			url = match.Value;
			return true;
		}

		url = string.Empty;
		return false;
	}
}