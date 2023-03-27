using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Albums;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedAlbumForApiContract
{
	[DataMember]
	public required ArchivedArtistForAlbumContract[]? Artists { get; set; }

	[DataMember]
	public required string? Description { get; set; }

	[DataMember]
	public required string? DescriptionEng { get; set; }

	[DataMember]
	public required AlbumDiscPropertiesContract[]? Discs { get; set; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public required DiscType DiscType { get; set; }

	[DataMember]
	public required int Id { get; set; }

	[DataMember]
	public required AlbumIdentifierContract[] Identifiers { get; set; }

	[DataMember]
	public required string? MainPictureMime { get; set; }

	[DataMember]
	public required LocalizedStringContract[]? Names { get; set; }

	[DataMember]
	public required ArchivedAlbumReleaseContract? OriginalRelease { get; set; }

	[DataMember]
	public required ArchivedEntryPictureFileContract[]? Pictures { get; set; }

	[DataMember]
	[JsonProperty("pvs")]
	public required ArchivedPVContract[]? PVs { get; set; }

	[DataMember]
	public required SongInAlbumRefContract[]? Songs { get; set; }

	[DataMember]
	public required TranslatedStringContract TranslatedName { get; set; }

	[DataMember]
	public required ArchivedWebLinkContract[]? WebLinks { get; set; }

	public static ArchivedAlbumForApiContract Create(ArchivedAlbumContract contract, IUserPermissionContext permissionContext)
	{
		return new()
		{
			Artists = contract.Artists,
			Description = contract.Description,
			DescriptionEng = contract.DescriptionEng,
			Discs = contract.Discs,
			DiscType = contract.DiscType,
			Id = contract.Id,
			Identifiers = contract.Identifiers,
			MainPictureMime = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
				? contract.MainPictureMime
				: null,
			Names = contract.Names,
			OriginalRelease = contract.OriginalRelease,
			Pictures = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
				? contract.Pictures
				: null,
			PVs = permissionContext.HasPermission(PermissionToken.ViewOtherPVs)
				? contract.PVs
				: contract.PVs?.Where(pv => pv.PVType == PVType.Original).ToArray(),
			Songs = contract.Songs,
			TranslatedName = contract.TranslatedName,
			WebLinks = contract.WebLinks,
		};
	}
}
