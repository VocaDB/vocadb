#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	/// <summary>
	/// Minimal tag details: Id, Name (translated), UrlSlug.
	/// Optionally includes AdditionalNames and Category.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagBaseContract : ITag, IEntryWithIntId
	{
		public TagBaseContract() { }

#nullable enable
		public TagBaseContract(Tag tag, ContentLanguagePreference languagePreference,
			bool includeAdditionalNames = false, bool includeCategory = false)
		{
			ParamIs.NotNull(() => tag);

			Id = tag.Id;
			Name = tag.TranslatedName[languagePreference];
			UrlSlug = tag.UrlSlug;

			if (includeAdditionalNames)
				AdditionalNames = tag.Names.GetAdditionalNamesStringForLanguage(languagePreference);

			if (includeCategory)
				CategoryName = tag.CategoryName;
		}
#nullable disable

		/// <summary>
		/// Additional names - optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public string CategoryName { get; init; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public string UrlSlug { get; init; }
	}
}
