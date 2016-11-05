using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagBaseContract : ITag, IEntryWithIntId {

		public TagBaseContract() { }

		public TagBaseContract(Tag tag, ContentLanguagePreference languagePreference,
			bool includeAdditionalNames = false) {
			
			ParamIs.NotNull(() => tag);

			Id = tag.Id;
			Name = tag.TranslatedName[languagePreference];
			UrlSlug = tag.UrlSlug;

			if (includeAdditionalNames)
				AdditionalNames = tag.Names.GetAdditionalNamesStringForLanguage(languagePreference);

		}

		/// <summary>
		/// Additional names - optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string UrlSlug { get; set; }

	}

}
