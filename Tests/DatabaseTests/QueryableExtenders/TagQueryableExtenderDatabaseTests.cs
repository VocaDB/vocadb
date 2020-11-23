using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests.QueryableExtenders
{

	/// <summary>
	/// Database tests for <see cref="TagQueryableExtender"/>.
	/// </summary>
	[TestClass]
	public class TagQueryableExtenderDatabaseTests
	{

		private readonly DatabaseTestContext<IDatabaseContext> context = new DatabaseTestContext<IDatabaseContext>();

		private Tag[] WhereHasName(params string[] names)
		{

			return context.RunTest(ctx =>
			{
				return ctx.Query<Tag>().WhereHasName(names).ToArray();
			});

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void WhereHasName()
		{

			var tags = WhereHasName("rock", "electronic");

			Assert.AreEqual(2, tags.Length, "Number of tags returned");
			Assert.IsTrue(tags.Any(t => t.DefaultName == "rock"), "Found 'rock' tag");
			Assert.IsTrue(tags.Any(t => t.DefaultName == "electronic"), "Found 'electronic' tag");

		}


		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void WhereHasName_Empty()
		{

			var tags = WhereHasName(new string[0]);

			Assert.AreEqual(0, tags.Length, "Number of tags returned");

		}

	}

}
