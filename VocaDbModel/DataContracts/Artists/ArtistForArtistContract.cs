#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForArtistContract
	{
		public ArtistForArtistContract() { }

		public ArtistForArtistContract(ArtistForArtist groupForArtist, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => groupForArtist);

			Parent = new ArtistContract(groupForArtist.Parent, languagePreference);
			Id = groupForArtist.Id;
			LinkType = groupForArtist.LinkType;
			Member = new ArtistContract(groupForArtist.Member, languagePreference);
		}

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public ArtistLinkType LinkType { get; init; }

		[DataMember]
		public ArtistContract Member { get; init; }

		[DataMember]
		public ArtistContract Parent { get; init; }
	}
}
