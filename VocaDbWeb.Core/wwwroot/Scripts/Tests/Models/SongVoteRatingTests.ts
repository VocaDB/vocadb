import { parseSongVoteRating } from '../../Models/SongVoteRating';
import SongVoteRating from '../../Models/SongVoteRating';

    QUnit.module("SongVoteRating");

    test("parseSongVoteRating nothing", () => {

        var result = parseSongVoteRating("Nothing");

        equal(result, SongVoteRating.Nothing, "result");

    });

    test("parseSongVoteRating like", () => {

        var result = parseSongVoteRating("Like");

        equal(result, SongVoteRating.Like, "result");

    });