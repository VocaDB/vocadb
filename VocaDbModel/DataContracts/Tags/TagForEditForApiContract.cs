using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record TagForEditForApiContract
{
	[DataMember]
	public bool CanDelete { get; init; }

	[DataMember]
	public string CategoryName { get; init; }

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public EnglishTranslatedStringContract Description { get; set;/* TODO */ }

	[DataMember]
	public bool HideFromSuggestions { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public LocalizedStringWithIdContract[] Names { get; set;/* TODO */ }

	[DataMember]
	public TagBaseContract? Parent { get; set;/* TODO */ }

	[DataMember]
	public TagBaseContract[] RelatedTags { get; init; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public int/* TODO: enum TagTargetTypes*/ Targets { get; init; }

	[DataMember]
	public string UpdateNotes { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public TagForEditForApiContract()
	{
		CategoryName = string.Empty;
		Description = new EnglishTranslatedStringContract();
		Name = string.Empty;
		Names = Array.Empty<LocalizedStringWithIdContract>();
		RelatedTags = Array.Empty<TagBaseContract>();
		UpdateNotes = string.Empty;
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public TagForEditForApiContract(
		Tag tag,
		bool isEmpty,
		IUserPermissionContext userContext,
		IAggregatedEntryImageUrlFactory? thumbPersister
	)
	{
		CanDelete = EntryPermissionManager.CanDelete(userContext, tag);
		CategoryName = tag.CategoryName;
		DefaultNameLanguage = tag.TranslatedName.DefaultLanguage;
		Deleted = tag.Deleted;
		Description = new EnglishTranslatedStringContract(tag.Description);
		HideFromSuggestions = tag.HideFromSuggestions;
		Id = tag.Id;
		MainPicture = tag.Thumb is not null && thumbPersister is not null
			? (userContext.HasPermission(PermissionToken.ViewCoverArtImages) ? new EntryThumbForApiContract(tag.Thumb, thumbPersister) : null)
			: null;
		Name = tag.TranslatedName[userContext.LanguagePreference];
		Names = tag.Names
			.Select(n => new LocalizedStringWithIdContract(n))
			.ToArray();
		Parent = tag.Parent is not null
			? new TagBaseContract(tag.Parent, userContext.LanguagePreference)
			: null;
		RelatedTags = tag.RelatedTags
			.Select(t => new TagBaseContract(t.LinkedTag, userContext.LanguagePreference, false))
			.ToArray();
		Status = tag.Status;
		Targets = (int)tag.Targets;
		UpdateNotes = string.Empty;
		WebLinks = tag.WebLinks.Links
			.Select(w => new WebLinkForApiContract(w))
			.OrderBy(w => w.DescriptionOrUrl)
			.ToArray();
	}
}
