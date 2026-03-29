import { OptionalGeoPointContract } from '@/DataContracts/OptionalGeoPointContract';
import { VenueForEditContract } from '@/DataContracts/Venue/VenueForEditContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { VenueRepository } from '@/Repositories/VenueRepository';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export class VenueEditStore {
	@observable address: string;
	@observable addressCountryCode: string;
	@observable defaultNameLanguage: ContentLanguageSelection;
	readonly deleted: boolean;
	readonly deleteStore: DeleteEntryStore;
	@observable description = '';
	@observable duplicateName?: string;
	@observable errors?: Record<string, string[]>;
	@observable latitude?: number;
	@observable longitude?: number;
	readonly name: string;
	readonly names: NamesEditStore;
	@observable status = EntryStatus.Draft;
	@observable submitting = false;
	readonly trashStore: DeleteEntryStore;
	@observable updateNotes = '';
	readonly webLinks: WebLinksEditStore;

	constructor(
		antiforgeryRepo: AntiforgeryRepository,
		private readonly venueRepo: VenueRepository,
		readonly contract: VenueForEditContract,
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				this.venueRepo.delete(requestToken, {
					id: this.contract.id,
					notes: notes,
					hardDelete: false,
				}),
		);

		this.trashStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				this.venueRepo.delete(requestToken, {
					id: this.contract.id,
					notes: notes,
					hardDelete: true,
				}),
		);

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

	@computed get coordinates(): OptionalGeoPointContract | undefined {
		if (!this.latitude || !this.longitude) return undefined;

		return {
			latitude: !isNaN(this.latitude) ? this.latitude : undefined,
			longitude: !isNaN(this.longitude) ? this.longitude : undefined,
		} as OptionalGeoPointContract;
	}

	@action checkName = async (value: string): Promise<void> => {
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

	@action submit = async (requestToken: string): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.venueRepo.edit(requestToken, {
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
				updateNotes: this.updateNotes,
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
