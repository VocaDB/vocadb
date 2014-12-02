/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../ViewModels/PVRatingButtonsViewModel.ts" />
/// <reference path="../TestSupport/FakeUserRepository.ts" />

module vdb.tests.viewModels {

    import cls = vdb.models;
    import vm = vdb.viewModels;

    var repository = new vdb.tests.testSupport.FakeUserRepository();

    QUnit.module("PVRatingButtonsViewModel");

    function createTarget(songId: number, rating: cls.SongVoteRating) {
        return new vm.PVRatingButtonsViewModel(repository, { id: songId, vote: cls.SongVoteRating[rating] }, null)
    }

    test("constructor", () => {

        var target = createTarget(39, cls.SongVoteRating.Nothing);

        equal(target.rating(), cls.SongVoteRating.Nothing, "rating");
        equal(target.isRated(), false, "isRated");
        equal(target.isRatingFavorite(), false, "isRatingFavorite");
        equal(target.isRatingLike(), false, "isRatingLike");

    });

    test("setRating_like", () => {

        var target = createTarget(39, cls.SongVoteRating.Nothing);
        target.setRating_like();

        equal(target.rating(), cls.SongVoteRating.Like, "rating");
        equal(target.isRated(), true, "isRated");
        equal(target.isRatingFavorite(), false, "isRatingFavorite");
        equal(target.isRatingLike(), true, "isRatingLike");
        equal(repository.songId, 39, "repository.songId");
        equal(repository.rating, cls.SongVoteRating.Like, "repository.rating");

    });

}