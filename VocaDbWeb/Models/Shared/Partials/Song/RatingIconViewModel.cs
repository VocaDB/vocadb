using VocaDb.Model.Domain.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song
{
	public class RatingIconViewModel
	{
		public RatingIconViewModel(SongVoteRating rating)
		{
			Rating = rating;
		}

		public SongVoteRating Rating { get; set; }
	}
}