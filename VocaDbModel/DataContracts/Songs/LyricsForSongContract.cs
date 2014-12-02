using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class LyricsForSongContract : ILocalizedString {

		public LyricsForSongContract() { }

		public LyricsForSongContract(LyricsForSong lyrics) {
			
			ParamIs.NotNull(() => lyrics);

			Id = lyrics.Id;
			Language = lyrics.Language;
			Source = lyrics.Source;
			Value = lyrics.Value;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection Language { get; set; }

		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public string Value { get; set; }

	}
}
