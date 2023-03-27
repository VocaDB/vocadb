using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedEventSeriesForApiContract
{
	[DataMember]
	public required string[]? Aliases { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public required EventCategory Category { get; set; }

	[DataMember]
	public required string Description { get; set; }

	[DataMember]
	public required int Id { get; set; }

	[DataMember]
	public required string? MainPictureMime { get; set; }

	[DataMember]
	public required LocalizedStringContract[]? Names { get; set; }

	[DataMember]
	public required ArchivedTranslatedStringContract TranslatedName { get; set; }

	[DataMember]
	public required ArchivedWebLinkContract[]? WebLinks { get; set; }

	public static ArchivedEventSeriesForApiContract Create(ArchivedEventSeriesContract contract, IUserPermissionContext permissionContext)
	{
		return new()
		{
			Aliases = contract.Aliases,
			Category = contract.Category,
			Description = contract.Description,
			Id = contract.Id,
			MainPictureMime = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
				? contract.MainPictureMime
				: null,
			Names = contract.Names,
			TranslatedName = contract.TranslatedName,
			WebLinks = contract.WebLinks,
		};
	}
}
