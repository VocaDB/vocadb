using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongWithAlbumContract : SongContract
	{

		public SongWithAlbumContract(Song song, ContentLanguagePreference languagePreference)
			: base(song, languagePreference)
		{

			Album = (song.Albums.Any() ? new AlbumContract(song.Albums.First().Album, languagePreference) : null);

		}

		[DataMember]
		public AlbumContract Album { get; set; }

	}

}
