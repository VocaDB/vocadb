using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedEventForApiContract
{
	[DataMember]
	public required ArchivedArtistForEventContract[]? Artists { get; set; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public required EventCategory Category { get; set; }

	[DataMember]
	public required DateTime? Date { get; set; }

	[DataMember]
	public required string Description { get; set; }

	[DataMember]
	public required int Id { get; set; }

	[DataMember]
	public required string? MainPictureMime { get; set; }

	[DataMember]
	public required string? Name { get; init; }

	[DataMember]
	public required LocalizedStringContract[]? Names { get; set; }

	[DataMember]
	[JsonProperty("pvs")]
	public required ArchivedPVContract[]? PVs { get; set; }

	[DataMember]
	public required ObjectRefContract? Series { get; set; }

	[DataMember]
	public required int SeriesNumber { get; set; }

	[DataMember]
	public required ObjectRefContract? SongList { get; set; }

	[DataMember]
	public required ArchivedTranslatedStringContract TranslatedName { get; set; }

	[DataMember]
	public required ObjectRefContract? Venue { get; set; }

	[DataMember]
	public required string? VenueName { get; set; }

	[DataMember]
	public required ArchivedWebLinkContract[]? WebLinks { get; set; }

	public static ArchivedEventForApiContract Create(ArchivedEventContract contract, IUserPermissionContext permissionContext)
	{
		return new()
		{
			Artists = contract.Artists,
			Category = contract.Category,
			Date = contract.Date,
			Description = contract.Description,
			Id = contract.Id,
			MainPictureMime = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
				? contract.MainPictureMime
				: null,
			Name = contract.Name,
			Names = contract.Names,
			PVs = permissionContext.HasPermission(PermissionToken.ViewOtherPVs)
				? contract.PVs
				: contract.PVs?.Where(pv => pv.PVType == PVType.Original).ToArray(),
			Series = contract.Series,
			SeriesNumber = contract.SeriesNumber,
			SongList = contract.SongList,
			TranslatedName = contract.TranslatedName,
			Venue = contract.Venue,
			VenueName = contract.VenueName,
			WebLinks = contract.WebLinks,
		};
	}
}
