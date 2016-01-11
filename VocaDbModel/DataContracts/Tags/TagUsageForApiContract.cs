using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagUsageForApiContract {

		public TagUsageForApiContract() { }

		public TagUsageForApiContract(TagUsage tagUsage, ContentLanguagePreference languagePreference) {
			Count = tagUsage.Count;
			Tag = new TagForApiContract(tagUsage.Tag, null, false, languagePreference, TagOptionalFields.AdditionalNames);
		}

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public TagForApiContract Tag { get; set; }

	}

}
