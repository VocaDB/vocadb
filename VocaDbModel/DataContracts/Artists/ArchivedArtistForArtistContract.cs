using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArchivedArtistForArtistContract : ObjectRefContract {

		public ArchivedArtistForArtistContract() { }

		public ArchivedArtistForArtistContract(GroupForArtist link) 
			: base(link.Group) {

			LinkType = link.LinkType;

		}

		public ArtistLinkType LinkType { get; set; }

	}

}
