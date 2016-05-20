
module vdb.viewModels.releaseEvents {
	
	export class ReleaseEventEditViewModel {

		constructor(date: string, series: models.IEntryWithIdAndName) {

			this.date = ko.observable(date ? moment(date).toDate() : null);
			this.dateStr = ko.computed(() => (this.date() ? this.date().toISOString() : null));

			this.series = new BasicEntryLinkViewModel(series, null);
			this.isSeriesEvent = ko.observable(!this.series.isEmpty());

			this.isSeriesEventStr = ko.computed<string>({
				read: () => this.isSeriesEvent() ? "true" : "false",
				write: (val) => this.isSeriesEvent(val === "true")
			});

			this.isSeriesEvent.subscribe(val => {
				if (!val)
					this.series.clear();
			});

		}

		public customName = ko.observable(false);

		public date: KnockoutObservable<Date>;

		public dateStr: KnockoutComputed<string>;

		public description = ko.observable<string>();

		public isSeriesEvent: KnockoutObservable<boolean>;

		public isSeriesEventStr: KnockoutComputed<string>;

		public series: BasicEntryLinkViewModel<models.IEntryWithIdAndName>;

		public submit = () => {
			this.submitting(true);
			return true;
		}

		public submitting = ko.observable(false);

	}

}