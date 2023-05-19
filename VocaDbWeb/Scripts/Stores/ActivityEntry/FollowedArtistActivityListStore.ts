import { ActivityEntryContract } from '@/DataContracts/ActivityEntry/ActivityEntryContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { GlobalValues } from '@/Shared/GlobalValues';
import { HttpClient, UrlMapper } from '@/vdb';
import { makeObservable, observable, runInAction } from 'mobx';

export class FollowedArtistsActivityListStore {
	@observable entries: ActivityEntryContract[] = [];

	constructor(
		private readonly values: GlobalValues,
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		makeObservable(this);
	}

	loadMore = async (): Promise<void> => {
		const result = await this.httpClient.get<
			PartialFindResultContract<ActivityEntryContract>
		>(
			this.urlMapper.mapRelative('/api/activityEntries/followedArtistActivity'),
		);

		const entries = result.items;

		if (!entries) return;

		runInAction(() => {
			this.entries.push(...entries);
		});
	};
}
