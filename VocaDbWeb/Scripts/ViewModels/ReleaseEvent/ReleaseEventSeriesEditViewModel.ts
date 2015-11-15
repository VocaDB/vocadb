
module vdb.viewModels.releaseEvents {

	export class ReleaseEventSeriesEditViewModel {
		
		public submit = () => {
			this.submitting(true);
			return true;
		}

		public submitting = ko.observable(false);

	}

}