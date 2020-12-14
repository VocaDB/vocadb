#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class TranslatedStringContract : ITranslatedString
	{
		public TranslatedStringContract() { }

		public TranslatedStringContract(string english, string japanese,
			string romaji, ContentLanguageSelection defaultLanguage)
		{
			English = english;
			Japanese = japanese;
			Romaji = romaji;
			DefaultLanguage = defaultLanguage;
		}

		public TranslatedStringContract(TranslatedString str)
		{
			ParamIs.NotNull(() => str);

			Default = str.Default;
			DefaultLanguage = str.DefaultLanguage;
			English = str.English;
			Japanese = str.Japanese;
			//Other = str.Other;
			Romaji = str.Romaji;
		}

		[DataMember]
		public string Default { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultLanguage { get; set; }

		[DataMember]
		public string English { get; set; }

		[DataMember]
		public string Japanese { get; set; }

		//[DataMember]
		//public string Other { get; set; }

		[DataMember]
		public string Romaji { get; set; }
	}
}
