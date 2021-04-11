#nullable disable

using System;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Users
{
	/// <summary>
	/// Song rated by a user.
	/// </summary>
	public class FavoriteSongForUser : ISongLink, IEntryWithIntId
	{
		public static int GetRatingScore(SongVoteRating rating) => rating switch
		{
			SongVoteRating.Favorite => 3,
			SongVoteRating.Like => 2,
			SongVoteRating.Dislike => -1,
			_ => 0,
		};

		private Song _song;
		private User _user;

		public FavoriteSongForUser()
		{
			Date = DateTime.Now;
		}

		public FavoriteSongForUser(User user, Song song, SongVoteRating rating)
			: this()
		{
			User = user;
			Song = song;
			Rating = rating;
		}

		public virtual DateTime Date { get; set; }

		public virtual int Id { get; set; }

		public virtual SongVoteRating Rating { get; set; }

		public virtual Song Song
		{
			get => _song;
			set
			{
				ParamIs.NotNull(() => value);
				_song = value;
			}
		}

		public virtual User User
		{
			get => _user;
			set
			{
				ParamIs.NotNull(() => value);
				_user = value;
			}
		}

		/// <summary>
		/// Deletes this link and performs any necessary bookkeeping.
		/// Link will be removed from collections on both sides and ratings will be updated.
		/// </summary>
		public virtual void Delete()
		{
			Song.UserFavorites.Remove(this);
			User.FavoriteSongs.Remove(this);

			SetRating(SongVoteRating.Nothing);
		}

#nullable enable
		public virtual bool Equals(FavoriteSongForUser? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as FavoriteSongForUser);
		}
#nullable disable

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

#nullable enable
		public virtual void Move(Song target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.UserFavorites.Remove(this);
			target.UserFavorites.Add(this);

			if (Rating == SongVoteRating.Favorite)
			{
				Song.FavoritedTimes--;
				target.FavoritedTimes++;
			}

			Song.RatingScore -= GetRatingScore(Rating);
			target.RatingScore += GetRatingScore(Rating);

			Song = target;
		}
#nullable disable

		public virtual void SetRating(SongVoteRating newRating)
		{
			if (Rating == newRating)
				return;

			if (newRating == SongVoteRating.Favorite)
				_song.FavoritedTimes++;

			if (Rating == SongVoteRating.Favorite)
				_song.FavoritedTimes--;

			_song.RatingScore -= GetRatingScore(Rating);
			_song.RatingScore += GetRatingScore(newRating);

			Rating = newRating;
		}

#nullable enable
		public override string ToString()
		{
			return $"favorited {Song} for {User}";
		}
#nullable disable
	}
}
