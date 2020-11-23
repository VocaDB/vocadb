using System.Collections.Generic;
using System.Linq;
using System;

namespace VocaDb.Model.Domain.Globalization
{

	/// <summary>
	/// String that is translated to all common languages supported by the system.
	/// </summary>
	public class TranslatedString : ITranslatedString
	{

		public static TranslatedString Create(Func<ContentLanguageSelection, string> factory)
		{

			return new TranslatedString(
				factory(ContentLanguageSelection.Japanese),
				factory(ContentLanguageSelection.Romaji),
				factory(ContentLanguageSelection.English)
			);

		}

		public static TranslatedString Create(string uniform)
		{
			return new TranslatedString(uniform, uniform, uniform);
		}

		private string english;
		private string original;
		private string romaji;

		public TranslatedString()
		{
			DefaultLanguage = ContentLanguageSelection.Japanese;
		}

		public TranslatedString(string original, string romaji, string english)
			: this()
		{

			Japanese = original;
			Romaji = romaji;
			English = english;

		}

		public TranslatedString(string original, string romaji, string english,
			ContentLanguageSelection defaultLanguage)
		{

			Japanese = original;
			Romaji = romaji;
			English = english;
			DefaultLanguage = defaultLanguage;

		}

		public TranslatedString(ITranslatedString contract)
			: this()
		{

			ParamIs.NotNull(() => contract);

			CopyFrom(contract);

		}

		public virtual string this[ContentLanguageSelection language]
		{
			get
			{

				switch (language)
				{
					case ContentLanguageSelection.English:
						return English;
					case ContentLanguageSelection.Japanese:
						return Japanese;
					case ContentLanguageSelection.Romaji:
						return Romaji;
					default:
						return Default;
				}

			}
			set
			{

				switch (language)
				{
					case ContentLanguageSelection.English:
						English = value;
						break;
					case ContentLanguageSelection.Japanese:
						Japanese = value;
						break;
					case ContentLanguageSelection.Romaji:
						Romaji = value;
						break;
					default:
						Default = value;
						break;
				}

			}
		}

		public string this[ContentLanguagePreference preference]
		{
			get
			{
				return GetBestMatch(preference);
			}
		}

		/// <summary>
		/// All names in prioritized order.
		/// Cannot be null.
		/// </summary>
		public virtual IEnumerable<string> All
		{
			get
			{
				return new[] {
					Japanese,
					Romaji,
					English,
				};
			}
		}

		public virtual IEnumerable<LocalizedString> AllLocalized
		{
			get
			{
				return new[] {
					new LocalizedString(Japanese, ContentLanguageSelection.Japanese),
					new LocalizedString(Romaji, ContentLanguageSelection.Romaji),
					new LocalizedString(English, ContentLanguageSelection.English),
				};
			}
		}

		/*public virtual string Default {
			get {  return defaultVal; }
			protected set {
				ParamIs.NotNullOrEmpty(() => value);
				defaultVal = value;
			}
		}*/

		/// <summary>
		/// Name in the default language, or first available translation.
		/// Can be null or empty, but only if there are no translations.
		/// </summary>
		public virtual string Default
		{
			get
			{
				return GetDefaultOrFirst();
			}
			set
			{

				switch (DefaultLanguage)
				{
					case ContentLanguageSelection.English:
						English = value;
						break;
					case ContentLanguageSelection.Japanese:
						Japanese = value;
						break;
					case ContentLanguageSelection.Romaji:
						Romaji = value;
						break;
					default:
						Japanese = value;
						break;
				}

			}
		}

		public virtual ContentLanguageSelection DefaultLanguage { get; set; }

		/// <summary>
		/// Name in English.
		/// TODO: currently this can be null/empty, but that should be changed for all new fields.
		/// </summary>
		public virtual string English
		{
			get { return english; }
			set
			{
				english = value;
				//UpdateDefault();
			}
		}

		/// <summary>
		/// Name in the original language (usually Japanese).
		/// TODO: currently this can be null/empty, but that should be changed for all new fields.
		/// </summary>
		public virtual string Japanese
		{
			get { return original; }
			set
			{
				original = value;
			}
		}

		/// <summary>
		/// Romanized name.
		/// TODO: currently this can be null/empty, but that should be changed for all new fields.
		/// </summary>
		public virtual string Romaji
		{
			get { return romaji; }
			set
			{
				romaji = value;
			}
		}

		public virtual void Clear()
		{
			Japanese = Romaji = English = string.Empty;
		}

		public virtual void CopyFrom(ITranslatedString contract)
		{

			ParamIs.NotNull(() => contract);

			DefaultLanguage = contract.DefaultLanguage;
			English = contract.English;
			Japanese = contract.Japanese;
			Romaji = contract.Romaji;

		}

		public virtual string GetBestMatch(ContentLanguagePreference preference)
		{

			return GetBestMatch(preference, DefaultLanguage);

		}

		public virtual string GetBestMatch(ContentLanguagePreference preference, ContentLanguageSelection defaultLanguage)
		{

			var val = this[preference == ContentLanguagePreference.Default ? defaultLanguage : (ContentLanguageSelection)preference];

			return (!string.IsNullOrEmpty(val) ? val : GetDefaultOrFirst(defaultLanguage));

		}

		public virtual string GetDefaultOrFirst()
		{

			return GetDefaultOrFirst(DefaultLanguage);

		}

		/// <summary>
		/// Gets the translation matching the selected language, or the first translation if the specified language has no translation.
		/// </summary>
		/// <param name="defaultLanguage">Selected language. If this is Unspecified, DefaultLanguage will be used.</param>
		/// <returns>Translated name for the selected language, or first translation. Cannot be null. Can be empty, but only if there are no translations.</returns>
		public virtual string GetDefaultOrFirst(ContentLanguageSelection defaultLanguage)
		{

			var val = (defaultLanguage != ContentLanguageSelection.Unspecified || DefaultLanguage != ContentLanguageSelection.Unspecified ? this[defaultLanguage] : null);

			return !string.IsNullOrEmpty(val) ? val : All.FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? string.Empty;

		}

	}

}
