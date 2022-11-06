using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists;

[DataContract(Namespace = Schemas.VocaDb)]
public class ArchivedArtistForArtistContract : ObjectRefContract
{
	public ArchivedArtistForArtistContract() { }

	public ArchivedArtistForArtistContract(ArtistForArtist link)
		: base(link.Parent)
	{
		LinkType = link.LinkType;
	}

	[DataMember]
	public ArtistLinkType LinkType { get; init; }
}
