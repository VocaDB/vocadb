using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForArtistForApiContract {

		public ArtistForArtistForApiContract() { }

		public ArtistForArtistForApiContract(ArtistForArtist link, ContentLanguagePreference languagePreference) {
			Parent = new ArtistContract(link.Parent, languagePreference);
			LinkType = link.LinkType;
		}

		[DataMember]
		public ArtistLinkType LinkType { get; set; }

		[DataMember]
		public ArtistContract Parent { get; set; }


	}

}
