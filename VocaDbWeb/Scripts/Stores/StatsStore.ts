import { HttpClient } from '@/Shared/HttpClient';
import {
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';
import moment from 'moment';

interface IReportCategory {
	name: string;

	reports: IReport[];
}

interface IReport {
	allowTimespan?: boolean;

	name: string;

	url: string;
}

export class StatsStore {
	categories: IReportCategory[] = [
		{
			name: 'Producers',
			reports: [
				{ name: 'Songs by producer', url: 'songsPerProducer' },
				{ name: 'Albums by producer', url: 'albumsPerProducer' },
				{ name: 'Followers by producer', url: 'followersPerProducer' },
				{
					name: 'Artists per month',
					url: 'artistsPerMonth',
					allowTimespan: true,
				},
			],
		},
		{
			name: 'Voicebanks',
			reports: [
				{
					name: 'Songs by Vocaloid',
					url: 'songsPerVocaloid',
					allowTimespan: true,
				},
				{
					name: 'Albums by Vocaloid',
					url: 'albumsPerVocaloid',
					allowTimespan: true,
				},
				{
					name: 'Songs by voicebank over time',
					url: 'songsPerVocaloidOverTime',
					allowTimespan: true,
				},
				{
					name: 'Songs by voicebank type over time',
					url: 'GetSongsPerVoicebankTypeOverTime',
					allowTimespan: true,
				},
				{
					name: 'Songs by UTAU over time',
					url: 'songsPerVocaloidOverTime?vocalistTypes=UTAU',
					allowTimespan: true,
				},
			],
		},
		{
			name: 'Albums',
			reports: [
				{ name: 'Releases by month', url: 'albumsPerMonth' },
				{ name: 'Cumulative albums per day', url: 'cumulativeAlbums' },
				{ name: 'Hits per album', url: 'hitsPerAlbum', allowTimespan: true },
				{ name: 'Albums per genre', url: 'albumsPerGenre' },
			],
		},
		{
			name: 'Songs',
			reports: [
				{
					name: 'Songs added per day',
					url: 'songsAddedPerDay',
					allowTimespan: true,
				},
				{
					name: 'Songs published per day',
					url: 'songsPublishedPerDay',
					allowTimespan: true,
				},
				{
					name: 'Songs published per month',
					url: 'songsPublishedPerDay?unit=Month',
					allowTimespan: true,
				},
				{
					name: 'Cumulative songs published per day',
					url: 'cumulativeSongsPublished',
					allowTimespan: true,
				},
				{ name: 'Views per song', url: 'hitsPerSong', allowTimespan: true },
				{
					name: 'Views per song over time',
					url: 'hitsPerSongOverTime',
					allowTimespan: true,
				},
				{
					name: 'Score per song over time',
					url: 'scorePerSongOverTime',
					allowTimespan: true,
				},
				{ name: 'Songs per genre', url: 'songsPerGenre' },
				{ name: 'Album songs over time', url: 'albumSongsOverTime' },
			],
		},
		{
			name: 'PVs',
			reports: [
				{ name: 'Original PVs over time', url: 'pvsPerServiceOverTime' },
				{
					name: 'Original PVs per service',
					url: 'pvsPerService?onlyOriginal=true',
					allowTimespan: true,
				},
				{
					name: 'All PVs per service',
					url: 'pvsPerService?onlyOriginal=false',
					allowTimespan: true,
				},
			],
		},
		{
			name: 'User',
			reports: [
				{ name: 'Edits per user', url: 'editsPerUser', allowTimespan: true },
				{ name: 'Users per language', url: 'usersPerLanguage' },
			],
		},
		{
			name: 'General',
			reports: [
				{ name: 'Edits per day', url: 'editsPerDay', allowTimespan: true },
			],
		},
	];

	@observable chartData?: any = undefined;
	@observable selectedReport?: IReport = undefined;
	@observable timespan?: string = undefined;

	constructor(private readonly httpClient: HttpClient) {
		makeObservable(this);

		reaction(() => this.selectedReport, this.updateReport);
		reaction(() => this.timespan, this.updateReport);
		runInAction(() => {
			this.selectedReport = this.categories[0].reports[0];
		});
	}

	@computed get showTimespanFilter(): boolean {
		return this.selectedReport?.allowTimespan ?? false;
	}

	private updateReport = (): void => {
		const cutoff =
			this.showTimespanFilter && this.timespan
				? moment().subtract(this.timespan, 'hours').toISOString()
				: undefined;

		this.httpClient
			.get(`/stats/${this.selectedReport?.url}`, { cutoff: cutoff })
			.then((data) =>
				runInAction(() => {
					this.chartData = data;
				}),
			);
	};
}
