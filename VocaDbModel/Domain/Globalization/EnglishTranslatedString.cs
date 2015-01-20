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

		public virtual string GetBestMatch(ContentLanguagePreference languagePreference) {

			if ((languagePreference == ContentLanguagePreference.English || languagePreference == ContentLanguagePreference.Romaji) && HasEnglish) {
				return English;
			} else {
				return Original;
			}

		}

		public string this[ContentLanguagePreference preference] {
			get {
				return GetBestMatch(preference);
			}
		}

	}

}
