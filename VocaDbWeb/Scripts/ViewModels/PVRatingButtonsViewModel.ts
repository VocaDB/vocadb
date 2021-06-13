import { parseSongVoteRating } from '@Models/SongVoteRating';
import SongVoteRating from '@Models/SongVoteRating';
import UserRepository from '@Repositories/UserRepository';
import ko, { Computed, Observable } from 'knockout';

// Knockout view model for PV rating buttons
export default class PVRatingButtonsViewModel {
	public isRated: Computed<boolean>;

	public isRatingFavorite: Computed<boolean>;

	public isRatingLike: Computed<boolean>;

	public rating: Observable<SongVoteRating>;

	// Rating operation is in progress. Prevents racing conditions.
	public ratingInProgress = ko.observable(false);

	private setRating: (rating: SongVoteRating) => void;
	public setRating_favorite = (): void =>
		this.setRating(SongVoteRating.Favorite);
	public setRating_like = (): void => this.setRating(SongVoteRating.Like);
	public setRating_nothing = (): void => this.setRating(SongVoteRating.Nothing);

	public constructor(
		repository: UserRepository,
		songWithVoteContract: SongWithVoteContract,
		ratingCallback: () => void,
		isLoggedIn = true,
	) {
		var songId = songWithVoteContract.id;
		this.rating = ko.observable(parseSongVoteRating(songWithVoteContract.vote));
		this.isRated = ko.computed(() => this.rating() !== SongVoteRating.Nothing);
		this.isRatingFavorite = ko.computed(
			() => this.rating() === SongVoteRating.Favorite,
		);
		this.isRatingLike = ko.computed(
			() => this.rating() === SongVoteRating.Like,
		);

		this.setRating = (rating: SongVoteRating): void => {
			if (this.ratingInProgress() || !isLoggedIn) return;

			this.ratingInProgress(true);
			this.rating(rating);

			repository
				.updateSongRating({ songId: songId, rating: rating })
				.then(() => {
					if (rating !== SongVoteRating.Nothing && ratingCallback)
						ratingCallback();
				})
				.finally(() => this.ratingInProgress(false));
		};
	}
}

export interface SongWithVoteContract {
	id: number;

	vote: string;
}
