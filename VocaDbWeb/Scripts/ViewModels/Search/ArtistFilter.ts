
module vdb.viewModels.search {
	
	export interface IArtistFilter extends vdb.models.IEntryWithIdAndName {
		
		artistType: vdb.models.artists.ArtistType;

	}

} 