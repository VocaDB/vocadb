import { parseSongVoteRating, SongVoteRating } from '@/Models/SongVoteRating';
import { UserRepository } from '@/Repositories/UserRepository';
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
export class PVRatingButtonsStore {
	@observable rating: SongVoteRating;
	// Rating operation is in progress. Prevents racing conditions.
	@observable ratingInProgress = false;
	private readonly songId: number;

	constructor(
		private readonly userRepo: UserRepository,
		songWithVoteContract: SongWithVoteContract,
		private readonly ratingCallback?: () => void,
		private readonly isLoggedIn = true,
	) {
		makeObservable(this);

		this.songId = songWithVoteContract.id;
		this.rating = parseSongVoteRating(songWithVoteContract.vote);
	}

	@computed get isRated(): boolean {
		return this.rating !== SongVoteRating.Nothing;
	}

	@computed get isRatingFavorite(): boolean {
		return this.rating === SongVoteRating.Favorite;
	}

	@computed get isRatingLike(): boolean {
		return this.rating === SongVoteRating.Like;
	}

	@action setRating = (rating: SongVoteRating): Promise<void> => {
		if (this.ratingInProgress || !this.isLoggedIn) return Promise.resolve();

		this.ratingInProgress = true;
		this.rating = rating;

		return this.userRepo
			.updateSongRating({ songId: this.songId, rating: rating })
			.then(() => {
				if (rating !== SongVoteRating.Nothing) this.ratingCallback?.();
			})
			.finally(() =>
				runInAction(() => {
					this.ratingInProgress = false;
				}),
			);
	};

	setRating_favorite = (): Promise<void> =>
		this.setRating(SongVoteRating.Favorite);

	setRating_like = (): Promise<void> => this.setRating(SongVoteRating.Like);

	setRating_nothing = (): Promise<void> =>
		this.setRating(SongVoteRating.Nothing);
}
