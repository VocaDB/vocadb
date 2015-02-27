using VocaDb.Model.DataContracts.Globalization;

namespace VocaDb.Model.Domain.Globalization {

	/// <summary>
	/// String that can be translated into one language.
	/// </summary>
	public class EnglishTranslatedString {

		private string english;
		private string original;

		public EnglishTranslatedString() :
			this(string.Empty) {
		}

		public EnglishTranslatedString(string original, string english = "") {
			Original = original;
			English = english;
		}

		public virtual string English {
			get { return english; }
			set {
				ParamIs.NotNull(() => value);
				english = value;
			}
		}

		/// <summary>
		/// Primarily gets English name, Original if English is not specified.
		/// </summary>
		public virtual string EnglishOrOriginal {
			get {
				return GetBestMatch(ContentLanguagePreference.English);
			}
		}

		public virtual bool HasEnglish {
			get {
				return !string.IsNullOrEmpty(English);
			}
		}

		public virtual bool IsEmpty {
			get {
				return string.IsNullOrEmpty(Original);
			}
		}

		public virtual string Original {
			get { return original; }
			set {
				ParamIs.NotNull(() => value);
				original = value;
			}
		}

		public virtual bool CopyFrom(EnglishTranslatedStringContract contract) {
			
			ParamIs.NotNull(() => contract);

			var changed = false;

			if (Original != contract.Original) {
				Original = contract.Original;
				changed = true;
			}

			if (English != contract.English) {
				English = contract.English;
				changed = true;
			}

			return changed;

		}

		public virtual bool ShowEnglish(ContentLanguagePreference languagePreference) {
			return (languagePreference == ContentLanguagePreference.English || languagePreference == ContentLanguagePreference.Romaji) && HasEnglish;
		}

		public virtual string GetBestMatch(ContentLanguagePreference languagePreference) {
			return ShowEnglish(languagePreference) ? English : Original;
		}

		public string this[ContentLanguagePreference preference] {
			get {
				return GetBestMatch(preference);
			}
		}

	}

}
