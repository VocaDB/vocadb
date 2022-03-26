using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Globalization
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record EnglishTranslatedStringContract
	{
		[DataMember]
		public string English { get; set; }

		[DataMember]
		public string Original { get; init; }

		public EnglishTranslatedStringContract()
		{
			English = Original = string.Empty;
		}

		public EnglishTranslatedStringContract(EnglishTranslatedString str)
		{
			ParamIs.NotNull(() => str);

			English = str.English;
			Original = str.Original;
		}

		public bool HasEnglish => !string.IsNullOrEmpty(English);

		public bool ShowEnglish(ContentLanguagePreference languagePreference) =>
			(languagePreference == ContentLanguagePreference.English || languagePreference == ContentLanguagePreference.Romaji) && HasEnglish;

		public string GetBestMatch(ContentLanguagePreference languagePreference) =>
			ShowEnglish(languagePreference) ? English : Original;

		/// <summary>
		/// Primarily gets English name, Original if English is not specified.
		/// </summary>
		public string EnglishOrOriginal => GetBestMatch(ContentLanguagePreference.English);
	}
}
