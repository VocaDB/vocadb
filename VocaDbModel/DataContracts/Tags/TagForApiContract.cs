#nullable disable

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

namespace VocaDb.Model.DataContracts.Tags
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagForApiContract
	{
		public TagForApiContract() { }

		public TagForApiContract(
			Tag tag,
			ContentLanguagePreference languagePreference,
			TagOptionalFields optionalFields
		) : this(tag, null, languagePreference, optionalFields) { }

		public TagForApiContract(
			Tag tag,
			IAggregatedEntryImageUrlFactory thumbPersister,
			ContentLanguagePreference languagePreference,
			TagOptionalFields optionalFields
		)
		{
			ParamIs.NotNull(() => tag);

			CategoryName = tag.CategoryName;
			CreateDate = tag.CreateDate;
			DefaultNameLanguage = tag.TranslatedName.DefaultLanguage;
			Id = tag.Id;
			Name = tag.TranslatedName[languagePreference];
			Status = tag.Status;
			Targets = (int)tag.Targets;
			UrlSlug = tag.UrlSlug;
			UsageCount = tag.UsageCount;
			Version = tag.Version;

			var includeAdditionalNames = optionalFields.HasFlag(TagOptionalFields.AdditionalNames);

			if (includeAdditionalNames)
			{
				AdditionalNames = tag.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (optionalFields.HasFlag(TagOptionalFields.Description))
			{
				Description = tag.Description[languagePreference];
			}

			if (optionalFields.HasFlag(TagOptionalFields.MainPicture) && tag.Thumb != null && thumbPersister != null)
			{
				MainPicture = new EntryThumbForApiContract(tag.Thumb, thumbPersister);
			}

			if (optionalFields.HasFlag(TagOptionalFields.Names))
			{
				Names = tag.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			}

			if (optionalFields.HasFlag(TagOptionalFields.Parent) && tag.Parent != null)
			{
				Parent = new TagBaseContract(tag.Parent, languagePreference, includeAdditionalNames);
			}

			if (optionalFields.HasFlag(TagOptionalFields.RelatedTags))
			{
				RelatedTags = tag.RelatedTags.Select(t => new TagBaseContract(t.LinkedTag, languagePreference, includeAdditionalNames)).ToArray();
			}

			if (optionalFields.HasFlag(TagOptionalFields.TranslatedDescription))
			{
				TranslatedDescription = new EnglishTranslatedStringContract(tag.Description);
			}

			if (optionalFields.HasFlag(TagOptionalFields.WebLinks))
			{
				WebLinks = tag.WebLinks.Links.Select(w => new WebLinkForApiContract(w)).ToArray();
			}
		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; init; }

		[DataMember(EmitDefaultValue = false)]
		[Obsolete("Tag aliases are now just names")]
		public TagBaseContract AliasedTo { get; init; }

		[DataMember]
		public string CategoryName { get; init; }

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; init; }

		/// <summary>
		/// Language selection of the original name.
		/// </summary>
		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection DefaultNameLanguage { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; init; }

		[DataMember]
		public string Name { get; init; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringWithIdContract[] Names { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public TagBaseContract Parent { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public TagBaseContract[] RelatedTags { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; init; }

		[DataMember]
		public int Targets { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public EnglishTranslatedStringContract TranslatedDescription { get; init; }

		[DataMember]
		public string UrlSlug { get; init; }

		[DataMember]
		public int UsageCount { get; init; }

		[DataMember]
		public int Version { get; init; }

		/// <summary>
		/// List of external links. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; init; }
	}

	[Flags]
	public enum TagOptionalFields
	{
		None = 0,
		AdditionalNames = 1 << 0,
		[Obsolete("Tag aliases are now just names")]
		AliasedTo = 1 << 1,
		Description = 1 << 2,
		MainPicture = 1 << 3,
		Names = 1 << 4,
		Parent = 1 << 5,
		RelatedTags = 1 << 6,
		TranslatedDescription = 1 << 7,
		WebLinks = 1 << 8,
	}
}
