namespace VocaDb.Model.Domain.Globalization
{
	/// <summary>
	/// Detects language from a string.
	/// </summary>
	public interface ILanguageDetector
	{
		/// <summary>
		/// Detects language from a string.
		/// </summary>
		/// <param name="str">String to be analyzed. Can be null or empty.</param>
		/// <param name="def">Default value to be returned when the language cannot be determined.</param>
		/// <returns>The detected language, or <paramref name="def"/>.</returns>
		ContentLanguageSelection Detect(string str, ContentLanguageSelection def);
	}
}
