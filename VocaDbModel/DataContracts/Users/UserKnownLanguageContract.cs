using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserKnownLanguageContract
	{
#nullable disable
		public UserKnownLanguageContract() { }
#nullable enable

		public UserKnownLanguageContract(UserKnownLanguage userKnownLanguage)
		{
			ParamIs.NotNull(() => userKnownLanguage);

			CultureCode = userKnownLanguage.CultureCode.CultureCode;
			Proficiency = userKnownLanguage.Proficiency;
		}

		[DataMember]
		public string CultureCode { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public UserLanguageProficiency Proficiency { get; init; }
	}
}
