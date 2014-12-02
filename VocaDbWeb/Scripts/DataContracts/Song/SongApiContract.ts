
module vdb.dataContracts {
	
	export interface SongApiContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		artistString: string;

		id: number;

		name: string;

		pvServices: string;

		// Not returned from the API, but can be used to cache the list of PV services client side
		pvServicesArray?: vdb.models.pvs.PVService[];

		ratingScore: number;

		thumbUrl: string;

	}

}