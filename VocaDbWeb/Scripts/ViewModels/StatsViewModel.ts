
module vdb.viewModels {
	
	export class StatsViewModel {

		public selectedUrl: KnockoutObservable<string>;

		constructor(public categories: IReportCategory[]) {

			this.selectedUrl = ko.observable(categories[0].reports[0].url);

		}

	}

	export interface IReportCategory {

		name: string;

		reports: IReport;

	}

	export interface IReport {

		name: string;

		url: string;

	}

} 