using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Tags
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record TagCategoryForApiContract
	{
		/// <summary>
		/// PERF: This is a subset of the <see cref="Tags.TagForApiContract" /> class with minimal properties.
		/// </summary>
		[DataContract(Namespace = Schemas.VocaDb)]
		public sealed record TagForApiContract
		{
			/// <summary>
			/// Comma-separated list of all other names that aren't the display name.
			/// </summary>
			[DataMember(EmitDefaultValue = false)]
			public string? AdditionalNames { get; init; }

			[DataMember]
			public int Id { get; init; }

			[DataMember]
			public string Name { get; init; }

			[DataMember]
			public int UsageCount { get; init; }

			public TagForApiContract(Tag tag, ContentLanguagePreference languagePreference)
			{
				AdditionalNames = tag.Names.GetAdditionalNamesStringForLanguage(languagePreference).EmptyToNull();
				Id = tag.Id;
				Name = tag.TranslatedName[languagePreference];
				UsageCount = tag.UsageCount;
			}
		}

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public TagForApiContract[] Tags { get; init; }

		public TagCategoryForApiContract(string name, ContentLanguagePreference languagePreference, IEnumerable<Tag> tags)
		{
			ParamIs.NotNull(() => name);
			ParamIs.NotNull(() => tags);

			Name = name;
			Tags = tags.Select(t => new TagForApiContract(t, languagePreference)).ToArray();
		}
	}
}
