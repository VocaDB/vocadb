
//module vdb.dataContracts {

    export interface SongContract extends CommonEntryContract {

        additionalNames: string;

        artistString: string;

		lengthSeconds: number;

		// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
		publishDate?: string;

		pvServices: string;

		ratingScore: number;

		songType: string;

		thumbUrl?: string;

    }

//}

