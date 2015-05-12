
module vdb.dataContracts {

    export interface SongContract {

        additionalNames: string;

        artistString: string;

        id: number;

		lengthSeconds: number;

		name: string;

		// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
		publishDate?: string;

		pvServices: string;

		ratingScore: number;

		songType: string;

		thumbUrl?: string;

    }

}

