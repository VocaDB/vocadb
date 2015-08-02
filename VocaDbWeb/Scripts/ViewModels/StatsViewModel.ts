
module vdb.viewModels {
	
	export class StatsViewModel {

		public categories: IReportCategory[];
		public chartData = ko.observable<any>(null);
		private selectedReport = ko.observable<IReport>(null);
		public timespan = ko.observable<string>(null);

		public showTimespanFilter = ko.computed(() => {
			return this.selectedReport && this.selectedReport() && this.selectedReport().allowTimespan;
		});

		private updateReport = () => {
			
			var cutoff = this.showTimespanFilter() && this.timespan() ? moment().subtract(parseInt(this.timespan()), "hours").toISOString() : null;

			$.getJSON("/stats/" + this.selectedReport().url, { cutoff: cutoff }, data => {
				this.chartData(data);
			});

		}

		constructor() {

			this.categories = [
				{
					name: 'Producers', reports: [
						{ name: 'Songs by producer', url: 'songsPerProducer' },
						{ name: 'Albums by producer', url: 'albumsPerProducer' },
						{ name: 'Followers by producer', url: 'followersPerProducer' }
					]
				},
				{
					name: 'Vocaloids', reports: [
						{ name: 'Songs by Vocaloid', url: 'songsPerVocaloid', allowTimespan: true },
						{ name: 'Albums by Vocaloid', url: 'albumsPerVocaloid', allowTimespan: true }
					]
				},
				{
					name: 'Albums', reports: [
						{ name: 'Releases by month', url: 'albumsPerMonth' },
						{ name: 'Cumulative albums per day', url: 'cumulativeAlbums' },
						{ name: 'Hits per album', url: 'hitsPerAlbum', allowTimespan: true },
						{ name: 'Albums per genre', url: 'albumsPerGenre' }
					]
				},
				{
					name: 'Songs', reports: [
						{ name: 'Songs added per day', url: 'songsAddedPerDay', allowTimespan: true },
						{ name: 'Songs published per day', url: 'songsPublishedPerDay' },
						{ name: 'Hits per song', url: 'hitsPerSong', allowTimespan: true },
						{ name: 'Songs per genre', url: 'songsPerGenre' }
					]
				},
				{
					name: 'User', reports: [
						{ name: 'Edits per user', url: 'editsPerUser', allowTimespan: true }
					]
				},
				{
					name: 'General', reports: [
						{ name: 'Edits per day', url: 'editsPerDay', allowTimespan: true }
					]
				}
			];

			this.selectedReport.subscribe(this.updateReport);
			this.timespan.subscribe(this.updateReport);
			this.selectedReport(this.categories[0].reports[0]);

		}

	}

	export interface IReportCategory {

		name: string;

		reports: IReport[];

	}

	export interface IReport {

		allowTimespan?: boolean;

		name: string;

		url: string;

	}

} 