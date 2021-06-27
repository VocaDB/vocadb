import GlobalValues from '@Shared/GlobalValues';

declare global {
	const vdb: {
		resources: {
			albumEdit: any;
			entryEdit: any;
			shared: any;
			song: any;
			album: { addedToCollection?: string };
			albumDetails: { download: string };
		};
		values: GlobalValues;
	};
}
