using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record CreateArtistForApiContract
{
	[DataMember]
	public ArtistType ArtistType { get; init; }

	[DataMember]
	public string Description { get; init; }

	[DataMember]
	public bool Draft { get; init; }

	[DataMember]
	public LocalizedStringContract[] Names { get; init; }

	[DataMember]
	public EntryPictureFileContract? PictureData { get; set; }

	[DataMember]
	public WebLinkForApiContract? WebLink { get; init; }

	public CreateArtistForApiContract()
	{
		Description = string.Empty;
		Names = Array.Empty<LocalizedStringContract>();
	}
}
