#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract]
	public class SongWithAlbumAndPVsContract : SongWithAlbumContract
	{
		public SongWithAlbumAndPVsContract(Song song, ContentLanguagePreference languagePreference, bool getPVs)
			: base(song, languagePreference)
		{
			if (getPVs)
				PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
			else
				PVs = new PVContract[] { };
		}

		[DataMember]
		public PVContract[] PVs { get; init; }
	}
}
