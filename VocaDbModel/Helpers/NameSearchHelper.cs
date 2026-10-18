using System.Text;

namespace VocaDb.Model.Helpers;

/// <summary>
/// Folds names for substring search, so that names can be matched with a binary collation.
/// The folding roughly matches VocaDB's default database collation (case, kana type and width insensitive):
/// NFKC (full-width and half-width forms), hiragana to katakana, then upper case.
/// </summary>
public static class NameSearchHelper
{
	public const int MaxLength = 255;

	public static string Normalize(string value)
	{
		string normalized;
		try
		{
			normalized = value.Normalize(NormalizationForm.FormKC);
		}
		catch (ArgumentException)
		{
			normalized = value;
		}

		var chars = normalized.ToUpperInvariant().ToCharArray();

		for (var i = 0; i < chars.Length; i++)
		{
			var c = chars[i];
			if ((c >= '\u3041' && c <= '\u3096') || c == '\u309D' || c == '\u309E')
				chars[i] = (char)(c + 0x60);
		}

		return chars.Length > MaxLength ? new string(chars, 0, MaxLength) : new string(chars);
	}

	/// <summary>
	/// Folds a search term that has already been escaped for LIKE (<see cref="Service.Helpers.FindHelpers.CleanTerm"/>).
	/// </summary>
	public static string NormalizeTerm(string term)
	{
		return Normalize(term.Replace("［", "[[]").Replace("％", "[%]"));
	}
}
