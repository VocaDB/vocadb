import OptionalGeoPointContract from '@/DataContracts/OptionalGeoPointContract';
import VenueForEditContract from '@/DataContracts/Venue/VenueForEditContract';
import EntryStatus from '@/Models/EntryStatus';
import NameMatchMode from '@/Models/NameMatchMode';
import VenueRepository from '@/Repositories/VenueRepository';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import DeleteEntryStore from '../DeleteEntryStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import WebLinksEditStore from '../WebLinksEditStore';

export default class VenueEditStore {
	@observable public address: string;
	@observable public addressCountryCode: string;
	@observable public defaultNameLanguage: string /* TODO: enum */;
	public readonly deleted: boolean;
	public readonly deleteStore = new DeleteEntryStore((notes) =>
		this.venueRepo.delete({
			id: this.contract.id,
			notes: notes,
			hardDelete: false,
		}),
	);
	@observable public description = '';
	@observable public duplicateName?: string;
	@observable public errors?: Record<string, string[]>;
	@observable public latitude?: number;
	@observable public longitude?: number;
	public readonly name: string;
	public readonly names: NamesEditStore;
	@observable public status = EntryStatus[EntryStatus.Draft];
	@observable public submitting = false;
	public readonly trashStore = new DeleteEntryStore((notes) =>
		this.venueRepo.delete({
			id: this.contract.id,
			notes: notes,
			hardDelete: true,
		}),
	);
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		private readonly venueRepo: VenueRepository,
		public readonly contract: VenueForEditContract,
	) {
		makeObservable(this);

		this.address = contract.address;
		this.addressCountryCode = contract.addressCountryCode;
		this.deleted = contract.deleted;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = contract.description;
		this.latitude = contract.coordinates?.latitude ?? undefined;
		this.longitude = contract.coordinates?.longitude ?? undefined;
		this.name = contract.name;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.status = contract.status;
		this.webLinks = new WebLinksEditStore(contract.webLinks);
	}

	@computed public get coordinates(): OptionalGeoPointContract | undefined {
		if (!this.latitude || !this.longitude) return undefined;

		return {
			latitude: !isNaN(this.latitude) ? this.latitude : undefined,
			longitude: !isNaN(this.longitude) ? this.longitude : undefined,
		};
	}

	@action public checkName = async (value: string): Promise<void> => {
		if (!value) {
			this.duplicateName = undefined;
			return;
		}

		const result = await this.venueRepo.getList({
			query: value,
			nameMatchMode: NameMatchMode.Exact,
			maxResults: 1,
		});

		runInAction(() => {
			this.duplicateName = result.items.length ? value : undefined;
		});
	};

	@action public submit = async (): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.venueRepo.edit({
				address: this.address,
				addressCountryCode: this.addressCountryCode,
				coordinates: this.coordinates,
				defaultNameLanguage: this.defaultNameLanguage,
				deleted: false,
				description: this.description,
				id: this.contract.id,
				name: '',
				names: this.names.toContracts(),
				status: this.status,
				webLinks: this.webLinks.toContracts(),
			});

			return id;
		} catch (error: any) {
			if (error.response) {
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
