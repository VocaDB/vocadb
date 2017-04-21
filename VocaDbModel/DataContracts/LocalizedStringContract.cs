using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class LocalizedStringContract : ILocalizedString {

		public LocalizedStringContract() {}

		public LocalizedStringContract(LocalizedString str) {
			
			ParamIs.NotNull(() => str);

			Language = str.Language;
			Value = str.Value;

		}

		public LocalizedStringContract(string value, ContentLanguageSelection language) {
			Value = value;
			Language = language;
		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection Language { get; set; }

		[DataMember]
		public string Value { get; set; }

	}

}
