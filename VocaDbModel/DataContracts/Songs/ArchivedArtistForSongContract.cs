#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistForSongContract : ObjectRefContract
	{
		public ArchivedArtistForSongContract() { }

		public ArchivedArtistForSongContract(ArtistForSong entry)
		{
			IsSupport = entry.IsSupport;
			Roles = entry.Roles;

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
