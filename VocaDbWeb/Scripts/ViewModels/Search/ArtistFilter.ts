
//module vdb.viewModels.search {
	
	export class ArtistFilter {
		
		constructor(public id: number) {}

		artistType = ko.observable<vdb.models.artists.ArtistType>(null);

		name = ko.observable<string>(null);

	}

//} 