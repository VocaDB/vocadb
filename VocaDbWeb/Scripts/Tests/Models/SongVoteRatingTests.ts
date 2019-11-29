/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/SongVoteRating.ts" />

//module vdb.tests.models {

    import cls = vdb.models;

    QUnit.module("SongVoteRating");

    test("parseSongVoteRating nothing", () => {

        var result = cls.parseSongVoteRating("Nothing");

        equal(result, cls.SongVoteRating.Nothing, "result");

    });

    test("parseSongVoteRating like", () => {

        var result = cls.parseSongVoteRating("Like");

        equal(result, cls.SongVoteRating.Like, "result");

    });

//}