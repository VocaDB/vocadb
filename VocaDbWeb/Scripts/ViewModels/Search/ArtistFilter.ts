import ArtistType from '../../Models/Artists/ArtistType';

//module vdb.viewModels.search {
	
	export default class ArtistFilter {
		
		constructor(public id: number) {}

		artistType = ko.observable<ArtistType>(null);

		name = ko.observable<string>(null);

	}

//} 