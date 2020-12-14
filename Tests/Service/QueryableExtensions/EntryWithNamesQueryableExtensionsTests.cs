#nullable disable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Service.QueryableExtensions
{
	/// <summary>
	/// Tests for <see cref="EntryWithNamesQueryableExtensions"/>.
	/// </summary>
	[TestClass]
	public class EntryWithNamesQueryableExtensionsTests
	{
		private Song[] songs;

		public EntryWithNamesQueryableExtensionsTests()
		{
			songs = new[] {
				CreateEntry.Song(name: "Nebula"),
				CreateEntry.Song(name: "Next Stage"),
				CreateEntry.Song(name: "Symphony"),
				CreateEntry.Song(name: "Japanese Emotion remix")
			};
		}

		private Song[] WhereHasNameGeneric(IEnumerable<Song> songs, params SearchTextQuery[] queries)
		{
			return songs
				.AsQueryable()
				.WhereHasNameGeneric<Song, SongName>(queries)
				.ToArray();
		}

		private void AssertSong(Song[] list, params string[] expectedNames)
		{
			Assert.AreEqual(expectedNames.Length, list.Length);

			foreach (var name in expectedNames)
			{
				Assert.IsTrue(list.Any(s => s.DefaultName == name), name);
			}
		}

		[TestMethod]
		public void WhereHasNameGeneric_SingleName_StartsWith()
		{
			var result = WhereHasNameGeneric(songs, SearchTextQuery.Create("Ne", NameMatchMode.StartsWith));

			AssertSong(result, "Nebula", "Next Stage");
		}

		[TestMethod]
		public void WhereHasNameGeneric_MultipleNames_StartsWith()
		{
			var result = WhereHasNameGeneric(songs, SearchTextQuery.Create("Ne", NameMatchMode.StartsWith), SearchTextQuery.Create("Sym", NameMatchMode.StartsWith));

			AssertSong(result, "Nebula", "Next Stage", "Symphony");
		}

		[TestMethod]
		public void WhereHasNameGeneric_MultipleNames_Words()
		{
			var result = WhereHasNameGeneric(songs, SearchTextQuery.Create("Stage", NameMatchMode.Words), SearchTextQuery.Create("Emotion remix", NameMatchMode.Words));

			AssertSong(result, "Next Stage", "Japanese Emotion remix");
		}
	}
}
