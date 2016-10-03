using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagForApiContract {

		public TagForApiContract() { }

		public TagForApiContract(Tag tag,
			ContentLanguagePreference languagePreference,
			TagOptionalFields optionalFields) : this(tag, null, false, languagePreference, optionalFields) { }

		public TagForApiContract(Tag tag, 
			IEntryImagePersisterOld thumbPersister,
			bool ssl,
			ContentLanguagePreference languagePreference,
			TagOptionalFields optionalFields) {

			ParamIs.NotNull(() => tag);

			CategoryName = tag.CategoryName;
			CreateDate = tag.CreateDate;
			DefaultNameLanguage = tag.TranslatedName.DefaultLanguage;
			Id = tag.Id;
			Name = tag.TranslatedName[languagePreference];
			Status = tag.Status;
			UrlSlug = tag.UrlSlug;
			UsageCount = tag.UsageCount;
			Version = tag.Version;

			var includeAdditionalNames = optionalFields.HasFlag(TagOptionalFields.AdditionalNames);

			if (includeAdditionalNames) {
				AdditionalNames = tag.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (optionalFields.HasFlag(TagOptionalFields.Description)) {
				Description = tag.Description[languagePreference];
			}

			if (optionalFields.HasFlag(TagOptionalFields.MainPicture) && tag.Thumb != null) {
				MainPicture = new EntryThumbForApiContract(tag.Thumb, thumbPersister, ssl);
			}

			if (optionalFields.HasFlag(TagOptionalFields.Names)) {
				Names = tag.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			}

			if (optionalFields.HasFlag(TagOptionalFields.Parent) && tag.Parent != null) {
				Parent = new TagBaseContract(tag.Parent, languagePreference, includeAdditionalNames);
			}

			if (optionalFields.HasFlag(TagOptionalFields.RelatedTags)) {
				RelatedTags = tag.RelatedTags.Select(t => new TagBaseContract(t.LinkedTag, languagePreference, includeAdditionalNames)).ToArray();
			}

			if (optionalFields.HasFlag(TagOptionalFields.TranslatedDescription)) {
				TranslatedDescription = new EnglishTranslatedStringContract(tag.Description);
			}

			if (optionalFields.HasFlag(TagOptionalFields.WebLinks)) {
				WebLinks = tag.WebLinks.Links.Select(w => new WebLinkForApiContract(w)).ToArray();
			}

		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set; }

		[DataMember(EmitDefaultValue = false)]
		[Obsolete("Tag aliases are now just names")]
		public TagBaseContract AliasedTo { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; set; }

		/// <summary>
		/// Language selection of the original name.
		/// </summary>
		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagBaseContract Parent { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagBaseContract[] RelatedTags { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EnglishTranslatedStringContract TranslatedDescription { get; set; }

		[DataMember]
		public string UrlSlug { get; set; }

		[DataMember]
		public int UsageCount { get; set; }

		[DataMember]
		public int Version { get; set; }

		/// <summary>
		/// List of external links. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum TagOptionalFields {

		None					= 0,
		AdditionalNames			= 1,
		[Obsolete("Tag aliases are now just names")]
		AliasedTo = 2,
		Description				= 4,
		MainPicture				= 8,
		Names					= 16,
		Parent					= 32,
		RelatedTags				= 64,
		TranslatedDescription	= 128,
		WebLinks				= 256

	}

}
