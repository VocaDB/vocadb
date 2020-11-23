using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagUsageForApiContract
	{
		public TagUsageForApiContract() { }

		public TagUsageForApiContract(TagUsage tagUsage, ContentLanguagePreference languagePreference)
		{
			Count = tagUsage.Count;
			Tag = new TagBaseContract(tagUsage.Tag, languagePreference, includeAdditionalNames: true, includeCategory: true);
		}

		public TagUsageForApiContract(Tag tag, int count, ContentLanguagePreference languagePreference)
		{
			Count = count;
			Tag = new TagBaseContract(tag, languagePreference, includeAdditionalNames: true, includeCategory: true);
		}

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public TagBaseContract Tag { get; set; }
	}
}
