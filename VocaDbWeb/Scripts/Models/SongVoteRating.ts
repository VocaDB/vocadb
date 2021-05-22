// Song vote values.
// Corresponds to the enum SongVoteRating.
enum SongVoteRating {
  Nothing = 0,
  Like = 3,
  Favorite = 5,
}

export default SongVoteRating;

export function parseSongVoteRating(rating: string): SongVoteRating {
  switch (rating) {
    case 'Nothing':
      return SongVoteRating.Nothing;
    case 'Like':
      return SongVoteRating.Like;
    case 'Favorite':
      return SongVoteRating.Favorite;
    default:
      return undefined!;
  }
}
