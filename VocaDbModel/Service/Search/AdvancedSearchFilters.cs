using System;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service.Search {

	public static class AdvancedSearchFilters {

		public static Tuple<string, AdvancedFilterType, string, bool> Filter(string description, AdvancedFilterType filterType, string param = "", bool negate = false) {
			return Tuple.Create(description, filterType, param, negate);
		}

		public static Tuple<string, AdvancedFilterType, string, bool>[] AlbumFilters { get; } = {
			Filter("Artist type: Vocaloid", AdvancedFilterType.ArtistType, ArtistType.Vocaloid.ToString()),
			Filter("Artist type: UTAU", AdvancedFilterType.ArtistType, ArtistType.UTAU.ToString()),
			Filter("Artist type: CeVIO", AdvancedFilterType.ArtistType, ArtistType.CeVIO.ToString()),
			Filter("Artist type: other voice synthesizer", AdvancedFilterType.ArtistType, ArtistType.OtherVoiceSynthesizer.ToString()),
			Filter("No cover picture", AdvancedFilterType.NoCoverPicture),
			Filter("With store link", AdvancedFilterType.HasStoreLink),
		};

		public static Tuple<string, AdvancedFilterType, string, bool>[] ArtistFilters { get; } = {
			Filter("Voice provider of: any voicebank", AdvancedFilterType.VoiceProvider),
			Filter("Voice provider of: Vocaloid", AdvancedFilterType.VoiceProvider, ArtistType.Vocaloid.ToString()),
			Filter("Voice provider of: UTAU", AdvancedFilterType.VoiceProvider, ArtistType.UTAU.ToString()),
			Filter("Voice provider of: CeVIO", AdvancedFilterType.VoiceProvider, ArtistType.CeVIO.ToString()),
			Filter("Voice provider of: other voice synthesizer", AdvancedFilterType.VoiceProvider, ArtistType.OtherVoiceSynthesizer.ToString()),
			Filter("Root voicebank (no base)", AdvancedFilterType.RootVoicebank),
			Filter("Derived voicebank", AdvancedFilterType.RootVoicebank, negate: true),
			Filter("User account on VocaDB", AdvancedFilterType.HasUserAccount)
		};

		public static Tuple<string, AdvancedFilterType, string, bool>[] SongFilters { get; } = {
			Filter("Artist type: Vocaloid", AdvancedFilterType.ArtistType, ArtistType.Vocaloid.ToString()),
			Filter("Artist type: UTAU", AdvancedFilterType.ArtistType, ArtistType.UTAU.ToString()),
			Filter("Artist type: CeVIO", AdvancedFilterType.ArtistType, ArtistType.CeVIO.ToString()),
			Filter("Artist type: other voice synthesizer", AdvancedFilterType.ArtistType, ArtistType.OtherVoiceSynthesizer.ToString()),
			Filter("Multiple voicebanks", AdvancedFilterType.HasMultipleVoicebanks),
			Filter("Lyrics: Any language", AdvancedFilterType.Lyrics, AdvancedSearchFilter.Any),
			Filter("Lyrics: Japanese", AdvancedFilterType.Lyrics, OptionalCultureCode.LanguageCode_Japanese),
			Filter("Lyrics: English", AdvancedFilterType.Lyrics, OptionalCultureCode.LanguageCode_English),
			Filter("Lyrics: Other/unspecified language", AdvancedFilterType.Lyrics, string.Empty),
			Filter("Has publish date", AdvancedFilterType.HasPublishDate),
			Filter("Album song", AdvancedFilterType.HasAlbum),
			Filter("Standalone (no album)", AdvancedFilterType.HasAlbum, negate: true),
			Filter("No original media", AdvancedFilterType.HasOriginalMedia, negate: true),
			Filter("No media", AdvancedFilterType.HasMedia, negate: true)
		};

	}
}
