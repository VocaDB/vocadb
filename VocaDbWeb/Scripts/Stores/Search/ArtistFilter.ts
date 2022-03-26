import ArtistType from '@Models/Artists/ArtistType';
import { makeObservable, observable } from 'mobx';

export default class ArtistFilter {
	@observable public artistType?: ArtistType = undefined;
	@observable public name?: string = undefined;

	public constructor(public readonly id: number) {
		makeObservable(this);
	}
}
