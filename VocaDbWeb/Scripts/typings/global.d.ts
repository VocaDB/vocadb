import GlobalValues from '@Shared/GlobalValues';

declare global {
	const vdb: {
		resources: {
			albumEdit: any;
			entryEdit: any;
			shared: any;
			song: any | /* TODO: Remove. */ { rankingsTitle?: string };
			album: { addedToCollection?: string };
			albumDetails: { download: string };
			artist: { authoredBy?: string; newArtistExternalLink?: string };
			layout: { paypalDonateTitle?: string };
			home: { welcome?: string; welcomeSubtitle?: string };
		};
		values: GlobalValues;
	};
}
