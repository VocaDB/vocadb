#nullable disable

using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service.Search;

public static class AdvancedSearchFilters
{
	public static Tuple<string, AdvancedFilterType, string, bool> Filter(string description, AdvancedFilterType filterType, string param = "", bool negate = false)
	{
		return Tuple.Create(description, filterType, param, negate);
	}

	public static Tuple<string, AdvancedFilterType, string, bool>[] AlbumFilters { get; } = {
		Filter("Artist type: Vocaloid", AdvancedFilterType.ArtistType, ArtistType.Vocaloid.ToString()),
		Filter("Artist type: UTAU", AdvancedFilterType.ArtistType, ArtistType.UTAU.ToString()),
		Filter("Artist type: CeVIO", AdvancedFilterType.ArtistType, ArtistType.CeVIO.ToString()),
		Filter("Artist type: Synthesizer V", AdvancedFilterType.ArtistType, ArtistType.SynthesizerV.ToString()),
		Filter("Artist type: NEUTRINO", AdvancedFilterType.ArtistType, ArtistType.NEUTRINO.ToString()),
		Filter("Artist type: VoiSona", AdvancedFilterType.ArtistType, ArtistType.VoiSona.ToString()),
		Filter("Artist type: New Type", AdvancedFilterType.ArtistType, ArtistType.NewType.ToString()),
		Filter("Artist type: Voiceroid", AdvancedFilterType.ArtistType, ArtistType.Voiceroid.ToString()),
		Filter("Artist type: VOICEVOX", AdvancedFilterType.ArtistType, ArtistType.VOICEVOX.ToString()),
		Filter("Artist type: ACE Virtual Singer", AdvancedFilterType.ArtistType, ArtistType.ACEVirtualSinger.ToString()),
		Filter("Artist type: A.I.VOICE", AdvancedFilterType.ArtistType, ArtistType.AIVOICE.ToString()),
		Filter("Artist type: other voice synthesizer", AdvancedFilterType.ArtistType, ArtistType.OtherVoiceSynthesizer.ToString()),
		Filter("No cover picture", AdvancedFilterType.NoCoverPicture),
		Filter("With store link", AdvancedFilterType.HasStoreLink),
		Filter("No tracks", AdvancedFilterType.HasTracks, negate: true)
	};

	public static Tuple<string, AdvancedFilterType, string, bool>[] ArtistFilters { get; } = {
		Filter("Voice provider of: any voicebank", AdvancedFilterType.VoiceProvider),
		Filter("Voice provider of: Vocaloid", AdvancedFilterType.VoiceProvider, ArtistType.Vocaloid.ToString()),
		Filter("Voice provider of: UTAU", AdvancedFilterType.VoiceProvider, ArtistType.UTAU.ToString()),
		Filter("Voice provider of: CeVIO", AdvancedFilterType.VoiceProvider, ArtistType.CeVIO.ToString()),
		Filter("Voice provider of: Synthesizer V", AdvancedFilterType.VoiceProvider, ArtistType.SynthesizerV.ToString()),
		Filter("Voice provider of: NEUTRINO", AdvancedFilterType.VoiceProvider, ArtistType.NEUTRINO.ToString()),
		Filter("Voice provider of: VoiSona", AdvancedFilterType.VoiceProvider, ArtistType.VoiSona.ToString()),
		Filter("Voice provider of: New Type", AdvancedFilterType.VoiceProvider, ArtistType.NewType.ToString()),
		Filter("Voice provider of: Voiceroid", AdvancedFilterType.VoiceProvider, ArtistType.Voiceroid.ToString()),
		Filter("Voice provider of: VOICEVOX", AdvancedFilterType.VoiceProvider, ArtistType.VOICEVOX.ToString()),
		Filter("Voice provider of: ACE Virtual Singer", AdvancedFilterType.VoiceProvider, ArtistType.ACEVirtualSinger.ToString()),
		Filter("Voice provider of: A.I.VOICE", AdvancedFilterType.VoiceProvider, ArtistType.AIVOICE.ToString()),
		Filter("Voice provider of: other voice synthesizer", AdvancedFilterType.VoiceProvider, ArtistType.OtherVoiceSynthesizer.ToString()),
		Filter("Root voicebank (no base)", AdvancedFilterType.RootVoicebank),
		Filter("Derived voicebank", AdvancedFilterType.RootVoicebank, negate: true),
		Filter("User account on VocaDB", AdvancedFilterType.HasUserAccount)
	};

	public static Tuple<string, AdvancedFilterType, string, bool>[] SongFilters { get; } = {
		Filter("Artist type: Vocaloid", AdvancedFilterType.ArtistType, ArtistType.Vocaloid.ToString()),
		Filter("Artist type: UTAU", AdvancedFilterType.ArtistType, ArtistType.UTAU.ToString()),
		Filter("Artist type: CeVIO", AdvancedFilterType.ArtistType, ArtistType.CeVIO.ToString()),
		Filter("Artist type: Synthesizer V", AdvancedFilterType.ArtistType, ArtistType.SynthesizerV.ToString()),
		Filter("Artist type: NEUTRINO", AdvancedFilterType.ArtistType, ArtistType.NEUTRINO.ToString()),
		Filter("Artist type: VoiSona", AdvancedFilterType.ArtistType, ArtistType.VoiSona.ToString()),
		Filter("Artist type: New Type", AdvancedFilterType.ArtistType, ArtistType.NewType.ToString()),
		Filter("Artist type: Voiceroid", AdvancedFilterType.ArtistType, ArtistType.Voiceroid.ToString()),
		Filter("Artist type: VOICEVOX", AdvancedFilterType.ArtistType, ArtistType.VOICEVOX.ToString()),
		Filter("Artist type: ACE Virtual Singer", AdvancedFilterType.ArtistType, ArtistType.ACEVirtualSinger.ToString()),
		Filter("Artist type: A.I.VOICE", AdvancedFilterType.ArtistType, ArtistType.AIVOICE.ToString()),
		Filter("Artist type: other voice synthesizer", AdvancedFilterType.ArtistType, ArtistType.OtherVoiceSynthesizer.ToString()),
		Filter("Multiple voicebanks", AdvancedFilterType.HasMultipleVoicebanks),
		Filter("Lyrics: Any language", AdvancedFilterType.Lyrics, AdvancedSearchFilter.Any),
		Filter("Lyrics: Japanese", AdvancedFilterType.Lyrics, OptionalCultureCode.LanguageCode_Japanese),
		Filter("Lyrics: Chinese", AdvancedFilterType.Lyrics, "zh"),
		Filter("Lyrics: English", AdvancedFilterType.Lyrics, OptionalCultureCode.LanguageCode_English),
		Filter("Lyrics: Other/unspecified language", AdvancedFilterType.Lyrics, string.Empty),
		Filter("Has publish date", AdvancedFilterType.HasPublishDate),
		Filter("Album song", AdvancedFilterType.HasAlbum),
		Filter("Standalone (no album)", AdvancedFilterType.HasAlbum, negate: true),
		Filter("No original media", AdvancedFilterType.HasOriginalMedia, negate: true),
		Filter("No media", AdvancedFilterType.HasMedia, negate: true)
	};
}
