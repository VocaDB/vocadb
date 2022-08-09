import { FrontPageContract } from '@/DataContracts/FrontPageContract';
import { SongWithPVAndVoteContract } from '@/DataContracts/Song/SongWithPVAndVoteContract';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { NewsListStore } from '@/Stores/NewsListStore';
import { PVRatingButtonsStore } from '@/Stores/PVRatingButtonsStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

export class FrontPagePVPlayerStore {
	public readonly paging = new ServerSidePagingStore(4);
	@observable public ratingButtonsStore?: PVRatingButtonsStore;
	@observable public selectedSong?: SongWithPVAndVoteContract;

	public constructor(
		values: GlobalValues,
		userRepo: UserRepository,
		data: FrontPageContract,
	) {
		makeObservable(this);

		this.paging.totalItems = data.newSongs.length;

		reaction(
			() => this.selectedSong,
			(song) => {
				if (!song) {
					this.ratingButtonsStore = undefined;
					return;
				}

				userRepo
					.getSongRating({ userId: values.loggedUserId, songId: song.id })
					.then((rating) => {
						runInAction(() => {
							this.ratingButtonsStore = new PVRatingButtonsStore(userRepo, {
								id: song.id,
								vote: rating,
							});
						});
					});
			},
		);

		runInAction(() => {
			this.selectedSong = data.firstSong;
		});
	}
}

export class FrontPageStore {
	public readonly newsListStore: NewsListStore;
	public readonly pvPlayerStore: FrontPagePVPlayerStore;

	public constructor(
		values: GlobalValues,
		userRepo: UserRepository,
		data: FrontPageContract,
	) {
		this.newsListStore = new NewsListStore(values.blogUrl);
		this.pvPlayerStore = new FrontPagePVPlayerStore(values, userRepo, data);
	}
}
