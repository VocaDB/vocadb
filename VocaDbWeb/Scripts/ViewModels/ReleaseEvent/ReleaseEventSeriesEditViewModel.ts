
module vdb.viewModels.releaseEvents {

	import rep = repositories;

	export class ReleaseEventSeriesEditViewModel {

		constructor(
			userRepository: rep.UserRepository,
			id: number,
			defaultNameLanguage: string,
			names: dataContracts.globalization.LocalizedStringWithIdContract[],
			webLinks: dc.WebLinkContract[]) {

			this.defaultNameLanguage = ko.observable(defaultNameLanguage);
			this.names = globalization.NamesEditViewModel.fromContracts(names);
			this.webLinks = new WebLinksEditViewModel(webLinks);			

			if (id) {
				window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.ReleaseEventSeries, id), 10000);
			}

		}
		
		public defaultNameLanguage: KnockoutObservable<string>;
		public description = ko.observable<string>();
		public names: globalization.NamesEditViewModel;
		public submitting = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

		public submit = () => {
			this.submitting(true);
			return true;
		}

	}

}