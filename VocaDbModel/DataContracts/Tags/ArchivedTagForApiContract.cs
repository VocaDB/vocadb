using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedTagForApiContract
{
	[DataMember]
	public required string CategoryName { get; set; }

	[DataMember]
	public required string? Description { get; set; }

	[DataMember]
	public required string? DescriptionEng { get; set; }

	[DataMember]
	public required bool HideFromSuggestions { get; set; }

	[DataMember]
	public required int Id { get; set; }

	[DataMember]
	public required LocalizedStringContract[]? Names { get; set; }

	[DataMember]
	public required ObjectRefContract? Parent { get; set; }

	[DataMember]
	public required ObjectRefContract[]? RelatedTags { get; set; }

	[DataMember]
	public required TagTargetTypes Targets { get; set; }

	[DataMember]
	public required string? ThumbMime { get; init; }

	[DataMember]
	public required ArchivedTranslatedStringContract TranslatedName { get; set; }

	[DataMember]
	public required ArchivedWebLinkContract[]? WebLinks { get; set; }

	public static ArchivedTagForApiContract Create(ArchivedTagContract contract, IUserPermissionContext permissionContext)
	{
		return new()
		{
			CategoryName = contract.CategoryName,
			Description = contract.Description,
			DescriptionEng = contract.DescriptionEng,
			HideFromSuggestions = contract.HideFromSuggestions,
			Id = contract.Id,
			Names = contract.Names,
			Parent = contract.Parent,
			RelatedTags = contract.RelatedTags,
			Targets = contract.Targets,
			ThumbMime = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
				? contract.ThumbMime
				: null,
			TranslatedName = contract.TranslatedName,
			WebLinks = contract.WebLinks,
		};
	}
}
