using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{

	[DataContract]
	public class SongWithPVAndVoteContract : SongContract
	{

		public SongWithPVAndVoteContract(Song song, SongVoteRating vote, ContentLanguagePreference languagePreference, bool includePVs = true)
			: base(song, languagePreference)
		{

			if (includePVs)
			{
				PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
			}
			Vote = vote;

		}

		[DataMember]
		public PVContract[] PVs { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongVoteRating Vote { get; set; }

	}
}
