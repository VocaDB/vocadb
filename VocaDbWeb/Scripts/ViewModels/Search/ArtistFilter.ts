import ArtistType from '@Models/Artists/ArtistType';
import ko from 'knockout';

export default class ArtistFilter {
	public constructor(public id: number) {}

	public artistType = ko.observable<ArtistType>(null!);

	public name = ko.observable<string>(null!);
}
