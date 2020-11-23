using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagMappingContract
	{
		public TagMappingContract() { }

		public TagMappingContract(TagMapping tagMapping, ContentLanguagePreference lang)
		{
			ParamIs.NotNull(() => tagMapping);

			SourceTag = tagMapping.SourceTag;
			Tag = new TagBaseContract(tagMapping.Tag, lang);
		}

		public string SourceTag { get; set; }

		public TagBaseContract Tag { get; set; }
	}
}
