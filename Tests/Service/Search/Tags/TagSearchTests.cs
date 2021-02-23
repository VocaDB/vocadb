#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.Tags
{
	/// <summary>
	/// Tests for <see cref="TagSearch"/>.
	/// </summary>
	[TestClass]
	public class TagSearchTests
	{
		private readonly FakeTagRepository _repository = new();

		private PartialFindResult<Tag> CallFind(TagQueryParams queryParams, bool onlyMinimalFields)
		{
			return _repository.HandleQuery(ctx => new TagSearch(ctx, ContentLanguagePreference.English).Find(queryParams, onlyMinimalFields));
		}

		[TestInitialize]
		public void SetUp()
		{
			_repository.Save(
				CreateEntry.Tag("electronic"), CreateEntry.Tag("rock"), CreateEntry.Tag("alternative rock"), CreateEntry.Tag("techno"));
		}

		public void Find_ByName()
		{
			var result = CallFind(new TagQueryParams(new CommonSearchParams(SearchTextQuery.Create("rock"), false, false), new PagingProperties(0, 100, true))
			{
				SortRule = TagSortRule.Name
			}, false);

			result.Should().NotBeNull("result");
			result.Items.Length.Should().Be(2, "Number of items returned");
			result.TotalCount.Should().Be(2, "Total number of items");
			result.Items[0].DefaultName.Should().Be("alternative rock", "First tag name");
			result.Items[1].DefaultName.Should().Be("rock", "Second tag name");
		}

		[TestMethod]
		public void Find_MinimalFields()
		{
			var result = CallFind(new TagQueryParams(new CommonSearchParams(SearchTextQuery.Empty, false, false), new PagingProperties(0, 100, true))
			{
				SortRule = TagSortRule.Name
			}, true);

			result.Items.Length.Should().Be(4, "Number of items returned");
			result.Items[0].DefaultName.Should().Be("alternative rock", "First tag name");
		}
	}
}
