
module vdb.viewModels.venues {

	export class VenueEditViewModel {

		constructor(
			private readonly repo: rep.VenueRepository,
			userRepository: rep.UserRepository,
			private readonly urlMapper: vdb.UrlMapper,
			contract: dc.VenueForEditContract) {

			this.address = ko.observable(contract.address);
			this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
			this.id = contract.id;
			this.latitude = ko.observable(contract.coordinates?.latitude ?? null);
			this.longitude = ko.observable(contract.coordinates?.longitude ?? null);
			this.names = globalization.NamesEditViewModel.fromContracts(contract.names);
			this.addressCountryCode = ko.observable(contract.addressCountryCode);
			this.webLinks = new WebLinksEditViewModel(contract.webLinks);

			this.coordinates = ko.computed(() => {

				if (!this.latitude() || !this.longitude())
					return null;

				return {
					latitude: !isNaN(this.latitude()) ? this.latitude() : null,
					longitude: !isNaN(this.longitude()) ? this.longitude() : null
				};

			});
			
			if (contract.id) {
				window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.Venue, contract.id), 10000);				
			} else {
				_.forEach([this.names.originalName, this.names.romajiName, this.names.englishName], name => {
					ko.computed(() => name.value()).extend({ rateLimit: 500 }).subscribe(this.checkName);
				});
			}

		}

		public address: KnockoutObservable<string>;
		
		private checkName = (value: string) => {

			if (!value) {
				this.duplicateName(null);
				return;				
			}

			this.repo.getList(value, vdb.models.NameMatchMode.Exact, 1, result => {				
				this.duplicateName(result.items.length ? value : null);
			});

		}

		public coordinates: KnockoutComputed<dc.OptionalGeoPointContract>;
		
		public defaultNameLanguage: KnockoutObservable<string>;
		
		public deleteViewModel = new DeleteEntryViewModel(notes => {
			this.repo.delete(this.id, notes, false, this.redirectToDetails);
		});

		public description = ko.observable<string>();
		public duplicateName = ko.observable<string>();
		private id: number;
		public latitude: KnockoutObservable<number>;
		public longitude: KnockoutObservable<number>;
		public names: globalization.NamesEditViewModel;
		
		private redirectToDetails = () => {
			window.location.href = this.urlMapper.mapRelative(utils.EntryUrlMapper.details(models.EntryType.Venue, this.id));
		}

		private redirectToRoot = () => {
			window.location.href = this.urlMapper.mapRelative("Event");
		}

		public addressCountryCode: KnockoutObservable<string>;
		public submitting = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

		public submit = () => {
			this.submitting(true);
			return true;
		}
		
		public trashViewModel = new DeleteEntryViewModel(notes => {
			this.repo.delete(this.id, notes, true, this.redirectToRoot);
		});

	}

}