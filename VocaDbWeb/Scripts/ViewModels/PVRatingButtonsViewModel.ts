
module vdb.viewModels {

    import cls = vdb.models;

    // Knockout view model for PV rating buttons
    export class PVRatingButtonsViewModel {

        public isRated: KnockoutComputed<boolean>;

        public isRatingFavorite: KnockoutComputed<boolean>;

        public isRatingLike: KnockoutComputed<boolean>;

		public rating: KnockoutObservable<cls.SongVoteRating>;

		// Rating operation is in progress. Prevents racing conditions.
		public ratingInProgress = ko.observable(false);

		private setRating: (rating: cls.SongVoteRating) => void;
		public setRating_favorite = () => this.setRating(cls.SongVoteRating.Favorite);
		public setRating_like = () => this.setRating(cls.SongVoteRating.Like);
		public setRating_nothing = () => this.setRating(cls.SongVoteRating.Nothing);

		constructor(repository: vdb.repositories.UserRepository, songWithVoteContract: SongWithVoteContract, ratingCallback: () => void,
			isLoggedIn = true) {

            var songId = songWithVoteContract.id;
            this.rating = ko.observable(cls.parseSongVoteRating(songWithVoteContract.vote));
            this.isRated = ko.computed(() => this.rating() !== cls.SongVoteRating.Nothing);
            this.isRatingFavorite = ko.computed(() => this.rating() === cls.SongVoteRating.Favorite);
            this.isRatingLike = ko.computed(() => this.rating() === cls.SongVoteRating.Like);

			this.setRating = (rating: cls.SongVoteRating) => {

				if (this.ratingInProgress() || !isLoggedIn)
					return;

				this.ratingInProgress(true);
				this.rating(rating);

                repository.updateSongRating(songId, rating, () => {
                    if (rating !== cls.SongVoteRating.Nothing && ratingCallback)
						ratingCallback();
				}).always(() => this.ratingInProgress(false));

            }

        }

    }

    export interface SongWithVoteContract {
        
        id: number;

        vote: string;
    
    }

}