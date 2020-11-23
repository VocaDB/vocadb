using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Albums
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistForAlbumContract : ObjectRefContract
	{
		public ArchivedArtistForAlbumContract() { }

		public ArchivedArtistForAlbumContract(ArtistForAlbum entry)
		{
			Roles = entry.Roles;
			IsSupport = entry.IsSupport;

			if (entry.Artist != null)
			{
				Id = entry.Artist.Id;
				NameHint = entry.Artist.DefaultName;
			}
			else
			{
				NameHint = entry.Name;
			}
		}

		[DataMember]
		public bool IsSupport { get; set; }

		[DataMember]
		public ArtistRoles Roles { get; set; }
	}
}
