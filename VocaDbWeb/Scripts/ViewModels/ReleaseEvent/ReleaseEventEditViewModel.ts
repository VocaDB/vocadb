
module vdb.viewModels.releaseEvents {
	
	export class ReleaseEventEditViewModel {
		
		constructor(date: string) {

			this.date = ko.observable(date ? moment(date).toDate() : null);
			this.dateStr = ko.computed(() => (this.date() ? this.date().toISOString() : null));

		}

		public date: KnockoutObservable<Date>;

		public dateStr: KnockoutComputed<string>;

		public submit = () => {
			this.submitting(true);
			return true;
		}

		public submitting = ko.observable(false);

	}

}