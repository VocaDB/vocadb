using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract(Namespace = Schemas.VocaDb)]
public class LyricsForSongContract
{
#nullable disable
	public LyricsForSongContract() { }
#nullable enable

	public LyricsForSongContract(LyricsForSong lyrics, bool includeValue = true)
	{
		ParamIs.NotNull(() => lyrics);

		CultureCodes = lyrics.CultureCodes.Select(c => c.CultureCode).ToArray();
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
	[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
	public string[]? CultureCodes { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	[DefaultValue("")]
	[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
	public string Source { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public TranslationType TranslationType { get; init; }

	[DataMember]
	[DefaultValue("")]
	[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
	public string URL { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public string? Value { get; init; }
}
