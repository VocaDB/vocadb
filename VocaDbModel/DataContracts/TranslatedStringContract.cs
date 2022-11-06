using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts;

[DataContract(Namespace = Schemas.VocaDb)]
public class TranslatedStringContract : ITranslatedString
{
#nullable disable
	public TranslatedStringContract() { }
#nullable enable

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

#nullable disable
	[DataMember]
	public string Default { get; init; }
#nullable enable

	[DataMember]
	public ContentLanguageSelection DefaultLanguage { get; set; }

	[DataMember]
	public string English { get; set; }

	[DataMember]
	public string Japanese { get; set; }

	//[DataMember]
	//public string Other { get; init; }

	[DataMember]
	public string Romaji { get; set; }
}
