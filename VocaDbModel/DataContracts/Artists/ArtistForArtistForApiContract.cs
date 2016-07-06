using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForArtistForApiContract {

		public ArtistForArtistForApiContract() { }

		public ArtistForArtistForApiContract(ArtistForArtist link, LinkDirection direction, ContentLanguagePreference languagePreference) {
			Artist = new ArtistContract(link.GetArtist(direction), languagePreference);
			LinkType = link.LinkType;
		}

		[DataMember]
		public ArtistContract Artist { get; set; }

		[DataMember]
		public ArtistLinkType LinkType { get; set; }

	}

}
