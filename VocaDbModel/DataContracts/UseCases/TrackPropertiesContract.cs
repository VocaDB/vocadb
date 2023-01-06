#nullable disable

using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases;

public class TrackPropertiesContract
{
	public TrackPropertiesContract() { }

	public TrackPropertiesContract(Song song, IEnumerable<Artist> artists, ContentLanguagePreference languagePreference)
	{
		Id = song.Id;
		Name = song.TranslatedName[languagePreference];

		ArtistSelections = artists.Select(a =>
			new ArtistSelectionForTrackContract(a, song.HasArtist(a), languagePreference))
				.OrderBy(a => a.Artist.Name).ToArray();
	}

	public ArtistSelectionForTrackContract[] ArtistSelections { get; init; }

	public int Id { get; init; }

	public string Name { get; init; }
}
