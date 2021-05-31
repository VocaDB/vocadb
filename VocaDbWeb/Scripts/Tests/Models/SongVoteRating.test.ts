import { parseSongVoteRating } from '@Models/SongVoteRating';
import SongVoteRating from '@Models/SongVoteRating';

test('parseSongVoteRating nothing', () => {
  var result = parseSongVoteRating('Nothing');

  expect(result, 'result').toBe(SongVoteRating.Nothing);
});

test('parseSongVoteRating like', () => {
  var result = parseSongVoteRating('Like');

  expect(result, 'result').toBe(SongVoteRating.Like);
});
