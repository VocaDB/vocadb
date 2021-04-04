#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagMappingContract
	{
		public TagMappingContract() { }

#nullable enable
		public TagMappingContract(TagMapping tagMapping, ContentLanguagePreference lang)
		{
			ParamIs.NotNull(() => tagMapping);

			SourceTag = tagMapping.SourceTag;
			Tag = new TagBaseContract(tagMapping.Tag, lang);
		}
#nullable disable

		public string SourceTag { get; init; }

		public TagBaseContract Tag { get; init; }
	}
}
