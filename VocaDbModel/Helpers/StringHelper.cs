using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace VocaDb.Model.Helpers;

public static class StringHelper
{
	private static readonly Regex s_invalidXmlCharacterReference = new(@"&#x([0-9A-Fa-f]+);", RegexOptions.Compiled);

	/// <summary>
	/// Removes unsupported control characters from XML.
	/// See http://en.wikipedia.org/w/index.php?title=C0_and_C1_control_codes for more info.
	/// </summary>
	public static string CleanInvalidXmlChars(string text)
	{
		if (string.IsNullOrEmpty(text))
			return text;

		var cleaned = s_invalidXmlCharacterReference.Replace(text, match =>
		{
			return int.TryParse(match.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint)
				&& IsValidXmlChar(codePoint)
				? match.Value
				: string.Empty;
		});

		return RemoveInvalidXmlChars(cleaned);
	}

	private static bool IsValidXmlChar(int codePoint)
	{
		return codePoint <= 0xFFFF
			? XmlConvert.IsXmlChar((char)codePoint)
			: codePoint <= 0x10FFFF;
	}

	public static string RemoveInvalidXmlChars(string text)
	{
		if (string.IsNullOrEmpty(text))
			return text;

		StringBuilder? builder = null;
		var start = 0;

		for (var i = 0; i < text.Length; i++)
		{
			var c = text[i];

			if (XmlConvert.IsXmlChar(c))
				continue;

			if (char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
			{
				i++;
				continue;
			}

			builder ??= new StringBuilder(text.Length);
			builder.Append(text, start, i - start);
			start = i + 1;
		}

		if (builder == null)
			return text;

		builder.Append(text, start, text.Length - start);

		return builder.ToString();
	}

	/// <summary>
	/// Removes invisible control characters from the string.
	/// This includes characters from <see cref="char.IsControl(char)"/>, except
	/// common whitespace characters like newlines and tabs.
	/// </summary>
	/// <param name="text">String to be processed. Can be null or empty.</param>
	/// <returns>String without control characters.</returns>
	[return: NotNullIfNotNull(nameof(text))]
	public static string? RemoveControlChars(string? text)
	{
		if (string.IsNullOrEmpty(text))
			return text;

		var whitelist = new HashSet<char>(new[] { '\n', '\t' });
		return new string(text.Where(c => whitelist.Contains(c) || !char.IsControl(c)).ToArray());
	}

	/// <summary>
	/// Trims string if it's not completely whitespace.
	/// </summary>
	/// <remarks>
	/// There are songs and albums where the name is completely whitespace, so this must be supported.
	/// However, whitespace in the beginning or end is assumed to be an error.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(text))]
	public static string? TrimIfNotWhitespace(string? text)
	{
		return string.IsNullOrWhiteSpace(text) ? text : text.Trim();
	}
}
