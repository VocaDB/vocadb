#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForSongRefContract : ObjectRefContract
	{
		public AlbumForSongRefContract()
		{
			DiscNumber = 1;
		}

		public AlbumForSongRefContract(SongInAlbum songInAlbum)
		{
			DiscNumber = songInAlbum.DiscNumber;
			TrackNumber = songInAlbum.TrackNumber;

			var album = songInAlbum.Album;
			Id = album.Id;
			NameHint = album.DefaultName;
		}

		[DataMember]
		public int DiscNumber { get; init; }

		[DataMember]
		public int TrackNumber { get; init; }
	}
}
