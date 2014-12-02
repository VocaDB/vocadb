using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Users {

	public class FavoriteSongForUser : ISongLink {

		public static int GetRatingScore(SongVoteRating rating) {

			switch (rating) {
				case SongVoteRating.Favorite:
					return 3;
				case SongVoteRating.Like:
					return 2;
				case SongVoteRating.Dislike:
					return -1;
				default:
					return 0;
			}

		}

		private Song song;
		private User user;

		public FavoriteSongForUser() {}

		public FavoriteSongForUser(User user, Song song, SongVoteRating rating) {

			User = user;
			Song = song;
			Rating = rating;

		}

		public virtual int Id { get; set; }

		public virtual SongVoteRating Rating { get; set; }

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		/// <summary>
		/// Deletes this link and performs any necessary bookkeeping.
		/// Link will be removed from collections on both sides and ratings will be updated.
		/// </summary>
		public virtual void Delete() {

			Song.UserFavorites.Remove(this);
			User.FavoriteSongs.Remove(this);

			SetRating(SongVoteRating.Nothing);

		}

		public virtual bool Equals(FavoriteSongForUser another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as FavoriteSongForUser);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public virtual void Move(Song target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.UserFavorites.Remove(this);
			target.UserFavorites.Add(this);

			if (Rating == SongVoteRating.Favorite) {
				Song.FavoritedTimes--;
				target.FavoritedTimes++;
			}

			Song.RatingScore -= GetRatingScore(Rating);
			target.RatingScore += GetRatingScore(Rating);

			Song = target;

		}

		public virtual void SetRating(SongVoteRating newRating) {

			if (Rating == newRating)
				return;

			if (newRating == SongVoteRating.Favorite)
				song.FavoritedTimes++;

			if (Rating == SongVoteRating.Favorite)
				song.FavoritedTimes--;

			song.RatingScore -= GetRatingScore(Rating);
			song.RatingScore += GetRatingScore(newRating);

			Rating = newRating;

		}

		public override string ToString() {
			return string.Format("favorited {0} for {1}", Song, User);
		}

	}
}
