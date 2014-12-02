
module vdb.viewModels {

    import cls = vdb.models;

    // Knockout view model for PV rating buttons
    export class PVRatingButtonsViewModel {

        public isRated: KnockoutComputed<boolean>;

        public isRatingFavorite: KnockoutComputed<boolean>;

        public isRatingLike: KnockoutComputed<boolean>;

        public rating: KnockoutObservable<cls.SongVoteRating>;

        public setRating_favorite: () => void;

        public setRating_like: () => void;

        public setRating_nothing: () => void;

        constructor(repository: vdb.repositories.UserRepository, songWithVoteContract: SongWithVoteContract, ratingCallback: () => void) {

            var songId = songWithVoteContract.id;
            this.rating = ko.observable(cls.parseSongVoteRating(songWithVoteContract.vote));
            this.isRated = ko.computed(() => this.rating() != cls.SongVoteRating.Nothing);
            this.isRatingFavorite = ko.computed(() => this.rating() == cls.SongVoteRating.Favorite);
            this.isRatingLike = ko.computed(() => this.rating() == cls.SongVoteRating.Like);

            var setRating = (rating: cls.SongVoteRating) => {
                this.rating(rating);
                repository.updateSongRating(songId, rating, () => {
                    if (rating != cls.SongVoteRating.Nothing &&  _.isFunction(ratingCallback))
                        ratingCallback();
                });
            }

            this.setRating_favorite = () => setRating(cls.SongVoteRating.Favorite);
            this.setRating_like = () => setRating(cls.SongVoteRating.Like);
            this.setRating_nothing = () => setRating(cls.SongVoteRating.Nothing);

        }

    }

    export interface SongWithVoteContract {
        
        id: number;

        vote: string;
    
    }

}