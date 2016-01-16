using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.SongSearch {

	/// <summary>
	/// Tests for <see cref="Model.Service.Search.SongSearch.SongSearch"/>.
	/// </summary>
	[TestClass]
	public class SongSearchTests {

		private Model.Service.Search.SongSearch.SongSearch songSearch;
		private readonly SongQueryParams queryParams = new SongQueryParams { SortRule = SongSortRule.Name };

		[TestInitialize]
		public void SetUp() {

			var repo = new FakeSongRepository();
			songSearch = new Model.Service.Search.SongSearch.SongSearch(repo.CreateContext(), 
				Model.Domain.Globalization.ContentLanguagePreference.Default, 
				new EntryUrlParser("http://test.vocadb.net", "http://test.vocadb.net"));

			repo.Save(
				CreateEntry.Song(name: "Nebula"),
				CreateEntry.Song(name: "Anger"),
				CreateEntry.Song(name: "Anger [EXTEND RMX]")
			);

		}

		private PartialFindResult<Song> CallFind() {
			return songSearch.Find(queryParams);
		}

		[TestMethod]
		public void Find_NameWords() {

			queryParams.Common.TextQuery = SearchTextQuery.Create("Anger");

			var result = CallFind();

			Assert.AreEqual(2, result.TotalCount, "Total number of results");
			Assert.AreEqual(2, result.Items.Length, "Number of returned items");
			Assert.AreEqual("Anger", result.Items[0].DefaultName, "First returned song");
			Assert.AreEqual("Anger [EXTEND RMX]", result.Items[1].DefaultName, "Second returned song");

		}

		// FIXME: currently broken
		[TestMethod]
		[Ignore]
		public void Find_ExactName() {

			queryParams.Common.TextQuery = SearchTextQuery.Create("\"Anger\"");

			var result = CallFind();

			Assert.AreEqual(1, result.TotalCount, "Total number of results");
			Assert.AreEqual(1, result.Items.Length, "Number of returned items");
			Assert.AreEqual("Anger", result.Items[0].DefaultName, "Returned song");

		}

		[TestMethod]
		public void ParseTextQuery_Empty() {

			var result = songSearch.ParseTextQuery(SearchTextQuery.Empty);

			Assert.IsFalse(result.HasNameQuery, "HasNameQuery");

		}

		[TestMethod]
		public void ParseTextQuery_Name() {

			var result = songSearch.ParseTextQuery(SearchTextQuery.Create("Hatsune Miku"));

			Assert.IsTrue(result.HasNameQuery, "HasNameQuery");
			Assert.AreEqual("Hatsune Miku", result.Name, "Name query");

		}

		[TestMethod]
		public void ParseTextQuery_Id() {

			var result = songSearch.ParseTextQuery(SearchTextQuery.Create("id:3939"));

			Assert.IsFalse(result.HasNameQuery, "HasNameQuery");
			Assert.AreEqual(3939, result.Id, "Id query");

		}

		[TestMethod]
		public void ParseTextQuery_IdInvalid() {

			var result = songSearch.ParseTextQuery(SearchTextQuery.Create("id:miku!"));

			Assert.AreEqual(0, result.Id, "Id query");

		}

		[TestMethod]
		public void ParseTextQuery_DateAfter() {

			var result = songSearch.ParseTextQuery(SearchTextQuery.Create("publish-date:2015/9/3"));

			Assert.AreEqual(new DateTime(2015, 9, 3), result.PublishedAfter, "Publish date after");

		}

		[TestMethod]
		public void ParseTextQuery_DateRange() {

			var result = songSearch.ParseTextQuery(SearchTextQuery.Create("publish-date:2015/9/3-2015/10/1"));

			Assert.AreEqual(new DateTime(2015, 9, 3), result.PublishedAfter, "Publish date after");
			Assert.AreEqual(new DateTime(2015, 10, 1), result.PublishedBefore, "Publish date before");

		}

		[TestMethod]
		public void ParseTextQuery_DateInvalid() {

			var result = songSearch.ParseTextQuery(SearchTextQuery.Create("publish-date:Miku!"));

			Assert.IsNull(result.PublishedAfter, "Publish date after");

		}

	}

}
