#nullable disable

using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Tests.Service.QueryableExtensions;

/// <summary>
/// Tests for <see cref="GenericQueryableExtensions"/>
/// </summary>
public class GenericQueryableExtensionsTests
{
	[TestClass]
	public class SelectObject
	{
		class Song
		{
			public string Name { get; set; }
			public int Length { get; set; }
			public string Description { get; set; }
			public string Artists { get; set; }
		}

		class SongDto
		{
			public string Name { get; set; }
			public int Length { get; set; }
		}

		class SongDto2
		{
			public string Name { get; set; }
			public double Length { get; set; }
		}

		private readonly IQueryable<Song> songs = new[] {
			new Song { Name = "Nebula", Length = 3939, Description = "Cool Mikuelectro", Artists = "Tripshots feat. Miku" },
			new Song { Name = "Rise to Eternity", Length = 39, Description = "Gumimetal", Artists = "A-DASH feat. Gumi" },
		}.AsQueryable();

		[TestMethod]
		public void MapToSubset()
		{
			var result = songs.SelectObject<Song, SongDto>().ToArray();

			result.Length.Should().Be(2, "Number of results");
			result[0].Name.Should().Be("Nebula", "First song name");
			result[0].Length.Should().Be(3939, "First song length");
		}

		[TestMethod]
		public void IgnoreIncompatibleProperties()
		{
			var result = songs.SelectObject<Song, SongDto2>().ToArray();

			result.Length.Should().Be(2, "Number of results");
			result[0].Name.Should().Be("Nebula", "First song name");
			result[0].Length.Should().Be(0, "First song length");
		}
	}
}
