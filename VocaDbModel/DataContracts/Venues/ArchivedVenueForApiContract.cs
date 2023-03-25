using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Venues;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedVenueForApiContract
{
	[DataMember]
	public required string Address { get; set; }

	[DataMember]
	public required string AddressCountryCode { get; set; }

	[DataMember]
	public required OptionalGeoPointContract Coordinates { get; set; }

	[DataMember]
	public required string Description { get; set; }

	[DataMember]
	public required int Id { get; set; }

	[DataMember]
	public required LocalizedStringContract[]? Names { get; set; }

	[DataMember]
	public required ArchivedTranslatedStringContract TranslatedName { get; set; }

	[DataMember]
	public required ArchivedWebLinkContract[]? WebLinks { get; set; }

	public static ArchivedVenueForApiContract Create(ArchivedVenueContract contract, IUserPermissionContext permissionContext)
	{
		return new()
		{
			Address = contract.Address,
			AddressCountryCode = contract.AddressCountryCode,
			Coordinates = contract.Coordinates,
			Description = contract.Description,
			Id = contract.Id,
			Names = contract.Names,
			TranslatedName = contract.TranslatedName,
			WebLinks = contract.WebLinks,
		};
	}
}
