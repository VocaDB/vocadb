import SongVoteRating from '@Models/SongVoteRating';
import PVRatingButtonsViewModel from '@ViewModels/PVRatingButtonsViewModel';

import FakeUserRepository from '../TestSupport/FakeUserRepository';

var repository = new FakeUserRepository();

function createTarget(
	songId: number,
	rating: SongVoteRating,
): PVRatingButtonsViewModel {
	return new PVRatingButtonsViewModel(
		repository,
		{ id: songId, vote: SongVoteRating[rating] },
		null!,
	);
}

test('constructor', () => {
	var target = createTarget(39, SongVoteRating.Nothing);

	expect(target.rating(), 'rating').toBe(SongVoteRating.Nothing);
	expect(target.isRated(), 'isRated').toBe(false);
	expect(target.isRatingFavorite(), 'isRatingFavorite').toBe(false);
	expect(target.isRatingLike(), 'isRatingLike').toBe(false);
});

test('setRating_like', () => {
	var target = createTarget(39, SongVoteRating.Nothing);
	target.setRating_like();

	expect(target.rating(), 'rating').toBe(SongVoteRating.Like);
	expect(target.isRated(), 'isRated').toBe(true);
	expect(target.isRatingFavorite(), 'isRatingFavorite').toBe(false);
	expect(target.isRatingLike(), 'isRatingLike').toBe(true);
	expect(repository.songId, 'repository.songId').toBe(39);
	expect(repository.rating, 'repository.rating').toBe(SongVoteRating.Like);
});
