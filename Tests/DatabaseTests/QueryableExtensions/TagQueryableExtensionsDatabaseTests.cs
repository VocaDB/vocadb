#nullable disable

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.QueryableExtensions
{
	/// <summary>
	/// Database tests for <see cref="TagQueryableExtensions"/>.
	/// </summary>
	[TestClass]
	public class TagQueryableExtensionsDatabaseTests
	{
		private readonly DatabaseTestContext<IDatabaseContext> _context = new();

		private Tag[] WhereHasName(params string[] names)
		{
			return _context.RunTest(ctx =>
			{
				return ctx.Query<Tag>().WhereHasName(names).ToArray();
			});
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void WhereHasName()
		{
			var tags = WhereHasName("rock", "electronic");

			tags.Length.Should().Be(2, "Number of tags returned");
			tags.Any(t => t.DefaultName == "rock").Should().BeTrue("Found 'rock' tag");
			tags.Any(t => t.DefaultName == "electronic").Should().BeTrue("Found 'electronic' tag");
		}


		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void WhereHasName_Empty()
		{
			var tags = WhereHasName(new string[0]);

			tags.Length.Should().Be(0, "Number of tags returned");
		}
	}
}
