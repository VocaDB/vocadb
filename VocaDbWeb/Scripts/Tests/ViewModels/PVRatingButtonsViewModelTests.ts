/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../ViewModels/PVRatingButtonsViewModel.ts" />
/// <reference path="../TestSupport/FakeUserRepository.ts" />

import FakeUserRepository from '../TestSupport/FakeUserRepository';
import PVRatingButtonsViewModel from '../../ViewModels/PVRatingButtonsViewModel';
import SongVoteRating from '../../Models/SongVoteRating';

//module vdb.tests.viewModels {

    var repository = new FakeUserRepository();

    QUnit.module("PVRatingButtonsViewModel");

    function createTarget(songId: number, rating: SongVoteRating) {
        return new PVRatingButtonsViewModel(repository, { id: songId, vote: SongVoteRating[rating] }, null)
    }

    test("constructor", () => {

        var target = createTarget(39, SongVoteRating.Nothing);

        equal(target.rating(), SongVoteRating.Nothing, "rating");
        equal(target.isRated(), false, "isRated");
        equal(target.isRatingFavorite(), false, "isRatingFavorite");
        equal(target.isRatingLike(), false, "isRatingLike");

    });

    test("setRating_like", () => {

        var target = createTarget(39, SongVoteRating.Nothing);
        target.setRating_like();

        equal(target.rating(), SongVoteRating.Like, "rating");
        equal(target.isRated(), true, "isRated");
        equal(target.isRatingFavorite(), false, "isRatingFavorite");
        equal(target.isRatingLike(), true, "isRatingLike");
        equal(repository.songId, 39, "repository.songId");
        equal(repository.rating, SongVoteRating.Like, "repository.rating");

    });

//}