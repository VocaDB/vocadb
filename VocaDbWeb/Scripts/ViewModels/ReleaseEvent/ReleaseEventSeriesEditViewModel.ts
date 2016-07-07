
module vdb.viewModels.releaseEvents {

	export class ReleaseEventSeriesEditViewModel {

		constructor(webLinks: dc.WebLinkContract[]) {
			this.webLinks = new WebLinksEditViewModel(webLinks);			
		}
		
		public description = ko.observable<string>();

		public submit = () => {
			this.submitting(true);
			return true;
		}

		public submitting = ko.observable(false);

        public webLinks: WebLinksEditViewModel;

	}

}