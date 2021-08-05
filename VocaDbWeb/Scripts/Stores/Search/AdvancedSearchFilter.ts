// Corresponds to the AdvancedFilterType enum in C#.
export enum AdvancedFilterType {
	Nothing = 'Nothing',

	// Common
	ArtistType = 'ArtistType',
	WebLink = 'WebLink',

	// Artist
	HasUserAccount = 'HasUserAccount',
	RootVoicebank = 'RootVoicebank',
	VoiceProvider = 'VoiceProvider',

	// Album
	HasStoreLink = 'HasStoreLink',
	HasTracks = 'HasTracks',
	NoCoverPicture = 'NoCoverPicture',

	// Song
	HasAlbum = 'HasAlbum',
	HasOriginalMedia = 'HasOriginalMedia',
	HasMedia = 'HasMedia',
	HasMultipleVoicebanks = 'HasMultipleVoicebanks',
	HasPublishDate = 'HasPublishDate',
	Lyrics = 'Lyrics',
	LyricsContent = 'LyricsContent',
}

export default interface AdvancedSearchFilter {
	description?: string;
	filterType: AdvancedFilterType;
	negate?: boolean;
	param: string;
}
