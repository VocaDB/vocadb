#nullable disable

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.SongSearch
{
	/// <summary>
	/// Tests for <see cref="Model.Service.Search.SongSearch.SongSearch"/>.
	/// </summary>
	[TestClass]
	public class SongSearchTests
	{
		private Model.Service.Search.SongSearch.SongSearch _songSearch;
		private readonly SongQueryParams _queryParams = new() { SortRule = SongSortRule.Name };

		[TestInitialize]
		public void SetUp()
		{
			var repo = new FakeSongRepository();
			_songSearch = new Model.Service.Search.SongSearch.SongSearch(repo.CreateContext(),
				Model.Domain.Globalization.ContentLanguagePreference.Default,
				new EntryUrlParser("https://test.vocadb.net"));

			repo.Save(
				CreateEntry.Song(name: "Nebula"),
				CreateEntry.Song(name: "Anger"),
				CreateEntry.Song(name: "Anger [EXTEND RMX]")
			);
		}

		private void AssertResults(PartialFindResult<Song> result, params string[] songNames)
		{
			result.TotalCount.Should().Be(songNames.Length, "Total number of results");
			result.Items.Length.Should().Be(songNames.Length, "Number of returned items");

			foreach (var songName in songNames)
			{
				result.Items.Any(s => s.DefaultName == songName).Should().BeTrue($"Song named '{songName}' was returned");
			}
		}

		private PartialFindResult<Song> CallFind()
		{
			return _songSearch.Find(_queryParams);
		}

		[TestMethod]
		public void Find_NameWords()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("Anger");

			var result = CallFind();

			AssertResults(result, "Anger", "Anger [EXTEND RMX]");
		}

		[TestMethod]
		public void Find_NameMatchModeExact()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("Anger", NameMatchMode.Exact);

			var result = CallFind();

			AssertResults(result, "Anger");
		}

		/// <summary>
		/// Find by exact name because the name is quoted.
		/// </summary>
		[TestMethod]
		public void Find_NameQuotedExact()
		{
			_queryParams.Common.TextQuery = SearchTextQuery.Create("\"Anger\"");

			var result = CallFind();

			AssertResults(result, "Anger");
		}

		[TestMethod]
		public void ParseTextQuery_Empty()
		{
			var result = _songSearch.ParseTextQuery(SearchTextQuery.Empty);

			result.HasNameQuery.Should().BeFalse("HasNameQuery");
		}

		[TestMethod]
		public void ParseTextQuery_Name()
		{
			var result = _songSearch.ParseTextQuery(SearchTextQuery.Create("Hatsune Miku"));

			result.HasNameQuery.Should().BeTrue("HasNameQuery");
			result.Name.Query.Should().Be("Hatsune Miku", "Name query");
		}

		[TestMethod]
		public void ParseTextQuery_Id()
		{
			var result = _songSearch.ParseTextQuery(SearchTextQuery.Create("id:3939"));

			result.HasNameQuery.Should().BeFalse("HasNameQuery");
			result.Id.Should().Be(3939, "Id query");
		}

		[TestMethod]
		public void ParseTextQuery_IdInvalid()
		{
			var result = _songSearch.ParseTextQuery(SearchTextQuery.Create("id:miku!"));

			result.Id.Should().Be(0, "Id query");
		}

		[TestMethod]
		public void ParseTextQuery_DateAfter()
		{
			var result = _songSearch.ParseTextQuery(SearchTextQuery.Create("publish-date:2015/9/3"));

			result.PublishedAfter.Should().Be(new DateTime(2015, 9, 3), "Publish date after");
		}

		[TestMethod]
		public void ParseTextQuery_DateRange()
		{
			var result = _songSearch.ParseTextQuery(SearchTextQuery.Create("publish-date:2015/9/3-2015/10/1"));

			result.PublishedAfter.Should().Be(new DateTime(2015, 9, 3), "Publish date after");
			result.PublishedBefore.Should().Be(new DateTime(2015, 10, 1), "Publish date before");
		}

		[TestMethod]
		public void ParseTextQuery_DateInvalid()
		{
			var result = _songSearch.ParseTextQuery(SearchTextQuery.Create("publish-date:Miku!"));

			result.PublishedAfter.Should().BeNull("Publish date after");
		}
	}
}
