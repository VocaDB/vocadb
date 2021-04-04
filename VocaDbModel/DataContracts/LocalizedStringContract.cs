#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class LocalizedStringContract : ILocalizedString
	{
		public LocalizedStringContract() { }

#nullable enable
		public LocalizedStringContract(LocalizedString str)
		{
			ParamIs.NotNull(() => str);

			Language = str.Language;
			Value = str.Value;
		}
#nullable disable

		public LocalizedStringContract(string value, ContentLanguageSelection language)
		{
			Value = value;
			Language = language;
		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection Language { get; init; }

		[DataMember]
		public string Value { get; set; }
	}
}
