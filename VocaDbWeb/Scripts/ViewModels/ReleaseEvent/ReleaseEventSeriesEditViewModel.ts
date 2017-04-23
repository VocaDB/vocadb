
module vdb.viewModels.releaseEvents {

	export class ReleaseEventSeriesEditViewModel {

		constructor(defaultNameLanguage: string,
			names: dataContracts.globalization.LocalizedStringWithIdContract[],
			webLinks: dc.WebLinkContract[]) {

			this.defaultNameLanguage = ko.observable(defaultNameLanguage);
			this.names = globalization.NamesEditViewModel.fromContracts(names);
			this.webLinks = new WebLinksEditViewModel(webLinks);			

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