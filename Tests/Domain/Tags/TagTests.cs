using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Tags;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Tags {

	/// <summary>
	/// Tests for <see cref="Tag"/>.
	/// </summary>
	[TestClass]
	public class TagTests {

		[TestMethod]
		public void SyncRelatedTags() {

			var tag1 = CreateEntry.Tag("rock");
			var tag2 = CreateEntry.Tag("metal");
			var tag3 = CreateEntry.Tag("power metal");
			var repository = new FakeTagRepository(tag1, tag2, tag3);

			tag1.AddRelatedTag(tag2);

			var result = tag1.SyncRelatedTags(new[] { tag2, tag3 }, repository.Load);

			Assert.AreEqual(1, result.Added.Length, "Number of added items");
			Assert.AreEqual(1, result.Unchanged.Length, "Number of unchanged items");
			Assert.AreEqual(0, result.Removed.Length, "Number of removed items");

			Assert.AreEqual(2, tag1.RelatedTags.Count, "Number of related tags for tag1");
			Assert.AreEqual(1, tag2.RelatedTags.Count, "Number of related tags for tag2");

		}

	}

}
