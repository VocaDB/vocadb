
module vdb.dataContracts {

    export interface SongContract {

        additionalNames: string;

        artistString: string;

        id: number;

		lengthSeconds: number;

		name: string;

		publishDate?: string;

		pvServices: string;

		ratingScore: number;

		songType: string;

		thumbUrl?: string;

    }

}

