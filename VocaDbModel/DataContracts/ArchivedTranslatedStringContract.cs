#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedTranslatedStringContract : ITranslatedString
	{
		public ArchivedTranslatedStringContract() { }

		public ArchivedTranslatedStringContract(TranslatedString translatedString)
		{
			ParamIs.NotNull(() => translatedString);

			DefaultLanguage = translatedString.DefaultLanguage;
			English = translatedString.English;
			Japanese = translatedString.Japanese;
			Romaji = translatedString.Romaji;
		}

		[DataMember]
		public ContentLanguageSelection DefaultLanguage { get; set; }

		[DataMember]
		public string English { get; set; }

		[DataMember]
		public string Japanese { get; set; }

		[DataMember]
		public string Romaji { get; set; }
	}
}
