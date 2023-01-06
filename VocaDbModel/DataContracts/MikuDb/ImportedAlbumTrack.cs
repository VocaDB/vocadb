#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.MikuDb;

[DataContract(Namespace = Schemas.VocaDb)]
public class ImportedAlbumTrack
{
	public ImportedAlbumTrack()
	{
		DiscNum = 1;
		ArtistNames = VocalistNames = Array.Empty<string>();
	}

	[DataMember]
	public string[] ArtistNames { get; set; }

	public string ArtistString
	{
		get
		{
			var producers = ArtistNames.Select(n => new CustomArtist(n, ArtistRoles.Composer));
			var vocalists = VocalistNames.Select(n => new CustomArtist(n, ArtistRoles.Vocalist));
			return ArtistHelper.GetArtistString(producers.Concat(vocalists), ContentFocus.Music).Default;
		}
	}

	[DataMember]
	public int DiscNum { get; init; }

	[DataMember]
	public string Title { get; set; }

	[DataMember]
	public int TrackNum { get; set; }

	[DataMember]
	public string[] VocalistNames { get; set; }

#nullable enable
	public override string ToString()
	{
		return $"Imported track {Title} ({ArtistString})";
	}
#nullable disable
}
