using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.VideoServices;

/// <summary>
/// Captures values from (partial) URL using regex and then formats a full
/// URL using those captured values.
/// </summary>
public class RegexLinkMatcher
{
	private readonly string _template;
	private readonly Regex _regex;

	/// <summary>
	/// Initializes matcher.
	/// </summary>
	/// <param name="template">URL template with ID placeholder, for example "https://twitter.com/{0}".</param>
	/// <param name="regexStr">Regex with groups for the ID, for example "^http(?:s)?://twitter\.com/(\w+)". Case will be ignored.</param>
	/// <remarks>The number of captured groups in <paramref name="regexStr"/> should match placeholders in <paramref name="template"/>.</remarks>
	public RegexLinkMatcher(string template, string regexStr)
	{
		_regex = new Regex(regexStr, RegexOptions.IgnoreCase);
		_template = template;
	}

	public string GetId(string url)
	{
		var match = _regex.Match(url);

		if (match.Groups.Count < 2)
			throw new InvalidOperationException("Regex needs a group for the ID");

		var group = match.Groups[1];
		return group.Value;
	}

	public bool IsMatch(string url) => _regex.IsMatch(url);

	public string MakeLinkFromUrl(string url) => MakeLinkFromId(GetId(url));

	public string MakeLinkFromId(string? id) => string.Format(_template, id);

	public (bool success, string? formattedUrl) GetLinkFromUrl(string url)
	{
		var success = TryGetLinkFromUrl(url, out var formattedUrl);
		return (success, formattedUrl);
	}

	public bool TryGetLinkFromUrl(string url, [NotNullWhen(true)] out string? formattedUrl)
	{
		var match = _regex.Match(url);

		if (match.Success)
		{
			var values = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
			formattedUrl = string.Format(_template, values);
			return true;
		}
		else
		{
			formattedUrl = null;
			return false;
		}
	}
}
