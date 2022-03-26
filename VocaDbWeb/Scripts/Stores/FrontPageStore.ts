import FrontPageContract from '@DataContracts/FrontPageContract';
import SongWithPVAndVoteContract from '@DataContracts/Song/SongWithPVAndVoteContract';
import UserRepository from '@Repositories/UserRepository';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

import GlobalValues from '../Shared/GlobalValues';
import NewsListStore from './NewsListStore';
import PVRatingButtonsStore from './PVRatingButtonsStore';
import ServerSidePagingStore from './ServerSidePagingStore';

export class PVPlayerStore {
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

export default class FrontPageStore {
	public readonly newsListStore: NewsListStore;
	public readonly pvPlayerStore: PVPlayerStore;

	public constructor(
		values: GlobalValues,
		userRepo: UserRepository,
		data: FrontPageContract,
	) {
		this.newsListStore = new NewsListStore(values.blogUrl);
		this.pvPlayerStore = new PVPlayerStore(values, userRepo, data);
	}
}
