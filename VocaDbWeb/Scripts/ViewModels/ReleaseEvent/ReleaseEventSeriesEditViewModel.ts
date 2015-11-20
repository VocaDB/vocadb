
module vdb.viewModels.releaseEvents {

	export class ReleaseEventSeriesEditViewModel {
		
		public description = ko.observable<string>();

		public submit = () => {
			this.submitting(true);
			return true;
		}

		public submitting = ko.observable(false);

	}

}