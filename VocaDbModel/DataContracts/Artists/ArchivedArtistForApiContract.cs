using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Artists;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedArtistForApiContract
{
	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public required ArtistType ArtistType { get; set; }

	[DataMember]
	public required ObjectRefContract? BaseVoicebank { get; set; }

	[DataMember]
	public required string? Description { get; set; }

	[DataMember]
	public required string? DescriptionEng { get; set; }

	[DataMember]
	public required ArchivedArtistForArtistContract[] Groups { get; set; }

	[DataMember]
	public required int Id { get; set; }

	[DataMember]
	public required string? MainPictureMime { get; set; }

	[DataMember]
	public required ObjectRefContract[] Members { get; set; }

	[DataMember]
	public required LocalizedStringContract[]? Names { get; set; }

	[DataMember]
	public required ArchivedEntryPictureFileContract[]? Pictures { get; set; }

	[DataMember]
	public required DateTime? ReleaseDate { get; set; }

	[DataMember]
	public required TranslatedStringContract TranslatedName { get; set; }

	[DataMember]
	public required ArchivedWebLinkContract[]? WebLinks { get; set; }

	public static ArchivedArtistForApiContract Create(ArchivedArtistContract contract, IUserPermissionContext permissionContext)
	{
		return new()
		{
			ArtistType = contract.ArtistType,
			BaseVoicebank = contract.BaseVoicebank,
			Description = contract.Description,
			DescriptionEng = contract.DescriptionEng,
			Groups = contract.Groups,
			Id = contract.Id,
			MainPictureMime = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
				? contract.MainPictureMime
				: null,
			Members = contract.Members,
			Names = contract.Names,
			Pictures = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
				? contract.Pictures
				: null,
			ReleaseDate = contract.ReleaseDate,
			TranslatedName = contract.TranslatedName,
			WebLinks = contract.WebLinks,
		};
	}
}
