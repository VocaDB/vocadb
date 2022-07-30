using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record CreateSongForApiContract
{
	[DataMember]
	public ArtistForSongContract[] Artists { get; init; }

	[DataMember]
	public bool Draft { get; init; }

	[DataMember]
	public LyricsForSongContract[] Lyrics { get; init; }

	[DataMember]
	public LocalizedStringContract[] Names { get; init; }

	[DataMember]
	public SongContract? OriginalVersion { get; init; }

	[DataMember]
	public string[] PVUrls { get; set; }

	[DataMember]
	public string ReprintPVUrl { get; init; }

	[DataMember]
	public SongType SongType { get; set; }

	[DataMember]
	public string UpdateNotes { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public CreateSongForApiContract()
	{
		Artists = Array.Empty<ArtistForSongContract>();
		Lyrics = Array.Empty<LyricsForSongContract>();
		Names = Array.Empty<LocalizedStringContract>();
		PVUrls = Array.Empty<string>();
		ReprintPVUrl = string.Empty;
		UpdateNotes = string.Empty;
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}
}
