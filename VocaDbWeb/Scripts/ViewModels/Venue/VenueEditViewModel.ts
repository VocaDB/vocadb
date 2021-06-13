import OptionalGeoPointContract from '@DataContracts/OptionalGeoPointContract';
import VenueForEditContract from '@DataContracts/Venue/VenueForEditContract';
import EntryType from '@Models/EntryType';
import NameMatchMode from '@Models/NameMatchMode';
import UserRepository from '@Repositories/UserRepository';
import VenueRepository from '@Repositories/VenueRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import UrlMapper from '@Shared/UrlMapper';
import ko, { Computed, Observable } from 'knockout';
import _ from 'lodash';

import DeleteEntryViewModel from '../DeleteEntryViewModel';
import NamesEditViewModel from '../Globalization/NamesEditViewModel';
import WebLinksEditViewModel from '../WebLinksEditViewModel';

export default class VenueEditViewModel {
	public constructor(
		private readonly repo: VenueRepository,
		userRepository: UserRepository,
		private readonly urlMapper: UrlMapper,
		contract: VenueForEditContract,
	) {
		this.address = ko.observable(contract.address);
		this.addressCountryCode = ko.observable(contract.addressCountryCode);
		this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
		this.id = contract.id;
		this.latitude = ko.observable(contract.coordinates?.latitude ?? null!);
		this.longitude = ko.observable(contract.coordinates?.longitude ?? null!);
		this.names = NamesEditViewModel.fromContracts(contract.names!);
		this.webLinks = new WebLinksEditViewModel(contract.webLinks);

		this.coordinates = ko.computed<OptionalGeoPointContract>(() => {
			if (!this.latitude() || !this.longitude()) return null!;

			return {
				latitude: !isNaN(this.latitude()) ? this.latitude() : null!,
				longitude: !isNaN(this.longitude()) ? this.longitude() : null!,
			};
		});

		if (contract.id) {
			window.setInterval(
				() =>
					userRepository.refreshEntryEdit({
						entryType: EntryType.Venue,
						entryId: contract.id,
					}),
				10000,
			);
		} else {
			_.forEach(
				[
					this.names.originalName,
					this.names.romajiName,
					this.names.englishName,
				],
				(name) => {
					ko.computed(() => name.value())
						.extend({ rateLimit: 500 })
						.subscribe(this.checkName);
				},
			);
		}
	}

	public address: Observable<string>;
	public addressCountryCode: Observable<string>;

	private checkName = (value: string): void => {
		if (!value) {
			this.duplicateName(null!);
			return;
		}

		this.repo
			.getList({
				query: value,
				nameMatchMode: NameMatchMode.Exact,
				maxResults: 1,
			})
			.then((result) => {
				this.duplicateName(result.items.length ? value : null!);
			});
	};

	public coordinates: Computed<OptionalGeoPointContract>;

	public defaultNameLanguage: Observable<string>;

	public deleteViewModel = new DeleteEntryViewModel((notes) => {
		this.repo
			.delete({ id: this.id, notes: notes, hardDelete: false })
			.then(this.redirectToDetails);
	});

	public description = ko.observable<string>();
	public duplicateName = ko.observable<string>();
	private id: number;
	public latitude: Observable<number>;
	public longitude: Observable<number>;
	public names: NamesEditViewModel;

	private redirectToDetails = (): void => {
		window.location.href = this.urlMapper.mapRelative(
			EntryUrlMapper.details(EntryType.Venue, this.id),
		);
	};

	private redirectToRoot = (): void => {
		window.location.href = this.urlMapper.mapRelative('Event');
	};

	public submitting = ko.observable(false);
	public webLinks: WebLinksEditViewModel;

	public submit = (): boolean => {
		this.submitting(true);
		return true;
	};

	public trashViewModel = new DeleteEntryViewModel((notes) => {
		this.repo
			.delete({ id: this.id, notes: notes, hardDelete: true })
			.then(this.redirectToRoot);
	});
}
