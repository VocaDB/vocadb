export interface GlobalResources {
	albumEdit: any;
	entryEdit: any;
	shared: any;
	song:
		| any /* TODO: Remove. */
		| { newSongInfo?: string; rankingsTitle?: string };
	album: {
		addedToCollection?: string;
		newAlbumArtistDesc?: string;
		newAlbumInfo?: string;
	};
	albumDetails: { download: string };
	artist: { authoredBy?: string; newArtistExternalLink?: string };
	layout: { paypalDonateTitle?: string };
	home: { welcome?: string; welcomeSubtitle?: string };
	user: { requestVerificationInfo?: string };
}
