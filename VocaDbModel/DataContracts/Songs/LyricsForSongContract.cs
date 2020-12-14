#nullable disable

using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class LyricsForSongContract
	{
		public LyricsForSongContract() { }

		public LyricsForSongContract(LyricsForSong lyrics, bool includeValue = true)
		{
			ParamIs.NotNull(() => lyrics);

			CultureCode = lyrics.CultureCode.CultureCode;
			Id = lyrics.Id;
			Source = lyrics.Source;
			TranslationType = lyrics.TranslationType;
			URL = lyrics.URL;

			if (includeValue)
			{
				Value = lyrics.Value;
			}
		}

		[DataMember]
		[DefaultValue("")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string CultureCode { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		[DefaultValue("")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string Source { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public TranslationType TranslationType { get; set; }

		[DataMember]
		[DefaultValue("")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string URL { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Value { get; set; }
	}
}
