#nullable disable

using VocaDb.Model.DataContracts.Globalization;

namespace VocaDb.Model.Domain.Globalization
{
	/// <summary>
	/// String that can be translated into one language.
	/// </summary>
	public class EnglishTranslatedString
	{
		private string _english;
		private string _original;

		public EnglishTranslatedString() :
			this(string.Empty)
		{
		}

		public EnglishTranslatedString(string original, string english = "")
		{
			Original = original;
			English = english;
		}

		public virtual string English
		{
			get => _english;
			set
			{
				ParamIs.NotNull(() => value);
				_english = value;
			}
		}

		/// <summary>
		/// Primarily gets English name, Original if English is not specified.
		/// </summary>
		public virtual string EnglishOrOriginal => GetBestMatch(ContentLanguagePreference.English);

		public virtual bool HasEnglish => !string.IsNullOrEmpty(English);

		public virtual bool IsEmpty => string.IsNullOrEmpty(Original);

		public virtual string Original
		{
			get => _original;
			set
			{
				ParamIs.NotNull(() => value);
				_original = value;
			}
		}

		public virtual bool CopyFrom(EnglishTranslatedStringContract contract)
		{
			ParamIs.NotNull(() => contract);

			var changed = false;
			var newOriginal = contract.Original?.Trim();

			if (Original != newOriginal)
			{
				Original = newOriginal;
				changed = true;
			}

			var newEnglish = contract.English?.Trim();

			if (English != newEnglish)
			{
				English = newEnglish;
				changed = true;
			}

			return changed;
		}

		public virtual bool CopyIfEmpty(EnglishTranslatedString source)
		{
			bool changed = false;

			if (Original == string.Empty && Original != source.Original)
			{
				Original = source.Original;
				changed = true;
			}

			if (English == string.Empty && English != source.English)
			{
				English = source.English;
				changed = true;
			}

			return changed;
		}

		public virtual bool ShowEnglish(ContentLanguagePreference languagePreference)
		{
			return (languagePreference == ContentLanguagePreference.English || languagePreference == ContentLanguagePreference.Romaji) && HasEnglish;
		}

		public virtual string GetBestMatch(ContentLanguagePreference languagePreference)
		{
			return ShowEnglish(languagePreference) ? English : Original;
		}

		public string this[ContentLanguagePreference preference] => GetBestMatch(preference);
	}
}
