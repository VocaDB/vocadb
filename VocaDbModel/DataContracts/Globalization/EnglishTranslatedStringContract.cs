#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Globalization
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EnglishTranslatedStringContract
	{
		public EnglishTranslatedStringContract()
		{
			English = Original = string.Empty;
		}

#nullable enable
		public EnglishTranslatedStringContract(EnglishTranslatedString str)
		{
			ParamIs.NotNull(() => str);

			English = str.English;
			Original = str.Original;
		}
#nullable disable

		[DataMember]
		public string English { get; set; }

		[DataMember]
		public string Original { get; init; }
	}
}
