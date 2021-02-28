#nullable disable

using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.Search.Tags
{
	[TestClass]
	public class TagSearchDatabaseTests
	{
		private readonly DatabaseTestContext _context = new();
		private readonly TagQueryParams _queryParams = new()
		{
			SortRule = TagSortRule.Name,
			Common = new CommonSearchParams(),
			Paging = new PagingProperties(0, 100, true)
		};
		private TestDatabase Db => TestContainerManager.TestDatabase;

		private void AssertHasTag(PartialFindResult<Tag> result, Tag expected)
		{
			result.Items.Any(s => s.Equals(expected)).Should().BeTrue($"Found {expected}");
		}

		private PartialFindResult<Tag> CallFind(ContentLanguagePreference languagePreference = ContentLanguagePreference.Default,
			bool onlyMinimalFields = false)
		{
			return _context.RunTest(querySource =>
			{
				var search = new TagSearch(querySource.OfType<Tag>(), languagePreference);

				var watch = new Stopwatch();
				watch.Start();

				var result = search.Find(_queryParams, onlyMinimalFields);

				Console.WriteLine("Test finished in {0}ms", watch.ElapsedMilliseconds);

				return result;
			});
		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListAll()
		{
			var result = CallFind();

			result.Items.Length.Should().Be(4, "Number of results");
			result.TotalCount.Should().Be(4, "Total result count");
			AssertHasTag(result, Db.Tag);
			AssertHasTag(result, Db.Tag2);
			AssertHasTag(result, Db.Tag3);
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void ListAll_MinimalFields()
		{
			var result = CallFind(onlyMinimalFields: true);

			result.Items.Length.Should().Be(4, "Number of results");
			result.TotalCount.Should().Be(4, "Total result count");

			var sampleTag = result.Items[0];
			sampleTag.TranslatedName.Should().NotBeNull("Translated name");
			sampleTag.TranslatedName.Default.Should().Be("alternative rock", "Sample tag default name");
			sampleTag.TranslatedName.DefaultLanguage.Should().Be(ContentLanguageSelection.English, "Sample tag default language");
		}
	}
}
