#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumForApiContract
	{
		public SongInAlbumForApiContract() { }

		public SongInAlbumForApiContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference, SongOptionalFields fields)
		{
			ParamIs.NotNull(() => songInAlbum);

			DiscNumber = songInAlbum.DiscNumber;
			Id = songInAlbum.Id;
			TrackNumber = songInAlbum.TrackNumber;

			var song = songInAlbum.Song;
			Song = song != null ? new SongForApiContract(song, null, languagePreference, fields) : null;
			Name = Song != null ? Song.Name : songInAlbum.Name;
		}

		[DataMember]
		public int DiscNumber { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public SongForApiContract Song { get; init; }

		[DataMember]
		public int TrackNumber { get; init; }
	}
}
