#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumContract
	{
		public SongInAlbumContract() { }

#nullable enable
		public SongInAlbumContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference, bool getThumbUrl = true,
			SongVoteRating? rating = null)
		{
			ParamIs.NotNull(() => songInAlbum);

			DiscNumber = songInAlbum.DiscNumber;
			Id = songInAlbum.Id;
			TrackNumber = songInAlbum.TrackNumber;

			var song = songInAlbum.Song;
			Song = song != null ? new SongContract(song, languagePreference, getThumbUrl) : null;
			Name = Song != null ? Song.Name : songInAlbum.Name;
			Rating = rating;
		}
#nullable disable

		[DataMember]
		public int DiscNumber { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public SongVoteRating? Rating { get; init; }

		[DataMember]
		public SongContract Song { get; init; }

		[DataMember]
		public int TrackNumber { get; init; }

		public override string ToString()
		{
			return $"({DiscNumber}.{TrackNumber}) {Song} in album";
		}
	}
}
