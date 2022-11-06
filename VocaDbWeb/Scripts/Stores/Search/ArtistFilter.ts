import { ArtistType } from '@/Models/Artists/ArtistType';
import { makeObservable, observable } from 'mobx';

export class ArtistFilter {
	@observable artistType?: ArtistType = undefined;
	@observable name?: string = undefined;

	constructor(readonly id: number) {
		makeObservable(this);
	}
}
