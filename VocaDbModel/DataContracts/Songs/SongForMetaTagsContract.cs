using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.Songs;

/// <summary>
/// Minimal song data contract for server-side rendering meta tags (OpenGraph, etc.).
/// Contains only the fields needed for SEO and social media previews.
/// </summary>
public sealed record SongForMetaTagsContract
{
	public string ArtistString { get; init; }

	public bool Deleted { get; init; }

	public string Name { get; init; }

	public EnglishTranslatedStringContract Notes { get; init; }

	public DateTime? PublishDate { get; init; }

	public SongType SongType { get; init; }

	public string? ThumbUrlMaxSize { get; init; }

	public SongForMetaTagsContract(
		Song song,
		ContentLanguagePreference languagePreference
	)
	{
		ArtistString = song.ArtistString[languagePreference];
		Deleted = song.Deleted;
		Name = song.TranslatedName[languagePreference];
		Notes = new EnglishTranslatedStringContract(song.Notes);
		PublishDate = song.PublishDate;
		SongType = song.SongType;

		// Get thumbnail URL for OpenGraph image from PVs
		var thumbUrl = VideoServiceHelper.GetThumbUrlPreferNotNico(song.PVs.PVs);
		ThumbUrlMaxSize = VideoServiceHelper.GetMaxSizeThumbUrl(song.PVs.PVs) ?? thumbUrl;
	}
}
