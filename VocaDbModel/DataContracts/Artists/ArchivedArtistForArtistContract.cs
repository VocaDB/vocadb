using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists
{

	public class ArchivedArtistForArtistContract : ObjectRefContract
	{

		public ArchivedArtistForArtistContract() { }

		public ArchivedArtistForArtistContract(ArtistForArtist link)
			: base(link.Parent)
		{

			LinkType = link.LinkType;

		}

		public ArtistLinkType LinkType { get; set; }

	}

}
