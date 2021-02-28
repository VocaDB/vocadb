using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.Domain.Songs
{
	[TestClass]
	public class SongVoteRatingTests
	{
		[DataRow(0, SongVoteRating.Nothing)]
		[DataRow(1, SongVoteRating.Dislike)]
		[DataRow(3, SongVoteRating.Like)]
		[DataRow(5, SongVoteRating.Favorite)]
		[TestMethod]
		public void Value(int expected, SongVoteRating actual)
		{
			((int)actual).Should().Be(expected);
		}
	}
}
