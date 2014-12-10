
module vdb.dataContracts {
	
	export interface SongApiContract extends SongContract, EntryWithTagUsagesContract {

		// Not returned from the API, but can be used to cache the list of PV services client side
		pvServicesArray?: vdb.models.pvs.PVService[];

	}

}