import SongVoteRating, { parseSongVoteRating } from '@Models/SongVoteRating';
import UserRepository from '@Repositories/UserRepository';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

interface SongWithVoteContract {
	id: number;
	vote: string;
}

// MobX store for PV rating buttons
export default class PVRatingButtonsStore {
	@observable public rating: SongVoteRating;
	// Rating operation is in progress. Prevents racing conditions.
	@observable public ratingInProgress = false;
	private readonly songId: number;

	public constructor(
		private readonly userRepo: UserRepository,
		songWithVoteContract: SongWithVoteContract,
		private readonly ratingCallback: () => void,
		private readonly isLoggedIn = true,
	) {
		makeObservable(this);

		this.songId = songWithVoteContract.id;
		this.rating = parseSongVoteRating(songWithVoteContract.vote);
	}

	@computed public get isRated(): boolean {
		return this.rating !== SongVoteRating.Nothing;
	}

	@computed public get isRatingFavorite(): boolean {
		return this.rating === SongVoteRating.Favorite;
	}

	@computed public get isRatingLike(): boolean {
		return this.rating === SongVoteRating.Like;
	}

	@action public setRating = (rating: SongVoteRating): void => {
		if (this.ratingInProgress || !this.isLoggedIn) return;

		this.ratingInProgress = true;
		this.rating = rating;

		this.userRepo
			.updateSongRating({ songId: this.songId, rating: rating })
			.then(() => {
				if (rating !== SongVoteRating.Nothing && this.ratingCallback)
					this.ratingCallback();
			})
			.finally(() =>
				runInAction(() => {
					this.ratingInProgress = false;
				}),
			);
	};
}
