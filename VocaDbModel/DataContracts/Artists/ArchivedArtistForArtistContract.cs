using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
	[JsonConverter(typeof(StringEnumConverter))]
	public ArtistLinkType LinkType { get; init; }
}
