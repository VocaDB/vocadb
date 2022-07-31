using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record CreateAlbumForApiContract
{
	[DataMember]
	public ArtistContract[] Artists { get; init; }

	[DataMember]
	public DiscType DiscType { get; init; }

	[DataMember]
	public LocalizedStringContract[] Names { get; init; }

	public CreateAlbumForApiContract()
	{
		Artists = Array.Empty<ArtistContract>();
		Names = Array.Empty<LocalizedStringContract>();
	}
}
