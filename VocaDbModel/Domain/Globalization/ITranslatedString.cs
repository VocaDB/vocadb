namespace VocaDb.Model.Domain.Globalization
{
	public interface ITranslatedString
	{
		ContentLanguageSelection DefaultLanguage { get; set; }

		/// <summary>
		/// Name in English.
		/// TODO: currently this can be null/empty, but that should be changed for all new fields.
		/// </summary>
		string English { get; set; }

		/// <summary>
		/// Name in the original language (usually Japanese).
		/// TODO: currently this can be null/empty, but that should be changed for all new fields.
		/// </summary>
		string Japanese { get; set; }

		/// <summary>
		/// Romanized name.
		/// TODO: currently this can be null/empty, but that should be changed for all new fields.
		/// </summary>
		string Romaji { get; set; }
	}
}