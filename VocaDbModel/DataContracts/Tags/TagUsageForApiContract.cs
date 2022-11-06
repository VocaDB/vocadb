using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags;

[DataContract(Namespace = Schemas.VocaDb)]
public class TagUsageForApiContract
{
#nullable disable
	public TagUsageForApiContract() { }
#nullable enable

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
	public int Count { get; init; }

	[DataMember]
	public TagBaseContract Tag { get; init; }
}
