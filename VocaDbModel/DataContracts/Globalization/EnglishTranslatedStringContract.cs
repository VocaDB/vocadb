using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Globalization {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EnglishTranslatedStringContract {

		public EnglishTranslatedStringContract() { }

		public EnglishTranslatedStringContract(EnglishTranslatedString str) {
			English = str.English;
			Original = str.Original;
		}

		[DataMember]
		public string English { get; set; }

		[DataMember]
		public string Original { get; set; }

	}
}
