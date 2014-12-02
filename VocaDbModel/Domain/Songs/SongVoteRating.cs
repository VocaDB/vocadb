namespace VocaDb.Model.Domain.Songs {

	/// <summary>
	/// Rating of a song vote.
	/// Saved in DB as integer - do not change the integer values here.
	/// </summary>
	public enum SongVoteRating {

		Nothing		= 0,

		Dislike		= 1,

		Like		= 3,

		Favorite	= 5

	}
}
