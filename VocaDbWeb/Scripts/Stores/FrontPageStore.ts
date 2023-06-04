import { FrontPageContract } from '@/DataContracts/FrontPageContract';
import type { SongWithPVAndVoteContract } from '@/DataContracts/Song/SongWithPVAndVoteContract';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { PVRatingButtonsStore } from '@/Stores/PVRatingButtonsStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

export class FrontPagePVPlayerStore {
	readonly paging = new ServerSidePagingStore(4);
	@observable ratingButtonsStore?: PVRatingButtonsStore;
	@observable selectedSong?: SongWithPVAndVoteContract;

	constructor(
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
	readonly pvPlayerStore: FrontPagePVPlayerStore;

	constructor(
		values: GlobalValues,
		userRepo: UserRepository,
		data: FrontPageContract,
	) {
		this.pvPlayerStore = new FrontPagePVPlayerStore(values, userRepo, data);
	}
}
