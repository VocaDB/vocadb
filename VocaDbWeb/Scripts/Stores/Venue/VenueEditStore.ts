import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import OptionalGeoPointContract from '../../DataContracts/OptionalGeoPointContract';
import VenueForEditContract from '../../DataContracts/Venue/VenueForEditContract';
import NameMatchMode from '../../Models/NameMatchMode';
import VenueRepository from '../../Repositories/VenueRepository';
import NamesEditStore from '../Globalization/NamesEditStore';
import WebLinksEditStore from '../WebLinksEditStore';

export default class VenueEditStore {
	@observable public address: string;
	@observable public addressCountryCode: string;
	@observable public defaultNameLanguage: string;
	@observable public description: string;
	@observable public duplicateName?: string;
	private readonly id: number;
	@observable public latitude?: number;
	@observable public longitude?: number;
	public readonly names: NamesEditStore;
	@observable public submitting = false;
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		private readonly venueRepo: VenueRepository,
		contract: VenueForEditContract,
	) {
		makeObservable(this);

		this.address = contract.address;
		this.addressCountryCode = contract.addressCountryCode;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.id = contract.id;
		this.latitude = contract.coordinates?.latitude ?? undefined;
		this.longitude = contract.coordinates?.longitude ?? undefined;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.webLinks = new WebLinksEditStore(contract.webLinks);

		// TODO
	}

	@computed public get coordinates(): OptionalGeoPointContract | undefined {
		if (!this.latitude || !this.longitude) return undefined;

		return {
			latitude: !isNaN(this.latitude) ? this.latitude : undefined,
			longitude: !isNaN(this.longitude) ? this.longitude : undefined,
		};
	}

	private checkName = (value: string): void => {
		if (!value) {
			this.duplicateName = undefined;
			return;
		}

		this.venueRepo
			.getList({
				query: value,
				nameMatchMode: NameMatchMode.Exact,
				maxResults: 1,
			})
			.then((result) => {
				runInAction(() => {
					this.duplicateName = result.items.length ? value : undefined;
				});
			});
	};

	@action public submit = (): boolean => {
		this.submitting = true;
		return true;
	};
}
