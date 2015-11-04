
module vdb.viewModels.releaseEvents {
	
	export class ReleaseEventEditViewModel {
		
		constructor(date: string) {

			this.date = ko.observable(date ? moment(date).toDate() : null);
			this.dateStr = ko.computed(() => (this.date() ? this.date().toISOString() : null));

		}

		public date: KnockoutObservable<Date>;

		public dateStr: KnockoutComputed<string>;

	}

}